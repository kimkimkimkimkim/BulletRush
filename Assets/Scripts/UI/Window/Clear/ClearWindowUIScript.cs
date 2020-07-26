using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;

public class ClearWindowUIScript : WindowBase
{
    [SerializeField] private Button _bonusButton;
    [SerializeField] private Button _nextButton;

    private Action onClickBonus;
    private Action onClickNext;

    public override void Init(Dictionary<string, object> param)
    {
        base.Init(param);

        onClickBonus = (Action)param["onClickBonus"];
        onClickNext = (Action)param["onClickNext"];

        _bonusButton.OnClickAsObservable()
            .SelectMany(_ => UIManager.Instance.CloseWindowObservable(gameObject))
            .Do(_ =>
            {
                if (onClickBonus != null)
                {
                    onClickBonus();
                    onClickBonus = null;
                }
            })
            .Subscribe();

        _nextButton.OnClickAsObservable()
            .SelectMany(_ => UIManager.Instance.CloseWindowObservable(gameObject))
            .Do(_ =>
            {
                if (onClickNext != null)
                {
                    onClickNext();
                    onClickNext = null;
                }
            })
            .Subscribe();
    }
}
