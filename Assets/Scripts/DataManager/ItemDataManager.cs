using System.Collections.Generic;
using UnityEngine;

public struct ItemData
{
    public int Key;
    public string Name;
    public float DropRate;
    public int MinValue;
    public int MaxValue;
}

public class ItemDataManager : Singleton<ItemDataManager>
{
    private Dictionary<int, ItemData> _itemDatas = new Dictionary<int, ItemData>();

    private void Awake()
    {
        LoadItemData();
    }

    public int GetItemsCount()
    {
        return _itemDatas.Count;
    }

    public ItemData GetItemData(int key)
    {
        return _itemDatas[key];
    }

    private void LoadItemData()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("TableData/ItemDataTable");

        string[] rowData = textAsset.text.Split("\r\n");

        for (int i = 1; i < rowData.Length; i++)
        {
            string[] colData = rowData[i].Split(",");

            if (colData.Length <= 1)
                return;

            ItemData data;

            data.Key = int.Parse(colData[0]);
            data.Name = colData[1];
            data.DropRate = float.Parse(colData[2]);
            data.MinValue = int.Parse(colData[3]);
            data.MaxValue = int.Parse(colData[4]);

            _itemDatas.Add(data.Key, data);
        }
    }
}