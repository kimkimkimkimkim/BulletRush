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

        switch (currentTabType)
        {
            case TabType.Rate:
                level = SaveDataUtil.Status.GetRateLevel();
                var rate = MasterRecords.GetRateMB().First(m => m.Level == level);
                if (possessedCoin < rate.NextLevelCost) return;
                SaveDataUtil.Property.SetCoin(possessedCoin - rate.NextLevelCost);
                SaveDataUtil.Status.SetRateLevel(level + 1);
                break;
            case TabType.Damage:
                level = SaveDataUtil.Status.GetDamageLevel();
                var damage = MasterRecords.GetDamageMB().First(m => m.Level == level);
                if (possessedCoin < damage.NextLevelCost) return;
                SaveDataUtil.Property.SetCoin(possessedCoin - damage.NextLevelCost);
                SaveDataUtil.Status.SetDamageLevel(level + 1);
                break;
            case TabType.Coin:
                level = SaveDataUtil.Status.GetCoinLevel();
                var coin = MasterRecords.GetCoinMB().First(m => m.Level == level);
                if (possessedCoin < coin.NextLevelCost) return;
                SaveDataUtil.Property.SetCoin(possessedCoin - coin.NextLevelCost);
                SaveDataUtil.Status.SetCoinLevel(level + 1);
                break;
            case TabType.OfflineReward:
                level = SaveDataUtil.Status.GetOfflineRewardLevel();
                var offlineReward = MasterRecords.GetOfflineRewardMB().First(m => m.Level == level);
                if (possessedCoin < offlineReward.NextLevelCost) return;
                SaveDataUtil.Property.SetCoin(possessedCoin - offlineReward.NextLevelCost);
                SaveDataUtil.Status.SetOfflineRewardLevel(level + 1);
                break;
            default:
                break;
        }

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
                var rate = MasterRecords.GetRateMB().First(m => m.Level == level);
                titleText = "RATE";
                valueText = rate.Value + "/s";
                upgradeCost = rate.NextLevelCost;
                break;
            case TabType.Damage:
                level = SaveDataUtil.Status.GetDamageLevel();
                var damage = MasterRecords.GetDamageMB().First(m => m.Level == level);
                titleText = "DAMAGE";
                valueText = damage.Value.ToString();
                upgradeCost = damage.NextLevelCost;
                break;
            case TabType.Coin:
                level = SaveDataUtil.Status.GetCoinLevel();
                var coin = MasterRecords.GetCoinMB().First(m => m.Level == level);
                titleText = "COIN";
                valueText = "x " + coin.Value;
                upgradeCost = coin.NextLevelCost;
                break;
            case TabType.OfflineReward:
                level = SaveDataUtil.Status.GetOfflineRewardLevel();
                var offlineReward = MasterRecords.GetOfflineRewardMB().First(m => m.Level == level);
                titleText = "OFFLINE";
                valueText = offlineReward.Value + "/m";
                upgradeCost = offlineReward.NextLevelCost;
                break;
            default:
                titleText = "";
                valueText = "";
                upgradeCost = 0;
                break;
        }

        _titleText.text = titleText;
        _valueText.text = valueText;
        _upgradeCostText.text = StatusUtil.GetCostText(upgradeCost);
        _upgradeButton.GetComponent<Image>().sprite = _buttonSpriteList[(int)currentTabType - 1];

        // お金足りるかチェック
        var isEnough = upgradeCost <= SaveDataUtil.Property.GetCoin();
        _upgradeButtonGrayOutPanel.SetActive(!isEnough);
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
