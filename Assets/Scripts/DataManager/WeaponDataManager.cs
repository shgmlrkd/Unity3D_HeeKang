using System.Collections.Generic;
using UnityEngine;

public struct WeaponData
{
    public int Key;
    public string Name;
    public int Level;
    public string Description;
    public float AttackPower;
    public float AttackInterval;
    public float AttackRange;
    public float AttackSpeed;   
    public float Knockback;
    public int Pierce;
    public int ProjectileCount;
    public float LifeTime;
}

public class WeaponDataManager : MonoBehaviour
{
    private static WeaponDataManager _instance;

    public static WeaponDataManager Instance
    {
        get { return _instance; }
    }

    private Dictionary<int, WeaponData> _weaponDatas = new Dictionary<int, WeaponData>();
    private void Awake()
    {
        _instance = this;

        LoadWeaponData();
    }

    public WeaponData GetWeaponData(int key)
    {
        return _weaponDatas[key];
    }

    private void LoadWeaponData()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("GameDataFolder/WeaponDataTable");

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
            data.Description = colData[3];
            data.AttackPower = float.Parse(colData[4]);
            data.AttackInterval = float.Parse(colData[5]);
            data.AttackRange = float.Parse(colData[6]);
            data.AttackSpeed = float.Parse(colData[7]);
            data.Knockback = float.Parse(colData[8]);
            data.Pierce = int.Parse(colData[9]);
            data.ProjectileCount = int.Parse(colData[10]);
            data.LifeTime = float.Parse(colData[11]);

            _weaponDatas.Add(data.Key, data);
        }
    }
}