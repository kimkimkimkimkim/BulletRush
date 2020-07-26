using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;

public class DefeatWindowUIScript : WindowBase
{
    [SerializeField] private Button _continueButton;
    [SerializeField] private Button _noThanksButton;

    private Action onClickContinue;
    private Action onClickNoThanks;

    public override void Init(Dictionary<string,object> param)
    {
        base.Init(param);

        onClickContinue = (Action)param["onClickContinue"];
        onClickNoThanks = (Action)param["onClickNoThanks"];

        _continueButton.OnClickAsObservable()
            .SelectMany(_ => UIManager.Instance.CloseWindowObservable(gameObject))
            .Do(_ =>
            {
                if(onClickContinue != null)
                {
                    onClickContinue();
                    onClickContinue = null;
                }
            })
            .Subscribe();

        _noThanksButton.OnClickAsObservable()
            .SelectMany(_ => UIManager.Instance.CloseWindowObservable(gameObject))
            .Do(_ =>
            {
                if (onClickNoThanks != null)
                {
                    onClickNoThanks();
                    onClickNoThanks = null;
                }
            })
            .Subscribe();
    }
}
