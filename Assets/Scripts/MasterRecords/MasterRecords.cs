using System;
using System.Collections.Generic;
using System.Linq;
using BulletRush.MasterRecord;
using UnityEngine;

public static class MasterRecords
{
    public static List<EnemyData> GetEnemySpawnDataList(int stageId) {
        if (stageId > ConstUtil.MAX_STAGE_COUNT || stageId < 0) return new List<EnemyData>();

        var stageData = StageCreator.GetStageData();
        return stageData[stageId - 1];
    }

    // nextLevelレベルにレベルアップするために必要なコイン量を取得
    public static int GetLevelUpCoin(int nextLevel)
    {
        return (int)Math.Pow(nextLevel - 1,2) * 10;
    }

    // stageIdステージをクリアした際に取得するコイン量を取得
    public static int GetStageClearRewardCoin(int stageId)
    {
        return (int)(GetLevelUpCoin(stageId + 1));
    }

    // コイン倍率ステータスを取得
    public static float GetCoinBonusStatus(int level) {
        return ((float)(19 * level) + 980)/ 999;
    }

    // オフライン報酬倍率ステータスを取得
    public static float GetOfflineBonusStatus(int level)
    {
        return ((float)(19 * level) + 980) / 999;
    }

    // ダメージステータスを取得
    public static float GetDamageStatus(int level)
    {
        return ((float)(1999 * level) - 1000) / 999;
    }

    // レートステータスを取得
    public static float GetRateStatus(int level) {
        return (999995 - (float)(995 * level)) / 999000;
    }

    // 想定DPSを取得
    public static float GetAssumptionDPS(int stageId)
    {
        return GetDamageStatus(stageId) / GetRateStatus(stageId);
    }
}
