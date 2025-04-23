using System.Collections.Generic;
using UnityEngine;

public struct PlayerData
{
    public int Key;
    public string Name;
    public int EnchantLevel;
    public float Exp;
    public float Hp;
    public int HpGold;
    public float AttackPowerRate;
    public int AttackPowerGold;
    public float AttackSpeedRate;
    public int AttackSpeedGold;
    public float Speed;
    public int SpeedGold;
}

public class PlayerDataManager : Singleton<PlayerDataManager>
{
    private Dictionary<int, PlayerData> _playerDatas = new Dictionary<int, PlayerData>();

    private void Awake()
    {
        LoadPlayerData();
    }

    public float GetPlayerTotalExpToLevel(int level)
    {
        PlayerData totalExp = _playerDatas[level];

        return totalExp.Exp;
    }

    public PlayerData GetPlayerDataByStatLevel(int hpLevel, int expLevel, int attackLevel, int attackSpeedLevel, int speedLevel)
    {
        PlayerData result = new PlayerData();

        PlayerData hpData = _playerDatas[hpLevel];
        PlayerData expData = _playerDatas[expLevel];
        PlayerData attackData = _playerDatas[attackLevel];
        PlayerData attackSpeedData = _playerDatas[attackSpeedLevel];
        PlayerData speedData = _playerDatas[speedLevel];

        result.Hp = hpData.Hp;
        result.HpGold = hpData.HpGold;

        result.Exp = expData.Exp;

        result.AttackPowerRate = attackData.AttackPowerRate;
        result.AttackPowerGold = attackData.AttackPowerGold;

        result.AttackSpeedRate = attackSpeedData.AttackSpeedRate;
        result.AttackSpeedGold = attackSpeedData.AttackSpeedGold;

        result.Speed = speedData.Speed;
        result.SpeedGold = speedData.SpeedGold;

        return result;
    }

    private void LoadPlayerData()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("TableData/PlayerDataTable");

        string[] rowData = textAsset.text.Split("\r\n");

        for (int i = 1; i < rowData.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(rowData[i]))
                continue;

            string[] colData = rowData[i].Split(",");

            if (colData.Length <= 1)
                return;

            PlayerData data;

            data.Key = int.Parse(colData[0]);
            data.Name = colData[1];
            data.EnchantLevel = int.Parse(colData[2]);
            data.Exp = float.Parse(colData[3]);
            data.Hp = float.Parse(colData[4]);
            data.HpGold = int.Parse(colData[5]);
            data.AttackPowerRate = float.Parse(colData[6]);
            data.AttackPowerGold = int.Parse(colData[7]); 
            data.AttackSpeedRate = float.Parse(colData[8]);
            data.AttackSpeedGold = int.Parse(colData[9]);
            data.Speed = float.Parse(colData[10]);
            data.SpeedGold = int.Parse(colData[11]);

            _playerDatas.Add(data.Key, data);
        }
    }
}