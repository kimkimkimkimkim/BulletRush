using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class DebugDialogUIScript : DialogBase
{
    [SerializeField] protected Button _closeButton;

    public override void Init(Dictionary<string, object> param)
    {
        var onClickClose = (Action)param["onClickClose"];

        _closeButton.OnClickIntentAsObservable(ButtonExtensions.ButtonClickIntent.OnlyOneTap)
            .SelectMany(_ => UIManager.Instance.CloseDialogObservable(gameObject))
            .Do(_ =>
            {
                if (onClickClose != null)
                {
                    onClickClose();
                    onClickClose = null;
                }
            })
            .Subscribe();

    }
}
