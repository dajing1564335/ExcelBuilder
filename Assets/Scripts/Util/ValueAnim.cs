using System;
using UnityEngine;

public abstract class ValueAnim<T>
{
    protected T _start;
    protected T _end;
    protected float _time;
    protected float _remainTime;
    event Action CallBack;

    public bool InAnim => _remainTime > 0;

    public void StartAnim(T start, T end, float time, Action callBack = null)
    {
        _start = start;
        _end = end;
        _time = time;
        _remainTime = time;
        CallBack = callBack;
    }

    public T GetValue()
    {
        _remainTime -= Time.deltaTime;
        if (_remainTime <= 0)
        {
            DelayInvoke();
            return _end;
        }
        return CalValue();
    }

    public abstract T CalValue();

    private async void DelayInvoke()
    {
        await System.Threading.Tasks.Task.Yield();
        CallBack?.Invoke();
    }
}

public class Vector3ValueAnim : ValueAnim<Vector3>
{
    public override Vector3 CalValue()
    {
        return Vector3.Lerp(_end, _start, _remainTime / _time);
    }
}

public class FloatValueAnim : ValueAnim<float>
{
    public override float CalValue()
    {
        return Mathf.Lerp(_end, _start, _remainTime / _time);
    }
}

public class ColorValueAnim : ValueAnim<Color>
{
    public override Color CalValue()
    {
        return Color.Lerp(_end, _start, _remainTime / _time);
    }
}

