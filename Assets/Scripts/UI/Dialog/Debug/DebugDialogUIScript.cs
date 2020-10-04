using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UnityEngine.SceneManagement;

public class DebugDialogUIScript : DialogBase
{
    [SerializeField] protected Button _closeButton;
    [SerializeField] protected Button _restartButton;

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

        _restartButton.OnClickIntentAsObservable()
            .SelectMany(_ => CommonDialogFactory.Create(new CommonDialogRequest()
            {
                title="Confirm",
                content = "プレイヤーデータを全て削除します。\nよろしいですか？"
            }))
            .Where(res => res.responseType == DialogUtil.DialogResponseType.Yes)
            .Do(_ =>
            {
                SaveData.Clear();
                SaveData.Save();
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            })
            .Subscribe();

    }
}
