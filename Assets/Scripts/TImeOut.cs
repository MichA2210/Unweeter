using System;
using System.Collections;
using UnityEngine;

public class TimeOut
{
    public static IEnumerator Set(float duration, Action OnComplete)
    {
        yield return new WaitForSeconds(duration);
        OnComplete();
    }

    public static Func<float, float> linear = x => x;

    public static Func<float, float> ease = x => -(Mathf.Cos(x * Mathf.PI) / 2) + 0.5f;

    public static IEnumerator InterpolateFloat( float x, float y, float duration, Action<float> OnIteration, Func<float, float> EasingFunc = null, Action OnComplete = null)
    {
        float accum = 0f;
        float alpha;
        while( accum < duration)
        {
            yield return null;
            accum += Time.deltaTime;

            alpha = EasingFunc == null ? accum / duration : EasingFunc(accum / duration);
            OnIteration(Mathf.Lerp(x, y, alpha));
        }

        if(OnComplete != null)
        {
            OnComplete();
        }

    }
}