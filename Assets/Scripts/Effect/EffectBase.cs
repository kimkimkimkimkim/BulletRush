using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UniRx;
using UnityEngine;

public class EffectBase : MonoBehaviour
{
    private Sequence effectSequence;

    public void SetEffectSequence(Sequence sequence)
    {
        effectSequence = sequence;
    }

    public void Play()
    {
        if (effectSequence == null) return;

        effectSequence
            .PlayAsObservable()
            .DoOnCompleted(() => Destroy(gameObject))
            .Subscribe()
            .AddTo(gameObject);
    }
}
