using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class SpawnCircleManager : MonoBehaviour
{
    [SerializeField] private Image _circleImage;

    public IObservable<Unit> PlayAnimationObservable() {
        var time = 2f;

        return DOTween.Sequence()
            .Append(_circleImage.DOFade(1,time/4))
            .Append(_circleImage.DOFade(0, time / 4))
            .Append(_circleImage.DOFade(1, time / 4))
            .Append(_circleImage.DOFade(0, time / 4))
            .PlayAsObservable()
            .AsUnitObservable();
    }

    public void SetAlpha(float alpha)
    {
        _circleImage.SetAlpha(alpha);
    }

}
