using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Simulation
{
    static float time = 0.0f;
    static float intervalTime = 0.5f;
    static float dps = 1.5f;
    static int totalEnemyNum = 0;
    static int deadEnemyNum = 0;
    static List<SimulationEnemyData> existingEnemyList = new List<SimulationEnemyData>();
    static List<SimulationEnemyData> enemyList = new List<SimulationEnemyData>();
    static List<float> momentDifficultyList = new List<float>();

    public static string GetSimulationResult(List<EnemyData> enemyDataList){
        enemyList = enemyDataList.Select(e => CreateSimulationEnemyDataFromEnemyData(e)).ToList();
        totalEnemyNum = CalculateTotalEnemyNum();
        SetUserInfo();

        // シミュレーションスタート
        while (!IsFinished())
        {
            SpawnEnemy();
            AddDifficulty();
            DebugLog();
            AttackEnemy();
            DebugLog();
            time += intervalTime;
        }

        DebugLogResult();

        return GetResultText();
    }

    private static void SetUserInfo() {
        var damageLevel = SaveDataUtil.Status.GetDamageLevel();
        var rateLevel = SaveDataUtil.Status.GetRateLevel();

        var damage = MasterRecords.GetDamageStatus(damageLevel);
        var rate = MasterRecords.GetRateStatus(rateLevel);

        dps = damage / rate;
    }

    private static void AddDifficulty()
    {
        momentDifficultyList.Add(GetMomentDifficulty());
    }

    private static void DebugLog()
    {
        //System.Console.WriteLine(GetFormatedNum(time) + "[s] " + GetFormatedNum(GetMomentDifficulty()) + "[難] " + string.Join(", ", existingEnemyList.Select(e => GetFormatedNum(e.currentHp)).ToList()));
    }

    private static void DebugLogResult()
    {
        //System.Console.WriteLine("平均:" + momentDifficultyList.Average() + " 最大:" + momentDifficultyList.Max());
    }

    private static string GetResultText() {
        return "平均:" + momentDifficultyList.Average() + " 最大:" + momentDifficultyList.Max();
    }

    private static string GetFormatedNum(float num)
    {
        return string.Format("{0,10}", string.Format("{00:F3}", num));
    }

    private static void AttackEnemy()
    {
        if (!existingEnemyList.Any()) return;

        var damage = dps * intervalTime;
        var randomIndex = UnityEngine.Random.Range(0, existingEnemyList.Count);
        var enemy = existingEnemyList[randomIndex];
        enemy.currentHp -= damage;

        if (enemy.currentHp <= 0)
        {
            // 敵が死んだら
            existingEnemyList.Remove(enemy);
            deadEnemyNum++;

            var list = enemy.Division();
            existingEnemyList.AddRange(list);
        }
    }

    private static void SpawnEnemy()
    {
        var spawnEnemyList = enemyList.Where(e => {
            return time - intervalTime < e.spawnTime && e.spawnTime <= time;
        })
        .ToList();
        existingEnemyList.AddRange(spawnEnemyList);
    }

    private static int CalculateTotalEnemyNum()
    {
        return enemyList.Select(e => {
            switch (e.size)
            {
                case EnemySize.Small:
                    return 1;
                case EnemySize.Medium:
                    return 3;
                case EnemySize.Large:
                    return 7;
                default:
                    return 1;
            }
        }).Sum();
    }

    private static float GetMomentDifficulty()
    {
        return existingEnemyList.Sum(e => e.CalculateMomentDifficulty());
    }

    private static bool IsFinished()
    {
        return deadEnemyNum == totalEnemyNum;
    }

    private static SimulationEnemyData CreateSimulationEnemyDataFromEnemyData(EnemyData enemyData) {
        return new SimulationEnemyData(enemyData.health, enemyData.enemySize, enemyData.enemyType, enemyData.time);
    }
}

public class SimulationEnemyData
{
    public float hp;
    public float currentHp;
    public EnemySize size;
    public EnemyType type;
    public float spawnTime;

    public SimulationEnemyData(float hp, EnemySize size, EnemyType type, float spawnTime)
    {
        this.hp = hp;
        this.currentHp = hp;
        this.size = size;
        this.type = type;
        this.spawnTime = spawnTime;
    }

    public List<SimulationEnemyData> Division()
    {
        if (size == EnemySize.Small) return new List<SimulationEnemyData>();

        var newHp = (int)Math.Ceiling((float)hp / 2);
        var newSize = (EnemySize)((int)size - 1);
        return new List<SimulationEnemyData>(){
            new SimulationEnemyData(newHp,newSize,type,0),
            new SimulationEnemyData(newHp,newSize,type,0)
        };
    }

    public float CalculateMomentDifficulty()
    {
        return currentHp * GetWeight(this.type) * GetWeight(this.size);
    }

    private float GetWeight(EnemyType type)
    {
        switch (type)
        {
            case EnemyType.Normal:
                return 1.0f;
            case EnemyType.Guard:
                return 1.3f;
            case EnemyType.Fast:
                return 1.2f;
            case EnemyType.Slow:
                return 1.2f;
            case EnemyType.Random:
                return 1.4f;
            default:
                return 1.0f;
        }
    }

