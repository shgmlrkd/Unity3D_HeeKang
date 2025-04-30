using System.Collections.Generic;
using UnityEngine;

public struct SelectPlayerData
{
    public int Key;
    public int PlayerKey;
    public int SkillIndex;
    public string PlayerName;
    public string PlayerTexturePath;
    public string SkillTexturePath;
}

public class SelectPlayerDataManager : Singleton<SelectPlayerDataManager>
{
    Dictionary<int, SelectPlayerData> _selectPalyerDatas = new Dictionary<int, SelectPlayerData>();

    private void Awake()
    {
        LoadSelectPlayerData();
    }

    public SelectPlayerData GetSelectPlayerData(int key)
    {
        return _selectPalyerDatas[key];
    }

    private void LoadSelectPlayerData()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("TableData/SelectPlayerDataTable");

        string[] rowData = textAsset.text.Split("\r\n");

        for (int i = 1; i < rowData.Length; i++)
        {
            string[] colData = rowData[i].Split(",");

            if (colData.Length <= 1)
                return;

            SelectPlayerData selectPlayerData;

            selectPlayerData.Key = int.Parse(colData[0]);
            selectPlayerData.PlayerKey = int.Parse(colData[1]);
            selectPlayerData.SkillIndex = int.Parse(colData[2]);
            selectPlayerData.PlayerName = colData[3];
            selectPlayerData.PlayerTexturePath = colData[4];
            selectPlayerData.SkillTexturePath = colData[5];

            _selectPalyerDatas.Add(selectPlayerData.Key, selectPlayerData);
        }
    }
}