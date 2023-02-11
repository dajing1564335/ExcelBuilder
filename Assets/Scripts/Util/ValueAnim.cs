using System;
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
    }

    public abstract class ValueAnim<T>
    {
        protected T start;
        protected T end;
        protected float time;
        protected float timer;
        protected Easing easing;
        private Action _callBack;

        public bool InAnim => timer < time;

        public void StartAnim(T start, T end, float time, Easing easing = Easing.Linear, Action callBack = null)
        {
            this.start = start;
            this.end = end;
            this.time = time;
            this.easing = easing;
            _callBack = callBack;
            timer = 0f;
        }

        public T GetValue()
        {
            timer += Time.deltaTime;
            if (timer >= time)
            {
                DelayInvoke();
                return end;
            }
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

            var x = timer / time;
            return easing switch
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
                _ => throw new NotImplementedException(),
            };
        }

        public abstract T CalValue();

        private async void DelayInvoke()
        {
            await System.Threading.Tasks.Task.Yield();
            _callBack?.Invoke();
        }
    }

    public class Vector3ValueAnim : ValueAnim<Vector3>
    {
        public override Vector3 CalValue()
        {
            return start + (end - start) * GetRate();
        }
    }
    
    public class FloatValueAnim : ValueAnim<float>
    {
        public override float CalValue()
        {
            return start + (end - start) * GetRate();
        }
    }

    public class ColorValueAnim : ValueAnim<Color>
    {
        public override Color CalValue()
        {
            return start + (end - start) * GetRate();
        }
    }
}