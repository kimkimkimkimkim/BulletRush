using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;
using UniRx.Triggers;
using TMPro;
using System.Linq;

public class HomeWindowUIScript : WindowBase
{
    [SerializeField] private GameObject _dragIcon;
    [SerializeField] private Button _rateButton;
    [SerializeField] private Button _damageButton;
    [SerializeField] private Button _coinButton;
    [SerializeField] private Button _offlineRewardButton;
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _valueText;
    [SerializeField] private Button _upgradeButton;
    [SerializeField] private TextMeshProUGUI _upgradeCostText;

    private TabType currentTabType = TabType.Rate;

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

        _rateButton.OnClickAsObservable()
            .Do(_ => OnClickTabAction(TabType.Rate))
            .Subscribe();

        _damageButton.OnClickAsObservable()
            .Do(_ => OnClickTabAction(TabType.Damage))
            .Subscribe();

        _coinButton.OnClickAsObservable()
            .Do(_ => OnClickTabAction(TabType.Coin))
            .Subscribe();

        _offlineRewardButton.OnClickAsObservable()
            .Do(_ => OnClickTabAction(TabType.OfflineReward))
            .Subscribe();

        _upgradeButton.OnClickAsObservable()
            .Do(_ => OnClickUpgradeButtonAction())
            .Subscribe();

        SetStatusInfo();
    }

    private void OnClickTabAction(TabType tabType)
    {
        currentTabType = tabType;
        SetStatusInfo();
    }

    private void OnClickUpgradeButtonAction()
    {
        int level;

        switch (currentTabType)
        {
            case TabType.Rate:
                level = SaveDataUtil.Status.GetRateLevel();
                SaveDataUtil.Status.SetRateLevel(level + 1);
                break;
            case TabType.Damage:
                level = SaveDataUtil.Status.GetDamageLevel();
                SaveDataUtil.Status.SetDamageLevel(level + 1);
                break;
            case TabType.Coin:
                level = SaveDataUtil.Status.GetCoinLevel();
                SaveDataUtil.Status.SetCoinLevel(level + 1);
                break;
            case TabType.OfflineReward:
                level = SaveDataUtil.Status.GetOfflineRewardLevel();
                SaveDataUtil.Status.SetOfflineRewardLevel(level + 1);
                break;
            default:
                break;
        }

        SetStatusInfo();
    }

    private void SetStatusInfo() {
        int level;
        string titleText;
        string valueText;
        string upgradeCostText;

        switch (currentTabType)
        {
            case TabType.Rate:
                level = SaveDataUtil.Status.GetRateLevel();
                var rate = MasterRecords.GetRateMB().First(m => m.Level == level);
                titleText = "RATE";
                valueText = rate.Value + "/s";
                upgradeCostText = rate.NextLevelCost.ToString();
                break;
            case TabType.Damage:
                level = SaveDataUtil.Status.GetDamageLevel();
                var damage = MasterRecords.GetDamageMB().First(m => m.Level == level);
                titleText = "DAMAGE";
                valueText = damage.Value.ToString();
                upgradeCostText = damage.NextLevelCost.ToString();
                break;
            case TabType.Coin:
                level = SaveDataUtil.Status.GetCoinLevel();
                var coin = MasterRecords.GetCoinMB().First(m => m.Level == level);
                titleText = "COIN";
                valueText = "x " + coin.Value;
                upgradeCostText = coin.NextLevelCost.ToString();
                break;
            case TabType.OfflineReward:
                level = SaveDataUtil.Status.GetOfflineRewardLevel();
                var offlineReward = MasterRecords.GetOfflineRewardMB().First(m => m.Level == level);
                titleText = "OFFLINE";
                valueText = offlineReward.Value + "/m";
                upgradeCostText = offlineReward.NextLevelCost.ToString();
                break;
            default:
                titleText = "";
                valueText = "";
                upgradeCostText = "";
                break;
        }

        _titleText.text = titleText;
        _valueText.text = valueText;
        _upgradeCostText.text = upgradeCostText;
    }

    private enum TabType
    {
        Rate,
        Damage,
        Coin,
        OfflineReward,
    }
}
