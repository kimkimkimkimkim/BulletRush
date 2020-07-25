using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private CharacterManager _characterManager;
    [SerializeField] private Joystick _joystick;
    [SerializeField] private GameObject _enemyPrefab;

    private void Start()
    {
        ConnectCharaAndJoystick();

        GameStart();
    }

    private void ConnectCharaAndJoystick()
    {
        _characterManager.joystick = _joystick;
    }

    private void GameStart()
    {
        CreateEnemy();
    }

    private void CreateEnemy() {
        var y = 1.2f;
        var enemy = (GameObject)Instantiate(_enemyPrefab);
        enemy.transform.position = new Vector3(0, y, 2.66f);

        var enemyManager = enemy.GetComponent<EnemyManager>();
        enemyManager.SetNum(3);
        enemyManager.Move(new Vector3(1, 0, 1));
    }
}
