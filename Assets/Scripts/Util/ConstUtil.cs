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

    // オフライン報酬を受け取ることができる最小の経過時間(秒)
    public static double MIN_ELAPSED_TIME = 60;

    // 最大のオフライン報酬を受け取ることが出来る経過時間(秒)
    public static double MAX_ELAPSED_TIME = 43200;
}
