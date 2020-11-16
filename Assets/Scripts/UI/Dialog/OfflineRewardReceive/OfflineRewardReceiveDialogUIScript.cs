using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UnityEngine.SceneManagement;
using System;

public class OfflineRewardReceiveDialogUIScript : DialogBase
{
    [SerializeField] protected Text _contentText;
    [SerializeField] protected Button _receiveButton;
    [SerializeField] protected Button _bonusButton;
    [SerializeField] protected GameObject _grayoutPanel;

    public override void Init(Dictionary<string, object> param)
    {
        var onClickReceive = (Action)param["onClickReceive"];
        var onClickBonus = (Action)param["onClickBonus"];
        var content = (string)param["content"];

        _receiveButton.OnClickIntentAsObservable(ButtonExtensions.ButtonClickIntent.OnlyOneTap)
            .SelectMany(_ => UIManager.Instance.CloseDialogObservable(gameObject))
            .Do(_ =>
            {
                if (onClickReceive != null)
                {
                    onClickReceive();
                    onClickReceive = null;
                }
            })
            .Subscribe();

        _bonusButton.OnClickIntentAsObservable(ButtonExtensions.ButtonClickIntent.OnlyOneTap)
            .Do(_ =>
            {
                if(!MobileAdsManager.Instance.TryShowRewarded(() =>
                {
                    // 正常に動画を視聴できた
                    UIManager.Instance.CloseDialogObservable(gameObject).Subscribe();
                    if (onClickBonus != null)
                    {
                        onClickBonus();
                        onClickBonus = null;
                    }
                }))
                {
                    // 動画を再生できなかった
                    UIManager.Instance.CloseDialogObservable(gameObject).Subscribe();
                    if (onClickReceive != null)
                    {
                        onClickReceive();
                        onClickReceive = null;
                    }
                }


            })
            .Subscribe();

        Observable.Interval(TimeSpan.FromSeconds(1))
            .Do(_ =>
            {
                var isLoaded = MobileAdsManager.Instance.IsRewardAdLoaded();
                _grayoutPanel.SetActive(!isLoaded);
            })
            .Subscribe()
            .AddTo(this);

        _contentText.text = content;
    }
}
