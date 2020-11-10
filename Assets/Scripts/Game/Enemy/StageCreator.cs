using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class StageCreator
{
    /// <summary>
    /// ランダムでステージを作成
    /// stageId >= 1
    /// </summary>
    public static List<EnemyData> GetEnemySpawnDataList(int stageId)
    {
        var intervalAndDifficultyDataList = GetIntervalAndDifficultyDataList(stageId);
        var enemyDataList = intervalAndDifficultyDataList.Select(d => GetEnemyData(d)).ToList();

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

    private static EnemyData GetEnemyData(IntervalAndDifficultyData data)
    {
        var random = UnityEngine.Random.Range(1, 101);

        var health = 0;
        var size = EnemySize.None;
        var num = 0;
        switch (data.difficulty)
        {
            case Difficulty.Easy:
                if(random <= 50)
                {
                    health = (int)Math.Floor(data.interval);
                    size = EnemySize.Small;
                    num = 1;
                }
                else if(random <= 70)
                {
                    health = (int)Math.Floor(data.interval/2);
                    size = EnemySize.Small;
                    num = 2;
                }
                else if(random <= 90)
                {
                    health = (int)Math.Floor(data.interval / 2);
                    size = EnemySize.Medium;
                    num = 1;
                }
                else
                {
                    health = (int)Math.Floor(data.interval / 4);
                    size = EnemySize.Small;
                    num = 1;
                }
                health = (int)Math.Floor((float)(health * 2 / 3));
                break;
            case Difficulty.Medium:
                if (random <= 15)
                {
                    health = (int)Math.Floor(data.interval);
                    size = EnemySize.Small;
                    num = 1;
                }
                else if (random <= 25)
                {
                    health = (int)Math.Floor(data.interval / 2);
                    size = EnemySize.Small;
                    num = 2;
                }
                else if (random <= 85)
                {
                    health = (int)Math.Floor(data.interval / 2);
                    size = EnemySize.Medium;
                    num = 1;
                }
                else if (random <= 90)
                {
                    health = (int)Math.Floor(data.interval / 4);
                    size = EnemySize.Medium;
                    num = 2;
                }
                else if (random <= 95)
                {
                    health = (int)Math.Floor(data.interval / 4);
                    size = EnemySize.Large;
                    num = 1;
                }
                else
                {
                    health = (int)Math.Floor(data.interval / 8);
                    size = EnemySize.Large;
                    num = 2;
                }
                break;
            case Difficulty.Difficult:
                if (random <= 5)
                {
                    health = (int)Math.Floor(data.interval / 2);
                    size = EnemySize.Small;
                    num = 2;
                }
                else if (random <= 25)
                {
                    health = (int)Math.Floor(data.interval / 4);
                    size = EnemySize.Small;
                    num = 4;
                }
                else if (random <= 45)
                {
                    health = (int)Math.Floor(data.interval / 2);
                    size = EnemySize.Medium;
                    num = 1;
                }
                else if (random <= 60)
                {
                    health = (int)Math.Floor(data.interval / 4);
                    size = EnemySize.Medium;
                    num = 2;
                }
                else if (random <= 65)
                {
                    health = (int)Math.Floor(data.interval / 8);
                    size = EnemySize.Medium;
                    num = 4;
                }
                else if (random <= 85)
                {
                    health = (int)Math.Floor(data.interval / 4);
                    size = EnemySize.Large;
                    num = 1;
                }
                else if (random <= 95)
                {
                    health = (int)Math.Floor(data.interval / 8);
                    size = EnemySize.Large;
                    num = 2;
                }
                else
                {
                    health = (int)Math.Floor(data.interval / 16);
                    size = EnemySize.Large;
                    num = 4;
                }
                health = (int)Math.Floor((float)(health * 3 / 2));
                break;
        }


        return new EnemyData();
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
