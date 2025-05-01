using System.Collections.Generic;
using UnityEngine;

public struct MonsterData
{
    public int Key;
    public string Name;
    public float Hp;
    public float Exp;
    public float MoveSpeed;
    public float RotateSpeed;
    public float AttackPower;
    public float AttackInterval;
    public float AttackDistance;
    public float LifeTime;
    public float StatScaleFactor;
    public float StatUpdateInterval;
}

public struct MonsterSpawnData
{
    public int Type;
    public float SpawnInterval;
    public float SpawnStartTime;
    public float SpawnEndTime;
    public float SpawnRange;
}

public class MonsterDataManager : Singleton<MonsterDataManager>
{
    private Dictionary<int, MonsterData> _monsterDatas = new Dictionary<int, MonsterData>();
    private Dictionary<string, MonsterSpawnData> _monsterSpawnIntervalDatas = new Dictionary<string, MonsterSpawnData>();

    private void Awake()
    {
        LoadMonsterData();
    }

    // 몬스터에 필요한 데이터
    public MonsterData GetMonsterData(int key)
    {
        return _monsterDatas[key];
    }

    // 몬스터 매니저(몬스터 스폰)에 필요한 데이터
    public MonsterSpawnData GetMonsterSpawnData(string monsterName)
    {
        return _monsterSpawnIntervalDatas[monsterName];
    }

    private void LoadMonsterData()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("TableData/MonsterDataTable");

        string[] rowData = textAsset.text.Split("\r\n");

        for (int i = 1; i < rowData.Length; i++)
        {
            string[] colData = rowData[i].Split(",");

            if (colData.Length <= 1)
                return;
            
            // 몬스터 관련 데이터
            MonsterData monsterData;

            monsterData.Key = int.Parse(colData[0]);
            monsterData.Name = colData[1];
            monsterData.Hp = float.Parse(colData[2]);
            monsterData.Exp = float.Parse(colData[3]);
            monsterData.MoveSpeed = float.Parse(colData[4]);
            monsterData.RotateSpeed = float.Parse(colData[5]);
            monsterData.AttackPower = float.Parse(colData[6]);
            monsterData.AttackInterval = float.Parse(colData[7]);
            monsterData.AttackDistance = float.Parse(colData[8]);
            monsterData.LifeTime = float.Parse(colData[9]);
            monsterData.StatScaleFactor = float.Parse(colData[10]);
            monsterData.StatUpdateInterval = float.Parse(colData[11]);

            // 몬스터 스폰 관련 데이터
            MonsterSpawnData monsterSpawnData;

            monsterSpawnData.Type = int.Parse(colData[12]);
            monsterSpawnData.SpawnInterval = float.Parse(colData[13]);
            monsterSpawnData.SpawnStartTime = float.Parse(colData[14]);
            monsterSpawnData.SpawnEndTime = float.Parse(colData[15]);
            monsterSpawnData.SpawnRange = float.Parse(colData[16]);

            _monsterDatas.Add(monsterData.Key, monsterData);
            _monsterSpawnIntervalDatas.Add(monsterData.Name, monsterSpawnData);
        }
    }
}
