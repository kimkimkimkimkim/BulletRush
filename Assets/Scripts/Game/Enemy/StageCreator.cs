﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BulletRush.MasterRecord;
using UnityEngine;

public static class StageCreator
{
    private static readonly string path = Application.dataPath + "/Resources/MasterRecords/";
    private static readonly string fileName = "StageData.json";

    public static void CreateStage()
    {
        var tempJson = new List<string>();
        for(var i = 0; i < ConstUtil.MAX_STAGE_COUNT; i++)
        {
            var enemyDataList = GetEnemyDataList(i + 1);
            tempJson.Add(JsonUtility.ToJson(new Serialization<EnemyData>(enemyDataList)));
        }
        var stageDataJson = JsonUtility.ToJson(new Serialization<string>(tempJson), true);

        using (StreamWriter writer = new StreamWriter(path + fileName, false, Encoding.GetEncoding("utf-8")))
        {
            writer.WriteLine(stageDataJson);
        }
    }

    public static List<List<EnemyData>> GetStageData()
    {
        var jsonStr = Resources.Load<TextAsset>("MasterRecords/StageData").ToString();
        var tempData = JsonUtility.FromJson<Serialization<string>>(jsonStr).ToList();
        var stageData = tempData.Select(json => JsonUtility.FromJson<Serialization<EnemyData>>(json).ToList()).ToList();
        return stageData;
    }

    // List<T>
    [Serializable]
    public class Serialization<T>
    {
        [SerializeField]
        List<T> target;
        public List<T> ToList() { return target; }

        public Serialization(List<T> target)
        {
            this.target = target;
        }
    }

    /// <summary>
    /// ランダムでステージを作成
    /// stageId >= 1
    /// </summary>
    public static List<EnemyData> GetEnemyDataList(int stageId)
    {
        var intervalAndDifficultyDataList = GetIntervalAndDifficultyDataList(stageId);
        var simpleEnemyDataList = new List<SimpleEnemyData>();
        var time = 0f;
        for(var i = 0; i< intervalAndDifficultyDataList.Count; i++)
        {
            var intervalAndDifficultyData = intervalAndDifficultyDataList[i];
            simpleEnemyDataList = simpleEnemyDataList.Concat(GetSimpleEnemyDataList(stageId, intervalAndDifficultyData,time)).ToList();
            time += intervalAndDifficultyData.interval;
        }

        // simpleからの変換
        var enemyDataList = simpleEnemyDataList.Select(s =>
        {
            return new EnemyData()
            {
                time = s.time,
                health = s.health,
                position = EnemyUtil.GetPosition(s.position),
                direction = EnemyUtil.GetDirection(s.direction),
                enemySize = s.enemySize,
                enemyType = s.enemyType,
            };
        }).ToList();

        // 角度乱数発生
        enemyDataList = enemyDataList.Select(e =>
        {
            var randomAngle = UnityEngine.Random.Range(-10.0f, 10.0f);
            return new EnemyData()
            {
                time = e.time,
                health = e.health,
                position = e.position,
                direction = Quaternion.Euler(0, randomAngle, 0) * e.direction,
                enemySize = e.enemySize,
                enemyType = e.enemyType,
            };
        }).ToList();

        return enemyDataList;
    }

