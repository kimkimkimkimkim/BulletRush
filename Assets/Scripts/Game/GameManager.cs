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
    [SerializeField] public bool _isTestMode;
    [SerializeField] public bool _isCreateNewStageData;

    [HideInInspector] public float score;
    [HideInInspector] public float stageClearScore;
    [HideInInspector] public Phase phase;

    private GameWindowUIScript gameWindowUIScript;
    private List<EnemyManager> enemyManagerList = new List<EnemyManager>();
    private List<IDisposable> observableList = new List<IDisposable>();
    private IDisposable phase2Observable;
    private IDisposable phase3Observable;
    private int killNum;
    private int stageClearKillNum;

    private void Start()
    {
        ConnectCharaAndJoystick();
        OfflineRewardReceiveDialogFactory.Create(new OfflineRewardReceiveDialogRequest() { content = "22" }).Subscribe();

        if(_isCreateNewStageData) StageCreator.CreateStage();
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
        score = 0;
        killNum = 0;
        stageClearKillNum = GetStageClearKillNum(enemySpawnDataList);
        InitializePhase();
        enemyManagerList.Clear();
        CreateEnemy(enemySpawnDataList);

        GameWindowFactory.Create(new GameWindowRequest() { simulationResultText = "" })
            .Subscribe();
    }

    private void InitializePhase()
    {
        phase = Phase.Phase1;
        phase2Observable = Observable.Timer(TimeSpan.FromSeconds(15))
            .Do(_ => {
                phase = Phase.Phase2;
                enemyManagerList.ForEach(e => SetSpeedAndMove(e));
                UIManager.Instance.PlaySpeedUpAnimationObservable();
            })
            .Subscribe();
        phase3Observable = Observable.Timer(TimeSpan.FromSeconds(45))
            .Do(_ => {
                phase = Phase.Phase3;
                enemyManagerList.ForEach(e => SetSpeedAndMove(e));
                UIManager.Instance.PlaySpeedUpAnimationObservable();
            })
            .Subscribe();
    }

    private void SetSpeedAndMove(EnemyManager enemyManager)
    {
        var rigidbody = enemyManager.GetComponent<Rigidbody>();
        enemyManager.SetSpeed(phase);
        enemyManager.Move(rigidbody.velocity);
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

    private int GetStageClearKillNum(List<EnemyData> enemyDataList)
    {
        return enemyDataList
            .Select(e =>
            {
                switch (e.enemySize)
                {
                    case EnemySize.Small:
                        return 1;
                    case EnemySize.Medium:
                        return 3;
                    case EnemySize.Large:
                        return 7;
                    default:
                        return 0;
                }
            })
            .Sum();
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

        return spawnCircle.GetComponent<SpawnCircleManager>().PlayAnimationObservable().DoOnCompleted(() => Destroy(spawnCircle));
    }

    private void CreateEnemy(EnemyData enemyData)
    {
        var y = 1.4f;
        var enemy = (GameObject)Instantiate(_enemyPrefabList[(int)enemyData.enemySize-1]);
        enemy.transform.position = new Vector3(enemyData.position.x, y, enemyData.position.z);

        var enemyManager = enemy.GetComponent<EnemyManager>();
        enemyManagerList.Add(enemyManager);
        enemyManager.enemyData = enemyData;
        enemyManager.Init(enemyData.health, enemyData.direction, phase);
    }

    private void Dispose()
    {
        observableList.ForEach(observable =>
        {
            observable.Dispose();
        });
        observableList.Clear();

        if (phase2Observable != null) phase2Observable.Dispose();
        if (phase3Observable != null) phase3Observable.Dispose();
    }

    public void Defeat() {
        Time.timeScale = 0;
        UIManager.Instance.SetUI(UIMode.Defeat);

        DefeatWindowFactory.Create(new DefeatWindowRequest())
            .Do(res =>
            {
                if (res.isContinue) {
                    MobileAdsManager.Instance.TryShowRewarded(() =>
                    {
                        Time.timeScale = 1;
                        UIManager.Instance.SetUI(UIMode.Playing);
                    });
                }
                else
                {
                    Time.timeScale = 1;
                    MobileAdsManager.Instance.TryShowInterstitial();
                    MobileAdsManager.Instance.DestroyBanner();
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
    }

    private void SetGameWindowUIScript()
    {
        var window = UIManager.Instance.GetNowWindow();
        if (window.GetComponent<GameWindowUIScript>())
        {
            gameWindowUIScript = window.GetComponent<GameWindowUIScript>();
        }
    }

    public void KillTheEnemy(EnemyManager enemyManager)
    {
        enemyManagerList.Remove(enemyManager);

        var enemyData = enemyManager.enemyData;
        if(enemyData.enemySize != EnemySize.Small)
        {
            Split(enemyData);
        }

        killNum++;
        if (killNum >= stageClearKillNum) Clear();
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
        Dispose();

        var stageId = SaveDataUtil.Status.GetNextStageId();
        var rewardCoinStatusLevel = SaveDataUtil.Status.GetCoinLevel();
        var rewardCoinStatus = MasterRecords.GetCoinBonusStatus(rewardCoinStatusLevel);
        var rewardCoin = MasterRecords.GetStageClearRewardCoin(stageId);
        SaveDataUtil.Status.SetClearedStageId(stageId);
        Observable.ReturnUnit()
            .Delay(TimeSpan.FromSeconds(0.05f), Scheduler.MainThreadIgnoreTimeScale)
            .SelectMany(_ => ClearWindowFactory.Create(new ClearWindowRequest()
            {
                clearResultData = new ClearResultData() { 
                    rewardCoin = (int)(rewardCoin * rewardCoinStatus),
                    rewardGem = 1
                },
            }))
            .Do(_ =>
            {
                Time.timeScale = 1;
                MobileAdsManager.Instance.DestroyBanner();
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            })
            .Subscribe();
    }

    public enum Phase
    {
        Phase1,
        Phase2,
        Phase3,
    }
}