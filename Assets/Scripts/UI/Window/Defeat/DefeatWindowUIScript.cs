using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;
using DG.Tweening;

public class DefeatWindowUIScript : WindowBase
{
    [SerializeField] private GameObject _countDownBase;
    [SerializeField] private Image _countdownImage;
    [SerializeField] private List<Sprite> _countdownSpriteList;
    [SerializeField] private Button _continueButton;
    [SerializeField] private Button _noThanksButton;

    private const float COUNTDOWN_TIME = 5f; // カウントダウン時間
    private const int MAX_COUNTDOWN_NUM = 3; // カウントダウンの最大値(3->2->1)

    private Action onClickContinue;
    private Action onClickNoThanks;
    private IDisposable countdownAnimationObservable;

    public override void Init(Dictionary<string,object> param)
    {
        base.Init(param);

        onClickContinue = (Action)param["onClickContinue"];
        onClickNoThanks = (Action)param["onClickNoThanks"];

        _continueButton.OnClickAsObservable()
            .Do(_ => Dispose())
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
            .Do(_ => Dispose())
            .Do(_ =>
            {
                if (onClickNoThanks != null)
                {
                    onClickNoThanks();
                    onClickNoThanks = null;
                }
            })
            .Subscribe();

        PlayAnimation();
    }

    private void PlayAnimation()
    {
        countdownAnimationObservable = Observable.ReturnUnit()
            .Do(_ =>
            {
                _countDownBase.SetActive(true);
                _continueButton.gameObject.SetActive(true);
                _noThanksButton.gameObject.SetActive(false);
            })
            .SelectMany(_ => PlayCountdownAnimation(3))
            .SelectMany(_ => PlayCountdownAnimation(2))
            .SelectMany(_ => PlayCountdownAnimation(1))
            .Do(_ =>
            {
                _countDownBase.SetActive(false);
                _continueButton.gameObject.SetActive(false);
                _noThanksButton.gameObject.SetActive(true);
            })
            .Subscribe()
            .AddTo(this);
    }

    private IObservable<Unit> PlayCountdownAnimation(int num) {
        var initialScale = 0.8f;

        if (num <= 0) return Observable.ReturnUnit();

        return DOTween.Sequence()
            .SetUpdate(true)
            .OnStart(() =>
            {
                _countdownImage.sprite = _countdownSpriteList[num - 1];
                _countdownImage.transform.localScale = Vector3.one * initialScale;
            })
            .Append(_countdownImage.transform.DOScale(Vector3.one, COUNTDOWN_TIME / MAX_COUNTDOWN_NUM).SetUpdate(true))
            .PlayAsObservable()
            .AsUnitObservable();
    }

    private void Dispose() {
        if (countdownAnimationObservable != null) countdownAnimationObservable.Dispose();
    }
}
