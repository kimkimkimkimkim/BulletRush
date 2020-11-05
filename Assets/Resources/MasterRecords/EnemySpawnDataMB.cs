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
        public List<SimpleEnemyData> enemyList;
    }

    [Serializable]
    public class SimpleEnemyData
    {
        public float time;
        public float health;
        public int position; // 1 ~ 9
        public int direction; // 1 ~ 4
        public EnemySize enemySize;
        public EnemyType enemyType;
    }
}
