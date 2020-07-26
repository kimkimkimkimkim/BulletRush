using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyData
{
    public float time { get; set; }
    public int num { get; set; }
    public Vector3 position { get; set; }
    public Vector3 direction { get; set; }
    public EnemySize enemySize { get; set; }
}

public enum EnemySize
{
    None,
    Small,
    Medium,
    Large,
}
