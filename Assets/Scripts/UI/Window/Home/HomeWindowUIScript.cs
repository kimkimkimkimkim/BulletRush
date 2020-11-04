using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;
using UniRx.Triggers;
using TMPro;
using System.Linq;
using DG.Tweening;

public class HomeWindowUIScript : WindowBase
{
    [SerializeField] private TextMeshProUGUI _stageText;
    [SerializeField] private TextMeshProUGUI _coinText;
    [SerializeField] private TextMeshProUGUI _gemText;
    [SerializeField] private GameObject _dragIcon;
    [SerializeField] private Button _debugButton;
    [SerializeField] private Button _rateButton;
    [SerializeField] private Button _damageButton;
    [SerializeField] private Button _coinButton;
    [SerializeField] private Button _offlineRewardButton;
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private Text _valueText;
    [SerializeField] private GameObject _valueTextEffectArea;
    [SerializeField] private Button _upgradeButton;
    [SerializeField] private List<Sprite> _buttonSpriteList;
    [SerializeField] private TextMeshProUGUI _upgradeCostText;
    [SerializeField] private GameObject _upgradeButtonGrayOutPanel;

    private TabType currentTabType = TabType.Rate;
    private IDisposable coinTextObservable;
    private IDisposable gemTextObservable;

    public override void Init(Dictionary<string, object> param)
    {
        base.Init(param);

        var joystick = (Joystick)param["joystick"];
        joystick.homeWindow = gameObject;

        var sTime = Time.time;
        _dragIcon.UpdateAsObservable()
            .Do(_ => {
                var time = 3 * (Time.time - sTime);
                var x = 100 * Mathf.Cos(time);
                var y = 50 * Mathf.Sin(2*time);
                _dragIcon.transform.localPosition = new Vector3(x, y, 0);
            })
            .Subscribe();

        _debugButton.OnClickIntentAsObservable()
            .SelectMany(_ => DebugDialogFactory.Create(new DebugDialogRequest()))
            .Subscribe();

        _rateButton.OnClickIntentAsObservable()
            .Do(_ => OnClickTabAction(TabType.Rate))
            .Subscribe();

        _damageButton.OnClickIntentAsObservable()
            .Do(_ => OnClickTabAction(TabType.Damage))
            .Subscribe();

        _coinButton.OnClickIntentAsObservable()
            .Do(_ => OnClickTabAction(TabType.Coin))
            .Subscribe();

        _offlineRewardButton.OnClickIntentAsObservable()
            .Do(_ => OnClickTabAction(TabType.OfflineReward))
            .Subscribe();

        _upgradeButton.OnClickIntentAsObservable()
            .Do(_ => OnClickUpgradeButtonAction())
            .Subscribe();

        SetStageInfo();
        SetPropertyInfo();
        SetStatusInfo();
    }

    private void OnClickTabAction(TabType tabType)
    {
        currentTabType = tabType;
        SetStatusInfo();
    }

    private void OnClickUpgradeButtonAction()
    {
        int possessedCoin = SaveDataUtil.Property.GetCoin();
        int level;
        int nextLevelCost;

        switch (currentTabType)
        {
            case TabType.Rate:
                level = SaveDataUtil.Status.GetRateLevel();
                nextLevelCost = MasterRecords.GetLevelUpCoin(level + 1);
                if (possessedCoin < nextLevelCost) return;
                SaveDataUtil.Status.SetRateLevel(level + 1);
                break;
            case TabType.Damage:
                level = SaveDataUtil.Status.GetDamageLevel();
                nextLevelCost = MasterRecords.GetLevelUpCoin(level + 1);
                if (possessedCoin < nextLevelCost) return;
                SaveDataUtil.Status.SetDamageLevel(level + 1);
                break;
            case TabType.Coin:
                level = SaveDataUtil.Status.GetCoinLevel();
                nextLevelCost = MasterRecords.GetLevelUpCoin(level + 1);
                if (possessedCoin < nextLevelCost) return;
                SaveDataUtil.Status.SetCoinLevel(level + 1);
                break;
            case TabType.OfflineReward:
                level = SaveDataUtil.Status.GetOfflineRewardLevel();
                nextLevelCost = MasterRecords.GetLevelUpCoin(level + 1);
                if (possessedCoin < nextLevelCost) return;
                SaveDataUtil.Status.SetOfflineRewardLevel(level + 1);
                break;
            default:
                nextLevelCost = 0;
                break;
        }

        SaveDataUtil.Property.SetCoin(possessedCoin - nextLevelCost);
        SetPropertyInfo();
        SetStatusInfo();

        // アニメーション
        var effectBase = EffectManager.Instance.CreateEffectBase(_valueTextEffectArea.transform,EffectManager.EffectType.Twinkle);
        effectBase.Play();
    }

