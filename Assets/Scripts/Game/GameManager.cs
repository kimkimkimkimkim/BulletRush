using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using UnityEngine.SceneManagement;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    [SerializeField] private CharacterManager _characterManager;
    [SerializeField] private Joystick _joystick;
    [SerializeField] private GameObject _enemyPrefab;

    private int killCount = 0;
    private List<IDisposable> observableList = new List<IDisposable>();
    private List<EnemySpawnData> enemySpawnDataList = new List<EnemySpawnData>()
    {
        new EnemySpawnData(){time = 0,num = 3,position = new Vector3(1,0,3),direction = new Vector3(-1,0,-1)},
        new EnemySpawnData(){time = 3,num = 4,position = new Vector3(-1,0,-3),direction = new Vector3(1,0,1)},
        new EnemySpawnData(){time = 6,num = 5,position = new Vector3(1,0,-3),direction = new Vector3(-1,0,1)},
        new EnemySpawnData(){time = 9,num = 6,position = new Vector3(-1,0,3),direction = new Vector3(1,0,-1)},
        new EnemySpawnData(){time = 12,num = 10,position = new Vector3(1,0,3),direction = new Vector3(-1,0,1)},
        new EnemySpawnData(){time = 12,num = 10,position = new Vector3(-1,0,3),direction = new Vector3(1,0,-1)},
        new EnemySpawnData(){time = 12,num = 10,position = new Vector3(0,0,-3),direction = new Vector3(0.2f,0,-1)},
        new EnemySpawnData(){time = 20,num = 20,position = new Vector3(0,0,5),direction = new Vector3(1,0,-1)},
    };

    private void Start()
    {
        ConnectCharaAndJoystick();

        //GameStart();
    }

    private void ConnectCharaAndJoystick()
    {
        _characterManager.joystick = _joystick;
        _characterManager.gameManager = this;
    }

    public void GameStart()
    {
        CreateEnemy(enemySpawnDataList);
    }

    private void CreateEnemy(List<EnemySpawnData> enemySpawnDataList)
    {
        enemySpawnDataList.ForEach(enemySpawnData =>
        {
            var observable = Observable.Timer(TimeSpan.FromSeconds(enemySpawnData.time))
                .Do(_ => CreateEnemy(enemySpawnData))
                .Subscribe();
            observableList.Add(observable);
        });
    }

    private void CreateEnemy(EnemySpawnData enemySpawnData)
    {
        var y = 1.2f;
        var enemy = (GameObject)Instantiate(_enemyPrefab);
        enemy.transform.position = new Vector3(enemySpawnData.position.x, y, enemySpawnData.position.z);

        var enemyManager = enemy.GetComponent<EnemyManager>();
        enemyManager.SetNum(enemySpawnData.num);
        enemyManager.Move(enemySpawnData.direction);
    }

    private void Dispose()
    {
        observableList.ForEach(observable =>
        {
            observable.Dispose();
        });
        observableList.Clear();
    }

    public void Defeat() {
        Time.timeScale = 0;
        UIManager.Instance.SetUI(UIMode.Defeat);

        DefeatWindowFactory.Create(new DefeatWindowRequest())
            .Do(_ => Time.timeScale = 1)
            .Do(res =>
            {
                if (res.isContinue) {
                    UIManager.Instance.SetUI(UIMode.Playing);
                }
                else
                {
                    Dispose();
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                }
            })
            .Subscribe();
    }

    public void KillTheEnemy()
    {
        killCount++;
        if (killCount >= enemySpawnDataList.Count) Clear();
    }

    private void Clear() {
        Time.timeScale = 0;
        UIManager.Instance.SetUI(UIMode.Win);

        ClearWindowFactory.Create(new ClearWindowRequest())
            .Do(_ => Time.timeScale = 1)
            .Do(_ => SceneManager.LoadScene(SceneManager.GetActiveScene().name))
            .Subscribe();
    }

}