using System;
using System.Collections.Generic;
using System.Linq;
using BulletRush.MasterRecord;
using UnityEngine;

public static class MasterRecords
{
    public static List<EnemyData> GetEnemySpawnDataList(int stageId) {
        if (stageId > ConstUtil.CURRENT_MAX_STAGE_COUNT || stageId < 0) return new List<EnemyData>();

        var enemySpawnDataList = Resources.Load<EnemySpawnDataMB>("MasterRecords/EnemySpawnDataMB").stageList;
        var enemySpawnData = enemySpawnDataList[stageId - 1];

        // 角度乱数発生
        var randomAngle = UnityEngine.Random.Range(-5.0f, 5.0f);
        enemySpawnData.enemyList.ForEach(e => e.direction = Quaternion.Euler(0,randomAngle,0) * e.direction);

        return enemySpawnData.enemyList;
    }

    // nextLevelレベルにレベルアップするために必要なコイン量を取得
    public static int GetLevelUpCoin(int nextLevel)
    {
        return (int)Math.Pow(nextLevel - 1,2);
    }

    // stageIdステージをクリアした際に取得するコイン量を取得
    public static int GetStageClearRewardCoin(int stageId)
    {
        return (int)(2 * Math.Log10(GetLevelUpCoin(stageId + 1)));
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
