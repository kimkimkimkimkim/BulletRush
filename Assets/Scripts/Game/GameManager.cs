using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using UnityEngine.SceneManagement;
using System.Linq;
using DG.Tweening;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    [SerializeField] private CharacterManager _characterManager;
    [SerializeField] private Joystick _joystick;
    [SerializeField] private List<GameObject> _enemyPrefabList;
    [SerializeField] private GameObject _spawnCircle;

    [HideInInspector] public float score;
    [HideInInspector] public float stageClearScore;

    private GameWindowUIScript gameWindowUIScript;
    private List<IDisposable> observableList = new List<IDisposable>();

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
        var stageId = SaveDataUtil.Status.GetNextStageId();
        var enemySpawnDataList = MasterRecords.GetEnemySpawnDataList(stageId);

        _characterManager.SetStatus();
        stageClearScore = GetStageClearScore(enemySpawnDataList);
        score = 0;
        CreateEnemy(enemySpawnDataList);

        GameWindowFactory.Create(new GameWindowRequest())
            .Subscribe();
    }

    private int GetStageClearScore(List<EnemyData> enemyDataList)
    {
        var score = 0;
        enemyDataList.ForEach(enemyData =>
        {
            score +=  GetTotalNum((int)enemyData.health,enemyData.enemySize);
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
                .SelectMany(_ => PlaySpawnCircleAnimationObservable(enemyData.position))
                .Do(_ => CreateEnemy(enemyData))
                .Subscribe();
            observableList.Add(observable);
        });
    }

    private IObservable<Unit> PlaySpawnCircleAnimationObservable(Vector3 position) {
        var y = 1f;
        var spawnCircle = (GameObject)Instantiate(_spawnCircle);
        spawnCircle.transform.position = new Vector3(position.x, y, position.z);
        spawnCircle.GetComponent<SpawnCircleManager>().SetAlpha(0);

        return spawnCircle.GetComponent<SpawnCircleManager>().PlayAnimationObservable();
    }

    private void CreateEnemy(EnemyData enemyData)
    {
        var y = 1.4f;
        var enemy = (GameObject)Instantiate(_enemyPrefabList[(int)enemyData.enemySize-1]);
        enemy.transform.position = new Vector3(enemyData.position.x, y, enemyData.position.z);

        var enemyManager = enemy.GetComponent<EnemyManager>();
        enemyManager.enemyData = enemyData;
        enemyManager.SetNum(enemyData.health);
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

    public void AddScore(float damage)
    {
        if (gameWindowUIScript == null) SetGameWindowUIScript();

        score += damage;
        if (gameWindowUIScript != null) gameWindowUIScript.SetScore(score);
        if (score >= stageClearScore) Clear();
    }

    private void SetGameWindowUIScript()
    {
        var window = UIManager.Instance.GetNowWindow();
        if (window.GetComponent<GameWindowUIScript>())
        {
            gameWindowUIScript = window.GetComponent<GameWindowUIScript>();
        }
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
        var num = GetSplitNum((int)enemyData.health);
        var size = GetOneLankLowerSize(enemyData.enemySize);

        var enemy1 = new EnemyData() { health = num, position = enemyData.position, direction = direction1, enemySize = size };
        var enemy2 = new EnemyData() { health = num, position = enemyData.position, direction = direction2, enemySize = size };
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

        var stageId = SaveDataUtil.Status.GetNextStageId();
        var stage = MasterRecords.GetStageMB().FirstOrDefault(m => m.Id == stageId);
        SaveDataUtil.Status.SetClearedStageId(stageId);

        Observable.ReturnUnit()
            .Delay(TimeSpan.FromSeconds(0.05f), Scheduler.MainThreadIgnoreTimeScale)
            .SelectMany(_ => ClearWindowFactory.Create(new ClearWindowRequest()
            {
                clearResultData = new ClearResultData() { rewardCoin = stage.RewardCoin,rewardGem = stage.RewardGem}
            }))
            .Do(_ => Time.timeScale = 1)
            .Do(_ => SceneManager.LoadScene(SceneManager.GetActiveScene().name))
            .Subscribe();
    }

}