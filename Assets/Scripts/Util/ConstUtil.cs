using System.Collections;
using System.Collections.Generic;
using BulletRush.MasterRecord;
using UnityEngine;

public static class ConstUtil
{
    // 各ステータスの最大レベル
    public static int MAX_STATUS_LEVEL = 1000;

    // 最大ステージ数
    public static int MAX_STAGE_COUNT = 1000;

    // 現在実装中の最大ステージ数
    public static int CURRENT_MAX_STAGE_COUNT = Resources.Load<EnemySpawnDataMB>("MasterRecords/EnemySpawnDataMB").stageList.Count;
}
