using System.Collections.Generic;
using UnityEngine;

public struct PlayerData
{
    public int Key;
    public string Name;
    public float Exp;
    public float Hp;
    public float Speed;
}

public class PlayerDataManager : Singleton<PlayerDataManager>
{
    private Dictionary<int, PlayerData> _playerDatas = new Dictionary<int, PlayerData>();

    private void Awake()
    {
        LoadPlayerData();
    }

    public PlayerData GetPlayerData(int key)
    {
        return _playerDatas[key];
    }

    /*public PlayerData GetPlayerDataByStatLevel(int hpLevel, int expLevel, int attackLevel, int attackSpeedLevel, int speedLevel)
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
    }*/

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
            data.Exp = float.Parse(colData[2]);
            data.Hp = float.Parse(colData[3]);
            data.Speed = float.Parse(colData[4]);

            _playerDatas.Add(data.Key, data);
        }
    }
}