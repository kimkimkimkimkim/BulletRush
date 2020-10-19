using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemyData
{
    public float time;
    public float health;
    public Vector3 position;
    public Vector3 direction;
    public EnemySize enemySize;
    public EnemyType enemyType;
}

[Serializable]
public enum EnemySize
{
    None,
    Small,
    Medium,
    Large,
}

[Serializable]
public enum EnemyType
{
    Normal,
    Guard,
    Fast,
    Slow,
    Random
}
