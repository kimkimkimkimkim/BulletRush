using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

public static class ButtonExtensions
{
    public static IObservable<Unit> OnClickIntentAsObservable(this Button button, ButtonClickIntent intent = ButtonClickIntent.Normal)
    {
        var clickObservable = button.OnClickAsObservable().Where(_ => !(Input.touchSupported && Input.touches.Length > 1));

        switch (intent)
        {
            case ButtonClickIntent.OnlyOneTap:
                clickObservable = clickObservable.First();
                break;

            case ButtonClickIntent.IntervalTap:
                clickObservable = clickObservable.ThrottleFirst(TimeSpan.FromSeconds(0.5f));
                break;

            case ButtonClickIntent.Normal:
            default:
                break;
        }

        return clickObservable
            .SelectMany(_ =>
            {
                Sequence sequence = DOTween.Sequence()
                    .Append(button.transform.DOScale(Vector3.one * 0.95f, 0.05f))
                    .Append(button.transform.DOScale(Vector3.one, 0.05f));
                return sequence.PlayAsObservable();
            })
            .AsUnitObservable();
    }

    public static IObservable<long> OnHoldAsObservable(this Button button, double ms = 350, Action releaseEvent = null)
    {
        return button.OnPointerDownAsObservable()
            .SelectMany(_ => Observable.Timer(TimeSpan.FromMilliseconds(ms)))
            .TakeUntil(button.OnPointerUpAsObservable()
            .Do(_ => {
                if (releaseEvent != null)releaseEvent();
            }))
            .RepeatUntilDestroy(button)
            .AsObservable();
    }

    public enum ButtonClickIntent
    {
        Normal,
        OnlyOneTap,
        IntervalTap,
    }
}
