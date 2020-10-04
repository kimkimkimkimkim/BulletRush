using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UnityEngine.SceneManagement;
using System;

public class CommonDialogUIScript : DialogBase
{
    [SerializeField] protected Text _titleText;
    [SerializeField] protected Text _contentText;
    [SerializeField] protected Button _yesButton;
    [SerializeField] protected Button _noButton;

    public override void Init(Dictionary<string, object> param)
    {
        var onClickYes = (Action)param["onClickYes"];
        var onClickNo = (Action)param["onClickNo"];
        var title = (string)param["title"];
        var content = (string)param["content"];

        _yesButton.OnClickIntentAsObservable(ButtonExtensions.ButtonClickIntent.OnlyOneTap)
            .SelectMany(_ => UIManager.Instance.CloseDialogObservable(gameObject))
            .Do(_ =>
            {
                if (onClickYes != null)
                {
                    onClickYes();
                    onClickYes = null;
                }
            })
            .Subscribe();

        _noButton.OnClickIntentAsObservable(ButtonExtensions.ButtonClickIntent.OnlyOneTap)
            .SelectMany(_ => UIManager.Instance.CloseDialogObservable(gameObject))
            .Do(_ =>
            {
                if (onClickNo != null)
                {
                    onClickNo();
                    onClickNo = null;
                }
            })
            .Subscribe();

        _titleText.text = title;
        _contentText.text = content;
    }
}
