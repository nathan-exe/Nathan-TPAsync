using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEditor;
using UnityEditor.Profiling.Memory.Experimental;
using UnityEngine;
using static Building;

public class BuildingManager : MonoBehaviour
{
    [Serializable]
    public struct Data
    {
        private int _tenants;
        private Unity.Mathematics.Random _random;

        public Data(Building building)
        {
            _random = new((uint)building.gameObject.GetInstanceID());
            _tenants = building.FloorCount * _random.NextInt(50, 200);
            PowerUsage = 0;
        }

        public float PowerUsage { get; private set; }

        public void UpdatePowerUsage()
        {
            //test pour ralentir le jeu et etre sûr que ça marche bien.
            //avec burst compiler, j'ai 300fps.
            //sans burst compiler, j'ai 12 fps.
            //--
            float lent = 0;
            for (int i = 0; i < 10000; i++)
            {
                lent = math.exp(math.sin(math.sqrt(PowerUsage)));
            }
            //--

            PowerUsage =  _random.NextFloat(1f, 5f) * _tenants ;
        }

    }

    [BurstCompile(CompileSynchronously = true)]
    public struct BuildingUpdateJob : IJobParallelFor
    {
        public NativeArray<Data> BuildingDataArray;
        
        public void Execute(int index)
        {
            Data data = BuildingDataArray[index];
            data.UpdatePowerUsage();
            BuildingDataArray[index] = data;
            //Debug.Log(data.PowerUsage);
        }
    }

    [SerializeField] List<Building> buildings;

    JobHandle _handle;
    BuildingUpdateJob _job;
    NativeArray<Data> _buildingDataArray;


    private void Awake()
    {
        _buildingDataArray = new NativeArray<Data>(buildings.Count, Allocator.Persistent);
        for (int i = 0; i < buildings.Count; i++)
        {
            _buildingDataArray[i] = new Data(buildings[i]);
        }
        _job = new BuildingUpdateJob() { BuildingDataArray = _buildingDataArray };
    }

    private void Update()
    {
        _handle = _job.Schedule(buildings.Count, 1);
    }

    private void LateUpdate()
    {
        _handle.Complete();
    }

    private void OnDestroy()
    {
        _buildingDataArray.Dispose();
    }
}
