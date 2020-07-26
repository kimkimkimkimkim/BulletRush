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
    [SerializeField] private List<GameObject> _enemyPrefabList;

    [HideInInspector] public int score;
    [HideInInspector] public int stageClearScore;

    private List<IDisposable> observableList = new List<IDisposable>();
    private List<EnemyData> enemyDataList = new List<EnemyData>()
    {
        new EnemyData(){time = 0,num = 3,position = new Vector3(1,0,3),direction = new Vector3(-1,0,-1),enemySize = EnemySize.Small},
        new EnemyData(){time = 3,num = 4,position = new Vector3(-1,0,-3),direction = new Vector3(1,0,1),enemySize = EnemySize.Medium},
        new EnemyData(){time = 6,num = 5,position = new Vector3(1,0,-3),direction = new Vector3(-1,0,1),enemySize = EnemySize.Medium},
        new EnemyData(){time = 9,num = 6,position = new Vector3(-1,0,3),direction = new Vector3(1,0,-1),enemySize = EnemySize.Large},
        new EnemyData(){time = 12,num = 10,position = new Vector3(1,0,3),direction = new Vector3(-1,0,1),enemySize = EnemySize.Medium},
        new EnemyData(){time = 12,num = 10,position = new Vector3(-1,0,3),direction = new Vector3(1,0,-1),enemySize = EnemySize.Medium},
        new EnemyData(){time = 12,num = 10,position = new Vector3(0,0,-3),direction = new Vector3(0.2f,0,-1),enemySize = EnemySize.Medium},
        new EnemyData(){time = 20,num = 20,position = new Vector3(0,0,4),direction = new Vector3(0.2f,0,-1.5f),enemySize = EnemySize.Large},
    };

    private void Start()
    {
        ConnectCharaAndJoystick();
    }

    private void ConnectCharaAndJoystick()
    {
        _characterManager.joystick = _joystick;
        _characterManager.gameManager = this;
    }

    public void GameStart()
    {
        stageClearScore = GetStageClearScore(enemyDataList);
        score = 0;
        CreateEnemy(enemyDataList);

        GameWindowFactory.Create(new GameWindowRequest())
            .Subscribe();
    }

    private int GetStageClearScore(List<EnemyData> enemyDataList)
    {
        var score = 0;
        enemyDataList.ForEach(enemyData =>
        {
            score +=  GetTotalNum(enemyData.num,enemyData.enemySize);
        });
        return score;
    }

    private int GetTotalNum(int num,EnemySize size)
    {

        switch (size)
        {
            case EnemySize.Small:
                return num;
            case EnemySize.Medium:
            case EnemySize.Large:
                return num + 2 * GetTotalNum(GetSplitNum(num),GetOneLankLowerSize(size));
            default:
                return 0;
        }
    }

    private void CreateEnemy(List<EnemyData> enemyDataList)
    {
        enemyDataList.ForEach(enemyData =>
        {
            var observable = Observable.Timer(TimeSpan.FromSeconds(enemyData.time))
                .Do(_ => CreateEnemy(enemyData))
                .Subscribe();
            observableList.Add(observable);
        });
    }

    private void CreateEnemy(EnemyData enemyData)
    {
        var y = 1.4f;
        var enemy = (GameObject)Instantiate(_enemyPrefabList[(int)enemyData.enemySize-1]);
        enemy.transform.position = new Vector3(enemyData.position.x, y, enemyData.position.z);

        var enemyManager = enemy.GetComponent<EnemyManager>();
        enemyManager.enemyData = enemyData;
        enemyManager.SetNum(enemyData.num);
        enemyManager.Move(enemyData.direction);
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

    public void AddScore(int damage)
    {
        score += damage;
        if (score >= stageClearScore) Clear();
    }

    public void KillTheEnemy(EnemyData enemyData)
    {
        if(enemyData.enemySize != EnemySize.Small)
        {
            Split(enemyData);
        }
    }

    private void Split(EnemyData enemyData)
    {
        var direction = enemyData.direction;
        var direction1 = Quaternion.Euler(0, 45, 0) * direction;
        var direction2 = Quaternion.Euler(0, -45, 0) * direction;
        var num = GetSplitNum(enemyData.num);
        var size = GetOneLankLowerSize(enemyData.enemySize);

        var enemy1 = new EnemyData() { num = num, position = enemyData.position, direction = direction1, enemySize = size };
        var enemy2 = new EnemyData() { num = num, position = enemyData.position, direction = direction2, enemySize = size };
        CreateEnemy(enemy1);
        CreateEnemy(enemy2);
    }

    private int GetSplitNum(int num)
    {
        return (int)Math.Ceiling((double)num / 2);
    }

    private EnemySize GetOneLankLowerSize(EnemySize size)
    {
        switch (size)
        {
            case EnemySize.Medium:
                return EnemySize.Small;
            case EnemySize.Large:
                return EnemySize.Medium;
            default:
                return EnemySize.None;
                
        }
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