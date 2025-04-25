using System.Collections.Generic;
using UnityEngine;

public struct WeaponData
{
    public int Key;
    public string Name;
    public int Level;
    public string UIPath;
    public string Description;
    public float AttackPower;
    public float AttackInterval;
    public float AttackRange;
    public float AttackSpeed;   
    public float Knockback;
    public int Pierce;
    public int ProjectileCount;
    public float LifeTime;
    public float RotSpeed;
}

public class WeaponDataManager : Singleton<WeaponDataManager>
{
    private Dictionary<int, WeaponData> _weaponDatas = new Dictionary<int, WeaponData>();

    private readonly int _weaponMaxLevel = 5;
    public int WeaponMaxLevel
    {
        get { return _weaponMaxLevel; }
    }
    private int _weaponCount;
    public int WeaponCount
    {
        get { return _weaponCount; }
    }

    private void Awake()
    {
        LoadWeaponData();

        _weaponCount = _weaponDatas.Count / _weaponMaxLevel;
    }

    public WeaponData GetWeaponData(int key)
    {
        return _weaponDatas[key];
    }

    private void LoadWeaponData()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("TableData/WeaponDataTable");

        string[] rowData = textAsset.text.Split("\r\n");

        for (int i = 1; i < rowData.Length; i++)
        {
            string[] colData = rowData[i].Split(",");

            if (colData.Length <= 1)
                return;

            WeaponData data;

            data.Key = int.Parse(colData[0]);
            data.Name = colData[1];
            data.Level = int.Parse(colData[2]);
            data.UIPath = colData[3];
            data.Description = colData[4];
            data.AttackPower = float.Parse(colData[5]);
            data.AttackInterval = float.Parse(colData[6]);
            data.AttackRange = float.Parse(colData[7]);
            data.AttackSpeed = float.Parse(colData[8]);
            data.Knockback = float.Parse(colData[9]);
            data.Pierce = int.Parse(colData[10]);
            data.ProjectileCount = int.Parse(colData[11]);
            data.LifeTime = float.Parse(colData[12]);
            data.RotSpeed = float.Parse(colData[13]);

            _weaponDatas.Add(data.Key, data);
        }
    }
}