    private static List<SimpleEnemyData> GetSimpleEnemyDataList(int stageId,IntervalAndDifficultyData data,float time)
    {
        var random = UnityEngine.Random.Range(1, 101);
        var assumptionDps = MasterRecords.GetAssumptionDPS(stageId);
        var health = 0;
        var size = EnemySize.None;
        var num = 0;
        switch (data.difficulty)
        {
            case Difficulty.Easy:
                if(random <= 50)
                {
                    health = (int)Math.Ceiling(data.interval * assumptionDps);
                    size = EnemySize.Small;
                    num = 1;
                }
                else if(random <= 70)
                {
                    health = (int)Math.Ceiling(data.interval * assumptionDps / 2);
                    size = EnemySize.Small;
                    num = 2;
                }
                else if(random <= 90)
                {
                    health = (int)Math.Ceiling(data.interval * assumptionDps / 2);
                    size = EnemySize.Medium;
                    num = 1;
                }
                else
                {
                    health = (int)Math.Ceiling(data.interval * assumptionDps / 4);
                    size = EnemySize.Small;
                    num = 1;
                }
                health = (int)Math.Ceiling((float)(health * 2 / 3));
                break;
            case Difficulty.Medium:
                if (random <= 15)
                {
                    health = (int)Math.Ceiling(data.interval * assumptionDps);
                    size = EnemySize.Small;
                    num = 1;
                }
                else if (random <= 25)
                {
                    health = (int)Math.Ceiling(data.interval * assumptionDps / 2);
                    size = EnemySize.Small;
                    num = 2;
                }
                else if (random <= 85)
                {
                    health = (int)Math.Ceiling(data.interval * assumptionDps / 2);
                    size = EnemySize.Medium;
                    num = 1;
                }
                else if (random <= 90)
                {
                    health = (int)Math.Ceiling(data.interval * assumptionDps / 4);
                    size = EnemySize.Medium;
                    num = 2;
                }
                else if (random <= 95)
                {
                    health = (int)Math.Ceiling(data.interval * assumptionDps / 4);
                    size = EnemySize.Large;
                    num = 1;
                }
                else
                {
                    health = (int)Math.Ceiling(data.interval * assumptionDps / 8);
                    size = EnemySize.Large;
                    num = 2;
                }
                break;
            case Difficulty.Difficult:
                if (random <= 5)
                {
                    health = (int)Math.Ceiling(data.interval * assumptionDps / 2);
                    size = EnemySize.Small;
                    num = 2;
                }
                else if (random <= 25)
                {
                    health = (int)Math.Ceiling(data.interval * assumptionDps / 4);
                    size = EnemySize.Small;
                    num = 4;
                }
                else if (random <= 45)
                {
                    health = (int)Math.Ceiling(data.interval * assumptionDps / 2);
                    size = EnemySize.Medium;
                    num = 1;
                }
                else if (random <= 60)
                {
                    health = (int)Math.Ceiling(data.interval * assumptionDps / 4);
                    size = EnemySize.Medium;
                    num = 2;
                }
                else if (random <= 65)
                {
                    health = (int)Math.Ceiling(data.interval * assumptionDps / 8);
                    size = EnemySize.Medium;
                    num = 4;
                }
                else if (random <= 85)
                {
                    health = (int)Math.Ceiling(data.interval * assumptionDps / 4);
                    size = EnemySize.Large;
                    num = 1;
                }
                else if (random <= 95)
                {
                    health = (int)Math.Ceiling(data.interval * assumptionDps / 8);
                    size = EnemySize.Large;
                    num = 2;
                }
                else
                {
                    health = (int)Math.Ceiling(data.interval * assumptionDps / 16);
                    size = EnemySize.Large;
                    num = 4;
                }
                health = (int)Math.Ceiling((float)(health * 3 / 2));
                break;
        }

        var positionList = GetPositionList(num);
        var simpleEnemyDataList = positionList.Select(position =>
        {
            return new SimpleEnemyData()
            {
                time = time,
                health = health,
                position = position,
                direction = GetDirection(),
                enemySize = size,
                enemyType = EnemyType.Normal,
            };
        }).ToList();

        return simpleEnemyDataList;
    }

