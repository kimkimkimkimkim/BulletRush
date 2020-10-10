using System;
using System.Collections.Generic;
using System.Linq;
using BulletRush.MasterRecord;
using UnityEngine;

public static class MasterRecords
{
    public static List<RateMB.Param> GetRateMB()
    {
        return Resources.Load<RateMB>("MasterRecords/RateMB").param;
    }

    public static List<DamageMB.Param> GetDamageMB()
    {
        return Resources.Load<DamageMB>("MasterRecords/DamageMB").param;
    }

    public static List<CoinMB.Param> GetCoinMB()
    {
        return Resources.Load<CoinMB>("MasterRecords/CoinMB").param;
    }

    public static List<OfflineRewardMB.Param> GetOfflineRewardMB()
    {
        return Resources.Load<OfflineRewardMB>("MasterRecords/OfflineRewardMB").param;
    }

    public static List<StageMB.Param> GetStageMB() { 
        return Resources.Load<StageMB>("MasterRecords/StageMB").param;
    }

    public static List<EnemyData> GetEnemySpawnDataList(int stageId) {
        var stageList = Resources.Load<StageMB>("MasterRecords/StageMB").param;
        var stage = stageList.FirstOrDefault(m => m.Id == stageId);
        if (stage == null) return new List<EnemyData>();

        var enemySpawnDataList = Resources.Load<EnemySpawnDataMB>("MasterRecords/EnemySpawnDataMB").stageList;
        var enemySpawnData = enemySpawnDataList[stage.EnemySpawnDataSheetIndex];
        return enemySpawnData.enemyList;
    }


}
