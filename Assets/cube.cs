using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using Unity.VisualScripting.Antlr3.Runtime;

public class cube : MonoBehaviour
{
    
    Coroutine _currentCoroutine;

    CancellationTokenSource cts;
    UniTask currentTask;

    public void Ctourner() { if (_currentCoroutine == null) _currentCoroutine = StartCoroutine(tourne()); }
    public void Cstop() { if(_currentCoroutine!=null) StopCoroutine(_currentCoroutine); }
    IEnumerator tourne()
    {
        float endTime = Time.time + 5;
        while (Time.time < endTime)
        {
            transform.Rotate(0, 360 * Time.deltaTime, 0);
            yield return null;
        }

    }

    public void Ttourner()
    {
        if (currentTask.Status.IsCompleted())
        {
            cts = new CancellationTokenSource();
            //cts.AddTo(this.GetCancellationTokenOnDestroy());
            currentTask = tourner(cancellationToken: cts.Token);
        }
    }

    public void Tstop()
    {
        cts.Cancel();
    }


    public async UniTask tourner(CancellationToken cancellationToken ) 
    {
        float endTime = Time.time + 5;
        while (Time.time < endTime &! cancellationToken.IsCancellationRequested)
        {
            transform.Rotate(0, 360 * Time.deltaTime, 0);
            await UniTask.NextFrame();
        }
    }
}