    private static List<IntervalAndDifficultyData> GetIntervalAndDifficultyDataList(int stageId)
    {
        var remain = stageId % 10;
        var intervalAndDifficultyMasterList = new List<IntervalAndDifficultyMaster>();
        switch (remain) {
            case 1:
            case 2:
            case 3:
            case 4:
            case 5:
                intervalAndDifficultyMasterList = new List<IntervalAndDifficultyMaster>()
                {
                    new IntervalAndDifficultyMaster(4,0,0,1,3,0),
                    new IntervalAndDifficultyMaster(1,2,0,1,2,0),
                    new IntervalAndDifficultyMaster(2,0,1,1,2,0),
                };
                break;
            case 6:
                intervalAndDifficultyMasterList = new List<IntervalAndDifficultyMaster>()
                {
                    new IntervalAndDifficultyMaster(5,0,0,1,4,0),
                    new IntervalAndDifficultyMaster(2,2,0,1,3,0),
                    new IntervalAndDifficultyMaster(3,0,1,1,3,0),
                };
                break;
            case 7:
                intervalAndDifficultyMasterList = new List<IntervalAndDifficultyMaster>()
                {
                    new IntervalAndDifficultyMaster(6,0,0,1,5,0),
                    new IntervalAndDifficultyMaster(0,4,0,1,3,0),
                    new IntervalAndDifficultyMaster(4,0,1,1,4,0),
                };
                break;
            case 8:
                intervalAndDifficultyMasterList = new List<IntervalAndDifficultyMaster>()
                {
                    new IntervalAndDifficultyMaster(8,0,0,2,5,1),
                    new IntervalAndDifficultyMaster(5,2,0,2,4,1),
                    new IntervalAndDifficultyMaster(3,2,1,1,4,1),
                };
                break;
            case 9:
                intervalAndDifficultyMasterList = new List<IntervalAndDifficultyMaster>()
                {
                    new IntervalAndDifficultyMaster(11,0,0,2,6,3),
                    new IntervalAndDifficultyMaster(8,2,0,2,5,3),
                    new IntervalAndDifficultyMaster(6,2,1,1,5,3),
                };
                break;
            case 0:
                intervalAndDifficultyMasterList = new List<IntervalAndDifficultyMaster>()
                {
                    new IntervalAndDifficultyMaster(14,0,0,2,7,5),
                    new IntervalAndDifficultyMaster(11,2,0,2,6,5),
                    new IntervalAndDifficultyMaster(9,2,1,1,7,4),
                };
                break;
        }
        return intervalAndDifficultyMasterList.GetRandom().GetShuffledDataList();
    }

    private static List<int> GetPositionList(int num)
    {
        var positionList = new List<int>();

        for (var i = 0; i < 12; i++)
        {
            positionList.Add(1);
        }

        for (var i = 0; i < 12; i++)
        {
            positionList.Add(2);
        }

        for (var i = 0; i < 12; i++)
        {
            positionList.Add(3);
        }

        for (var i = 0; i < 12; i++)
        {
            positionList.Add(4);
        }

        for (var i = 0; i < 4; i++)
        {
            positionList.Add(5);
        }

        for (var i = 0; i < 12; i++)
        {
            positionList.Add(6);
        }

        for (var i = 0; i < 12; i++)
        {
            positionList.Add(7);
        }

        for (var i = 0; i < 12; i++)
        {
            positionList.Add(8);
        }

        for (var i = 0; i < 12; i++)
        {
            positionList.Add(9);
        }

        positionList = positionList.GetShuffledList();

        return positionList.GetRange(0, num);
    }

    private static int GetDirection()
    {
        var directionList = new List<int>() { 1, 2, 3, 4 };
        return directionList.GetRandom();
    }
}

public class IntervalAndDifficultyMaster
{
    public List<float> intervalList { get; set; }
    public List<Difficulty> difficultyList { get; set; }

    public IntervalAndDifficultyMaster(int twoNum,int threeNum,int fourNum,int easyNum,int mediumNum,int difficultNum)
    {
        intervalList = new List<float>();
        for(var i = 0; i < twoNum; i++)
        {
            intervalList.Add(2);
        }
        for (var i = 0; i < threeNum; i++)
        {
            intervalList.Add(3);
        }
        for (var i = 0; i < fourNum; i++)
        {
            intervalList.Add(4);
        }

        difficultyList = new List<Difficulty>();
        for (var i = 0; i < easyNum; i++)
        {
            difficultyList.Add(Difficulty.Easy);
        }
        for (var i = 0; i < mediumNum; i++)
        {
            difficultyList.Add(Difficulty.Medium);
        }
        for (var i = 0; i < difficultNum; i++)
        {
            difficultyList.Add(Difficulty.Difficult);
        }
    }

    public List<IntervalAndDifficultyData> GetShuffledDataList()
    {
        intervalList = intervalList.OrderBy(a => Guid.NewGuid()).ToList();
        difficultyList = difficultyList.OrderBy(a => Guid.NewGuid()).ToList();

        var count = Math.Min(intervalList.Count, difficultyList.Count);
        var shuffledDataList = new List<IntervalAndDifficultyData>();
        for(var i = 0; i < count; i++)
        {
            shuffledDataList.Add(new IntervalAndDifficultyData()
            {
                interval = intervalList[i],
                difficulty = difficultyList[i]
            });
        }
        return shuffledDataList;
    }
}

public class IntervalAndDifficultyData
{
    public float interval { get; set; }
    public Difficulty difficulty { get; set; }

}

public enum Difficulty
{
    Easy,
    Medium,
    Difficult
}
