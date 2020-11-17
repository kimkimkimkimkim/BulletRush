using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class OfflineRewardManager : SingletonMonoBehaviour<OfflineRewardManager>
{
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        var lastQuitTime = SaveDataUtil.Status.GetLastQuitTime();
        var now = DateTime.Now;
        TimeSpan timeSpan = now - lastQuitTime;
        double elapsedTimeSeconds = timeSpan.TotalSeconds;

        Debug.Log("last : " + lastQuitTime + " now : " + now + " 経過時間(秒) : " + elapsedTimeSeconds);
        if(lastQuitTime != DateTimeUtil.EPOCH && elapsedTimeSeconds >= 60)
        {
            var offlineRewardStatusLevel = SaveDataUtil.Status.GetOfflineRewardLevel();
            var offlineRewardStatus = MasterRecords.GetOfflineBonusStatus(offlineRewardStatusLevel);
            var reward = (int)(MasterRecords.GetOfflineReward(elapsedTimeSeconds) * offlineRewardStatus);

            OfflineRewardReceiveDialogFactory.Create(new OfflineRewardReceiveDialogRequest() { content = reward.ToString() })
                .Do(res =>
                {
                    if (res.isBonus) reward *= 2;
                    SaveDataUtil.Property.AddCoin(reward);

                    var currentWindow = UIManager.Instance.GetNowWindow();
                    if (currentWindow.GetComponent<HomeWindowUIScript>())
                    {
                        currentWindow.GetComponent<HomeWindowUIScript>().SetPropertyInfo();
                    }
                })
                .Subscribe();
        }
    }

    private void OnApplicationQuit()
    {
        SaveDataUtil.Status.SetLastQuitTime();
    }
}
