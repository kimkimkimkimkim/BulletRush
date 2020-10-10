using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BulletRush.MasterRecord
{
    [CreateAssetMenu]
    public class EnemySpawnDataMB : ScriptableObject
    {
        public List<EnemySpawnData> stageList;
    }

    [Serializable]
    public class EnemySpawnData
    {
        public List<EnemyData> enemyList;
    }
}
