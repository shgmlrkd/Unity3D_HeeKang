using System.Collections.Generic;
using UnityEngine;

public struct MonsterData
{
    public int Key;
    public string Name;
    public string Type;
    public float Hp;
    public float MoveSpeed;
    public int AttackPower;
    public float AttackInterval;
    public int Distance;
    public int LifeTime;
    public int PlayerLevel;
    public float SpawnInterval;
    public int Exp;
}

public class MonsterDataManager : MonoBehaviour
{
    private static MonsterDataManager _instance;

    public static MonsterDataManager Instance
    {
        get { return _instance; }
    }

    private Dictionary<int, MonsterData> _monsterDatas = new Dictionary<int, MonsterData>();
    private void Awake()
    {
        _instance = this;

        LoadMonsterData();
    }

    public MonsterData GetMonsterData(int key)
    {
        return _monsterDatas[key];
    }

    private void LoadMonsterData()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("GameDataFolder/MonsterTable");

        string[] rowData = textAsset.text.Split("\r\n");

        for (int i = 1; i < rowData.Length; i++)
        {
            string[] colData = rowData[i].Split(",");

            if (colData.Length <= 1)
                return;

            MonsterData data;
            data.Key = int.Parse(colData[0]);
            data.Name = colData[1];
            data.Type = colData[2];
            data.Hp = float.Parse(colData[3]);
            data.MoveSpeed = float.Parse(colData[4]);
            data.AttackPower = int.Parse(colData[5]);
            data.AttackInterval = float.Parse(colData[6]);
            data.Distance = int.Parse(colData[7]);
            data.LifeTime = int.Parse(colData[8]);
            data.PlayerLevel = int.Parse(colData[9]);
            data.SpawnInterval = float.Parse(colData[10]);
            data.Exp = int.Parse(colData[11]);

            _monsterDatas.Add(data.Key, data);
        }
    }
}
