using System.Collections.Generic;
using UnityEngine;

public struct MonsterWeaponData
{
    public int Key;
    public string Name;
    public float AttackPower;
    public float AttackInterval;
    public float AttackSpeed;
    public float LifeTime;
}

public class MonsterWeaponDataManager : Singleton<MonsterWeaponDataManager>
{
    private Dictionary<int, MonsterWeaponData> _monsterWeaponDatas = new Dictionary<int, MonsterWeaponData>();
    private void Awake()
    {
        LoadMonsterWeaponData();
    }

    // 몬스터 무기 데이터
    public MonsterWeaponData GetMonsterWeaponData(int key)
    {
        return _monsterWeaponDatas[key];
    }

    private void LoadMonsterWeaponData()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("TableData/MonsterWeaponDataTable");

        string[] rowData = textAsset.text.Split("\r\n");

        for (int i = 1; i < rowData.Length; i++)
        {
            string[] colData = rowData[i].Split(",");

            if (colData.Length <= 1)
                return;

            // 몬스터 무기 관련 데이터
            MonsterWeaponData monsterWeaponData;

            monsterWeaponData.Key = int.Parse(colData[0]);
            monsterWeaponData.Name = colData[1];
            monsterWeaponData.AttackPower = float.Parse(colData[2]);
            monsterWeaponData.AttackInterval = float.Parse(colData[3]);
            monsterWeaponData.AttackSpeed = float.Parse(colData[4]);
            monsterWeaponData.LifeTime = float.Parse(colData[5]);

            _monsterWeaponDatas.Add(monsterWeaponData.Key, monsterWeaponData);
        }
    }
}
