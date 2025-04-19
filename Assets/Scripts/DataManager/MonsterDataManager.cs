using System.Collections.Generic;
using UnityEngine;

public struct MonsterData
{
    public int Key;
    public string Name;
    public int Type;
    public float Hp;
    public int Exp;
    public float MoveSpeed;
    public float RotateSpeed;
    public float AttackPower;
    public float AttackInterval;
    public float AttackDistance;
    public float LifeTime;
    public float SpawnInterval;
    public float SpawnStartTime;
    public float SpawnEndTime;
    public float StatScaleFactor;
}

public class MonsterDataManager : Singleton<MonsterDataManager>
{
    private Dictionary<int, MonsterData> _monsterDatas = new Dictionary<int, MonsterData>();
    private Dictionary<string, float> _monsterSpawnIntervalDatas = new Dictionary<string, float>();

    private void Awake()
    {
        LoadMonsterData();
    }

    public MonsterData GetMonsterData(int key)
    {
        return _monsterDatas[key];
    }

    public float GetMonsterSpawnIntervalData(string monsterName)
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

            MonsterData data;

            data.Key = int.Parse(colData[0]);
            data.Name = colData[1];
            data.Type = int.Parse(colData[2]);
            data.Hp = float.Parse(colData[3]);
            data.Exp = int.Parse(colData[4]);
            data.MoveSpeed = float.Parse(colData[5]);
            data.RotateSpeed = float.Parse(colData[6]);
            data.AttackPower = float.Parse(colData[7]);
            data.AttackInterval = float.Parse(colData[8]);
            data.AttackDistance = float.Parse(colData[9]);
            data.LifeTime = float.Parse(colData[10]);
            data.SpawnInterval = float.Parse(colData[11]);
            data.SpawnStartTime = float.Parse(colData[12]);
            data.SpawnEndTime = float.Parse(colData[13]);
            data.StatScaleFactor = float.Parse(colData[14]);

            _monsterDatas.Add(data.Key, data);
            _monsterSpawnIntervalDatas.Add(data.Name, data.SpawnInterval);
        }
    }
}
