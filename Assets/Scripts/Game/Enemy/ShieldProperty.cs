using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class ShieldProperty : MonoBehaviour
{
    private EnemyManager parentEnemyManager;
    private float period = 100; // 円運動周期
    private float radius = 0.5f;
    private float time = 0.0f;

    public void Init(EnemyManager enemyManager) {
        parentEnemyManager = enemyManager;

        Observable.EveryUpdate()
            .Do(_ => Rotate())
            .Subscribe()
            .AddTo(this);
    }

    private void Rotate()
    {
        // 回転のクォータニオン作成
        time += Time.deltaTime;
        var angle = (360 / period) * time;

        // 円運動の位置計算
        var center = parentEnemyManager.gameObject.transform.position;
        var x = radius * Mathf.Cos(angle);
        var z = radius * Mathf.Sin(angle);
        transform.position = new Vector3(center.x + x, center.y, center.z + z);

        // 向き更新
        transform.LookAt(parentEnemyManager.gameObject.transform, Vector3.up);
    }
}