    private float GetWeight(EnemySize size)
    {
        switch (size)
        {
            case EnemySize.Small:
                return 1.0f;
            case EnemySize.Medium:
                return 1.1f;
            case EnemySize.Large:
                return 1.3f;
            default:
                return 1.0f;
        }
    }
}

/*
using System;
using System.Collections.Generic;
using System.Linq;

public class Hello{
    static float time = 0.0f;
    static float intervalTime = 0.5f;
    static float dps = 1.5f;
    static int totalEnemyNum = 0;
    static int deadEnemyNum = 0;
    static List<EnemyInfo> existingEnemyList = new List<EnemyInfo>();
    static List<EnemyInfo> enemyList = new List<EnemyInfo>(){
        new EnemyInfo(10,Size.Small,Type.Normal,0),
        new EnemyInfo(10,Size.Medium,Type.Normal,2),
        new EnemyInfo(10,Size.Large,Type.Normal,4)
    };
    static List<float> momentDifficultyList = new List<float>();
        
    public static void Main(){
        totalEnemyNum = CalculateTotalEnemyNum();
        
        while(!IsFinished()){
            SpawnEnemy();
            AddDifficulty();
            DebugLog();
            AttackEnemy();
            DebugLog();
            time += intervalTime;
        }
        
        DebugLogResult();
    }
    
    private static void AddDifficulty(){
        momentDifficultyList.Add(GetMomentDifficulty());
    }
    
    private static void DebugLog(){
        System.Console.WriteLine(GetFormatedNum(time) + "[s] " + GetFormatedNum(GetMomentDifficulty()) + "[難] " + string.Join( ", ", existingEnemyList.Select(e => GetFormatedNum(e.currentHp)).ToList()));
    }
    
    private static void DebugLogResult(){
        System.Console.WriteLine("平均:" + momentDifficultyList.Average() + " 最大:" + momentDifficultyList.Max());
    }
    
    private static string GetFormatedNum(float num){
        return string.Format("{0,10}", string.Format("{00:F3}", num));
    }
    
    private static void AttackEnemy(){
        if(!existingEnemyList.Any()) return;
        
        Random rnd = new Random();
        var damage = dps * intervalTime;
        var randomIndex = rnd.Next(existingEnemyList.Count);
        var enemy = existingEnemyList[randomIndex];
        enemy.currentHp -= damage;
        
        if(enemy.currentHp <= 0){
            // 敵が死んだら
            existingEnemyList.Remove(enemy);
            deadEnemyNum++;
            
            var list = enemy.Division();
            existingEnemyList.AddRange(list);
        }
    }
    
    private static void SpawnEnemy(){
        var spawnEnemyList = enemyList.Where(e => {
            return time - intervalTime < e.spawnTime && e.spawnTime <= time;
        })
        .ToList();
        existingEnemyList.AddRange(spawnEnemyList);
    }
    
    private static int CalculateTotalEnemyNum(){
        return enemyList.Select(e => {
            switch(e.size){
            case Size.Small:
                return 1;
            case Size.Medium:
                return 3;
            case Size.Large:
                return 7;
            default:
                return 1;
            }
        }).Sum();
    }
    
    private static float GetMomentDifficulty(){
        return existingEnemyList.Sum(e => e.CalculateMomentDifficulty());
    }
    
    private static bool IsFinished(){
        return deadEnemyNum == totalEnemyNum;
    }
}

public class EnemyInfo{
    public float hp;
    public float currentHp;
    public Size size;
    public Type type;
    public float spawnTime;
    
    public EnemyInfo(float hp,Size size,Type type,float spawnTime){
        this.hp = hp;
        this.currentHp = hp;
        this.size = size;
        this.type = type;
        this.spawnTime = spawnTime;
    }
    
    public List<EnemyInfo> Division(){
        if(size == Size.Small) return new List<EnemyInfo>();
        
        var newHp = (int)Math.Ceiling((float)hp / 2);
        var newSize = (Size)((int)size - 1);
        return new List<EnemyInfo>(){
            new EnemyInfo(newHp,newSize,type,0),
            new EnemyInfo(newHp,newSize,type,0)
        };
    }
    
    public float CalculateMomentDifficulty(){
        return currentHp * GetWeight(this.type) * GetWeight(this.size);
    }
    
    private float GetWeight(Type type){
        switch(type){
        case Type.Normal:
            return 1.0f;
        case Type.Guard:
            return 1.3f;
        case Type.Fast:
            return 1.2f;
        case Type.Slow:
            return 1.2f;
        case Type.Random:
            return 1.4f;
        default:
            return 1.0f;
        }
    }
    
    private float GetWeight(Size size){
        switch(size){
        case Size.Small:
            return 1.0f;
        case Size.Medium:
            return 1.1f;
        case Size.Large:
            return 1.3f;
        default:
            return 1.0f;
        }
    }
}

public enum Size{
    Small = 0,
    Medium = 1,
    Large = 2,
}

public enum Type{
    Normal,
    Guard,
    Fast,
    Slow,
    Random
}
*/