    private void SetStageInfo() {
        var nextStageId = SaveDataUtil.Status.GetNextStageId();
        _stageText.text = "STAGE " + nextStageId;
    }

    private void SetPropertyInfo()
    {
        if (coinTextObservable != null) coinTextObservable.Dispose();
        if (gemTextObservable != null) gemTextObservable.Dispose();

        const float ANIMATION_TIME = 0.5f;

        var coin = int.Parse(_coinText.text);
        var gem = int.Parse(_gemText.text);
        coinTextObservable = DOTween.To(() => coin, x => _coinText.text = x.ToString(), SaveDataUtil.Property.GetCoin(), ANIMATION_TIME)
            .OnCompleteAsObservable()
            .Subscribe();
        gemTextObservable = DOTween.To(() => gem,x => _gemText.text = x.ToString(),SaveDataUtil.Property.GetGem(),ANIMATION_TIME)
            .OnCompleteAsObservable()
            .Subscribe();
    }

    private void SetStatusInfo() {
        int level;
        string titleText;
        string valueText;
        int upgradeCost;

        switch (currentTabType)
        {
            case TabType.Rate:
                level = SaveDataUtil.Status.GetRateLevel();
                var rate = MasterRecords.GetRateStatus(level);
                titleText = "RATE";
                valueText = rate + "/s";
                break;
            case TabType.Damage:
                level = SaveDataUtil.Status.GetDamageLevel();
                var damage = MasterRecords.GetDamageStatus(level);
                titleText = "DAMAGE";
                valueText = damage.ToString();
                break;
            case TabType.Coin:
                level = SaveDataUtil.Status.GetCoinLevel();
                var coin = MasterRecords.GetCoinBonusStatus(level) * 100;
                titleText = "COIN";
                valueText = coin + "%";
                break;
            case TabType.OfflineReward:
                level = SaveDataUtil.Status.GetOfflineRewardLevel();
                var offlineReward = MasterRecords.GetOfflineBonusStatus(level) * 100;
                titleText = "OFFLINE";
                valueText = offlineReward + "%";
                break;
            default:
                titleText = "";
                valueText = "";
                level = 1000;
                break;
        }
        upgradeCost = MasterRecords.GetLevelUpCoin(level + 1);

        // お金足りるかチェック
        var isEnough = upgradeCost <= SaveDataUtil.Property.GetCoin();
        // 最大レベルチェック
        var isMaxLevel = level == ConstUtil.MAX_STATUS_LEVEL;

        _titleText.text = titleText;
        _valueText.text = valueText;
        _upgradeCostText.text = isMaxLevel ? "MAX" : StatusUtil.GetCostText(upgradeCost);
        _upgradeButton.GetComponent<Image>().sprite = _buttonSpriteList[(int)currentTabType - 1];
        _upgradeButtonGrayOutPanel.SetActive(!isEnough || isMaxLevel);
    }

    private enum TabType
    {
        None = 0,
        Rate,
        Damage,
        Coin,
        OfflineReward,
    }
}
