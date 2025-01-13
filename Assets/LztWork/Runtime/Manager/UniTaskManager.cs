using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

public class UniTaskManager : SingletonMonoBehaviour<UniTaskManager>
{
    private CancellationTokenSource _cancellationTokenSource;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public UniTask Delay(int millisecondsDelay)
    {
        return UniTask.Delay(millisecondsDelay, cancellationToken: _cancellationTokenSource.Token);
    }

    public UniTask DelayFrame(int delayFrameCount)
    {
        return UniTask.DelayFrame(delayFrameCount, cancellationToken: _cancellationTokenSource.Token);
    }

    public UniTask WaitUntil(Func<bool> predicate)
    {
        return UniTask.WaitUntil(predicate, cancellationToken: _cancellationTokenSource.Token);
    }

    public UniTask WaitWhile(Func<bool> predicate)
    {
        return UniTask.WaitWhile(predicate, cancellationToken: _cancellationTokenSource.Token);
    }

    public void StopTasks()
    {
        _cancellationTokenSource.Cancel();
    }

    public void ResetToken()
    {
        if (_cancellationTokenSource != null)
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }
        _cancellationTokenSource = new();
    }
}
