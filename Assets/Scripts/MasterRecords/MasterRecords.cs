﻿using System;
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
        var simpleEnemyDataList = enemySpawnData.enemyList;

        // simpleからの変換
        var enemyDataList = simpleEnemyDataList.Select(s =>
        {
            return new EnemyData()
            {
                time = s.time,
                health = s.health,
                position = EnemyUtil.GetPosition(s.position),
                direction = EnemyUtil.GetDirection(s.direction),
                enemySize = s.enemySize,
                enemyType = s.enemyType,
            };
        }).ToList();

        // 角度乱数発生
        enemyDataList = enemyDataList.Select(e =>
        {
            var randomAngle = UnityEngine.Random.Range(-10.0f, 10.0f);
            return new EnemyData()
            {
                time = e.time,
                health = e.health,
                position = e.position,
                direction = Quaternion.Euler(0, randomAngle, 0) * e.direction,
                enemySize = e.enemySize,
                enemyType = e.enemyType,
            };
        }).ToList();

        return enemyDataList;
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
