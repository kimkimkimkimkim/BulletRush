using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EffectManager : SingletonMonoBehaviour<EffectManager>
{
    [SerializeField] private GameObject _effectBasePrefab;
    [SerializeField] private GameObject _twinkleIconPrefab;

    public EffectBase CreateEffectBase(Transform parent,EffectType type)
    {
        var effectBasePrefab = (GameObject)Instantiate(_effectBasePrefab);
        var effectBase = effectBasePrefab.GetComponent<EffectBase>();
        effectBasePrefab.transform.SetParent(parent);
        effectBasePrefab.transform.localScale = Vector3.one;
        effectBasePrefab.transform.localPosition = Vector3.zero;

        switch (type)
        {
            case EffectType.Twinkle:
                effectBase.SetEffectSequence(TwinkleEffectObservable(effectBasePrefab.transform));
                break;
        }

        return effectBase;
    }

    #region Twinkle
    private Sequence TwinkleEffectObservable(Transform parent) {
        const float ANIMATION_TIME = 1f;
        const int TWINKLE_NUM = 10;
        Sequence[] sequenceArray = new Sequence[TWINKLE_NUM];

        sequenceArray = sequenceArray.Select((s,i) =>
        {
            var twinkleIcon = (GameObject)Instantiate(_twinkleIconPrefab);
            twinkleIcon.transform.SetParent(parent);
            twinkleIcon.transform.localPosition = GetRandomPosition(0f,10f);
            twinkleIcon.transform.localScale = Vector3.one * 0.5f;

            var scaleSequence = DOTween.Sequence()
                .Append(twinkleIcon.transform.DOScale(Vector3.one * 1.5f, ANIMATION_TIME / 2))
                .Append(twinkleIcon.transform.DOScale(Vector3.one * 0.5f, ANIMATION_TIME / 2));
            var positionSequence = DOTween.Sequence()
                .Append(twinkleIcon.transform.DOLocalMove(GetRandomPosition(50f, 100f), ANIMATION_TIME).SetEase(Ease.OutExpo));
            var alphaSequence = DOTween.Sequence()
                .Append(twinkleIcon.GetComponent<Image>().DOFade(0,ANIMATION_TIME /2).SetDelay(ANIMATION_TIME/2));

            return DOTween.Sequence()
                .Append(scaleSequence)
                .Join(positionSequence)
                .Join(alphaSequence);
        })
        .ToArray();

        return DOTween.Sequence()
            .Append(sequenceArray[0])
            .Join(sequenceArray[1])
            .Join(sequenceArray[2])
            .Join(sequenceArray[3])
            .Join(sequenceArray[4])
            .Join(sequenceArray[5])
            .Join(sequenceArray[6])
            .Join(sequenceArray[7])
            .Join(sequenceArray[8])
            .Join(sequenceArray[9]);
    }

    /// <summary>
    /// 極座標系における線分の長さの最大最小を指定すると、それに応じたランダムな座標を直交座標で返す
    /// </summary>
    private Vector3 GetRandomPosition(float minR,float maxR) {
        var angle = UnityEngine.Random.Range(0f, 360f);
        var length = UnityEngine.Random.Range(minR, maxR);
        var pos = MathUtil.PolarToCartesian(length, angle);
        return new Vector3(pos.x,pos.y,0);
    }

    #endregion

    public enum EffectType
    {
        None,
        Twinkle
    }

}