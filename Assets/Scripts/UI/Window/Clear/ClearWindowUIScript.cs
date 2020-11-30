using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;
using DG.Tweening;
using TMPro;

public class ClearWindowUIScript : WindowBase
{
    [SerializeField] private Text _possessionCoinText;
    [SerializeField] private Text _possessionGemText;
    [SerializeField] private GameObject _possessionCoinPanel;
    [SerializeField] private GameObject _possessionGemPanel;
    [SerializeField] private Image _titleImage;
    [SerializeField] private Text _rewardCoinText;
    [SerializeField] private Text _rewardGemText;
    [SerializeField] private GameObject _rewardCoinPanel;
    [SerializeField] private GameObject _rewardGemPanel;
    [SerializeField] private Image _chestImage;
    [SerializeField] private GameObject _chestBackGroundImage;
    [SerializeField] private Button _bonusButton;
    [SerializeField] private Button _nextButton;
    [SerializeField] private GameObject _bonusButtonBase;
    [SerializeField] private GameObject _nextButtonBase;

    private ClearResultData clearResultData;
    private int possessionCoin;
    private int possessionGem;

    public override void Init(Dictionary<string, object> param)
    {
        base.Init(param);

        var onClickBonus = (Action)param["onClickBonus"];
        var onClickNext = (Action)param["onClickNext"];
        clearResultData = (ClearResultData)param["clearResultData"];
        possessionCoin = SaveDataUtil.Property.GetCoin();
        possessionGem = SaveDataUtil.Property.GetGem();

        _possessionCoinText.text = possessionCoin.ToString();
        _possessionGemText.text = possessionGem.ToString();

        _bonusButton.OnClickAsObservable()
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
            .Do(_ =>
            {
                if (onClickNext != null)
                {
                    onClickNext();
                    onClickNext = null;
                }
            })
            .Subscribe();

        PlayAnimation();
    }

    private void PlayAnimation() {
        _titleImage.transform.localScale = Vector3.zero;
        _titleImage.transform
            .DOScale(Vector3.one, 0.5f)
            .SetEase(Ease.OutBounce)
            .SetUpdate(true);

        Observable.ReturnUnit()
            .Delay(TimeSpan.FromSeconds(0.25f), Scheduler.MainThreadIgnoreTimeScale)
            .SelectMany(_ =>
            {
                var coin = 0;
                var gem = 0;
                return DOTween.Sequence()
                    .SetUpdate(true)
                    .Append(DOTween.To(() => coin, (x) => coin = x, clearResultData.rewardCoin, 1).OnUpdate(() => {
                        _rewardCoinText.text = "+ " + TextUtil.GetFormattedValue(coin);
                    }))
                    .PlayAsObservable();
            })
            .Do(_ => PlayShowButtonAnimation(_bonusButtonBase.transform))
            .SelectMany(_ => Observable.ReturnUnit().Delay(TimeSpan.FromSeconds(5f), Scheduler.MainThreadIgnoreTimeScale))
            .Do(_ => PlayShowButtonAnimation(_nextButtonBase.transform))
            .Subscribe();
    }

    private void PlayShowButtonAnimation(Transform buttonTransform)
    {
        buttonTransform.gameObject.SetActive(true);
        buttonTransform.localScale = Vector3.zero;
        buttonTransform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack).SetUpdate(true);
    }
}
