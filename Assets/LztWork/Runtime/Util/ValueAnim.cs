#define DelayCallback

using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace util
{
    public enum Easing
    {
        Linear,
        EaseInSine,
        EaseOutSine,
        EaseInOutSine,
        EaseInQuad,
        EaseOutQuad,
        EaseInOutQuad,
        EaseInCubic,
        EaseOutCubic,
        EaseInOutCubic,
        EaseInQuart,
        EaseOutQuart,
        EaseInOutQuart,
        EaseInQuint,
        EaseOutQuint,
        EaseInOutQuint,
        EaseInExpo,
        EaseOutExpo,
        EaseInOutExpo,
        EaseInCirc,
        EaseOutCirc,
        EaseInOutCirc,
        EaseInBack,
        EaseOutBack,
        EaseInOutBack,
        EaseInElastic,
        EaseOutElastic,
        EaseInOutElastic,
        EaseInBounce,
        EaseOutBounce,
        Bézier
    }

    public struct AnimData<T>
    {
        public readonly T Start;                     //起始值
        public readonly T End;                       //结束值
        public readonly float Time;                  //值动画时间
        public readonly Easing Easing;               //时间缓动函数
        public readonly Action CallBack;             //值动画结束回调函数
        public readonly List<T> ValueBézier;         //值的贝塞尔控制点，可空
        public readonly List<Vector2> TimeBézier;    //时间的贝塞尔控制点，只有在Easing = Bézier时起效

        public AnimData(T start, T end, float time, Easing easing, Action callBack, List<T> valueBézier, List<Vector2> timeBézier)
        {
            Start = start;
            End = end;
            Time = time;
            Easing = easing;
            CallBack = callBack;
            ValueBézier = valueBézier;
            TimeBézier = timeBézier;
        }
    }

    public abstract class ValueAnim<T>
    {
        private Queue<AnimData<T>> _anim = new();

        private float _timer;

        protected AnimData<T> current => _anim.Peek();
        public bool InAnim => _anim.Count > 0;
        public bool InAnimFunc() => _anim.Count > 0;

        public void StartAnim(T start, T end, float time, Easing easing = Easing.Linear, Action callBack = null, List<T> valueBézier = null, List<Vector2> timeBézier = null)
        {
            _anim.Clear();
            _anim.Enqueue(new(start, end, time, easing, callBack, valueBézier, timeBézier));
            _timer = 0f;
        }
        
        public void StartAnim(Queue<AnimData<T>> anim)
        {
            _anim = anim;
            _timer = 0f;
        }

        public void StopAnim()
        {
            _anim.Clear();
        }
        
        public T GetValue()
        {
            _timer += Time.deltaTime;
            if (_timer < current.Time) return CalValue();
#if DelayCallback
            DelayInvoke(current.CallBack).Forget();
#else
            current.CallBack?.Invoke();
#endif
            if (_anim.Count == 1)
            {
                return _anim.Dequeue().End;
            }
            _timer -= current.Time;
            _anim.Dequeue();
            return CalValue();
        }

        protected float GetRate()
        {
            const float c1 = 1.70158f;
            const float c2 = c1 * 1.525f;
            const float c3 = c1 + 1;
            const float c4 = 2 * Mathf.PI / 3;
            const float c5 = 2 * Mathf.PI / 4.5f;
            const float n1 = 7.5625f;
            const float d1 = 2.75f;

            var x = _timer / current.Time;
            return current.Easing switch
            {
                Easing.Linear => x,
                Easing.EaseInSine => 1 - Mathf.Cos(x * Mathf.PI / 2),
                Easing.EaseOutSine => Mathf.Sin(x * Mathf.PI / 2),
                Easing.EaseInOutSine => (1 - Mathf.Cos(x * Mathf.PI)) / 2,
                Easing.EaseInQuad => x * x,
                Easing.EaseOutQuad => 1 - (1 - x) * (1 - x),
                Easing.EaseInOutQuad => x < 0.5f ? 2 * x * x : 1 - 2 * (1 - x) * (1 - x),
                Easing.EaseInCubic => x * x * x,
                Easing.EaseOutCubic => 1 - (1 - x) * (1 - x) * (1 - x),
                Easing.EaseInOutCubic => x < 0.5f ? 4 * x * x * x : 1 - 4 * (1 - x) * (1 - x) * (1 - x),
                Easing.EaseInQuart => x * x * x * x,
                Easing.EaseOutQuart => 1 - (1 - x) * (1 - x) * (1 - x) * (1 - x),
                Easing.EaseInOutQuart => x < 0.5f ? 8 * x * x * x * x : 1 - 8 * (1 - x) * (1 - x) * (1 - x) * (1 - x),
                Easing.EaseInQuint => x * x * x * x * x,
                Easing.EaseOutQuint => 1 - (1 - x) * (1 - x) * (1 - x) * (1 - x) * (1 - x),
                Easing.EaseInOutQuint => x < 0.5f ? 16 * x * x * x * x * x : 1 - 16 * (1 - x) * (1 - x) * (1 - x) * (1 - x) * (1 - x),
                Easing.EaseInExpo => Mathf.Pow(2, 10 * x - 10),
                Easing.EaseOutExpo => 1 - Mathf.Pow(2, -10 * x),
                Easing.EaseInOutExpo => x < 0.5f ? Mathf.Pow(2, 20 * x - 11) : 1 - Mathf.Pow(2, -20 * x + 9),
                Easing.EaseInCirc => 1 - Mathf.Sqrt(1 - x * x),
                Easing.EaseOutCirc => Mathf.Sqrt((2 - x) * x),
                Easing.EaseInOutCirc => x < 0.5f ? (1 - Mathf.Sqrt(1 - 4 * x * x)) / 2 : (Mathf.Sqrt(1 - 4 * (1 - x) * (1 - x)) + 1) / 2,
                Easing.EaseInBack => c3 * x * x * x - c1 * x * x,
                Easing.EaseOutBack => 1 - c3 * (1 - x) * (1 - x) * (1 - x) + c1 * (1 - x) * (1 - x),
                Easing.EaseInOutBack => x < 0.5f ? 2 * x * x * ((c2 + 1) * 2 * x - c2) : 2 * (1 - x) * (1 - x) * ((c2 + 1) * (x * 2 - 2) + c2) + 1,
                Easing.EaseInElastic => -Mathf.Pow(2, 10 * x - 10) * Mathf.Sin((x * 10 - 10.75f) * c4),
                Easing.EaseOutElastic => Mathf.Pow(2, -10 * x) * Mathf.Sin((x * 10 - 0.75f) * c4) + 1,
                Easing.EaseInOutElastic => x < 0.5f ? -(Mathf.Pow(2, 20 * x - 10) * Mathf.Sin((20 * x - 11.125f) * c5)) / 2 : (Mathf.Pow(2, -20 * x + 10) * Mathf.Sin((20 * x - 11.125f) * c5)) / 2 + 1,
                Easing.EaseInBounce => x < 1 / d1 ? 1 - n1 * x * x : x < 2 / d1 ? 0.25f - n1 * (x - 1.5f / d1) * (x - 1.5f / d1) : x < 2.5f / d1 ? 0.0625f - n1 * (x - 2.25f / d1) * (x - 2.25f / d1) : 0.015625f - n1 * (x - 2.625f / d1) * (x - 2.625f / d1),
                Easing.EaseOutBounce => x < 1 / d1 ? n1 * x * x : x < 2 / d1 ? n1 * (x - 1.5f / d1) * (x - 1.5f / d1) + 0.75f : x < 2.5f / d1 ? n1 * (x - 2.25f / d1) * (x - 2.25f / d1) + 0.9375f : n1 * (x - 2.625f / d1) * (x - 2.625f / d1) + 0.984375f,
                Easing.Bézier => DeCasteljau.ComputePoint(current.TimeBézier, x).y,
                _ => throw new NotImplementedException(),
            };
        }

        protected abstract T CalValue();

#if DelayCallback
        private async UniTask DelayInvoke(Action action)
        {
            await UniTask.Yield();
            action?.Invoke();
        }
#endif
    }

    public class Vector3ValueAnim : ValueAnim<Vector3>
    {
        protected override Vector3 CalValue()
        {
            return current.ValueBézier == null ? current.Start + (current.End - current.Start) * GetRate()
                : DeCasteljau.ComputePoint(current.ValueBézier, GetRate());
        }
    }
    
    public class Vector2ValueAnim : ValueAnim<Vector2>
    {
        protected override Vector2 CalValue()
        {
            return current.ValueBézier == null ? current.Start + (current.End - current.Start) * GetRate()
                : DeCasteljau.ComputePoint(current.ValueBézier, GetRate());
        }
    }

    public class FloatValueAnim : ValueAnim<float>
    {
        protected override float CalValue()
        {
            return current.ValueBézier == null ? current.Start + (current.End - current.Start) * GetRate()
                : DeCasteljau.ComputePoint(current.ValueBézier, GetRate());
        }
    }

    public class ColorValueAnim : ValueAnim<Color>
    {
        protected override Color CalValue()
        {
            return current.ValueBézier == null ? current.Start + (current.End - current.Start) * GetRate()
                : DeCasteljau.ComputePoint(current.ValueBézier, GetRate());
        }
    }
}