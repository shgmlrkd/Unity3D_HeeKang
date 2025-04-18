using System.Collections.Generic;
//using System.Globalization;
using UnityEngine;
public struct PlayerData
{
    public int Key;
    public int Level;
    public float Speed;
    public float Experience;
    public int Hp;
}

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance { get; private set; }
    private Dictionary<int, PlayerData> _playerDataDict = new Dictionary<int, PlayerData>();
    public PlayerData GetPlayerData(int key)
    {
        return _playerDataDict[key];
    }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadCSV();
    }
    private void LoadCSV()
    {
        TextAsset csvFile = Resources.Load<TextAsset>("GameDataFolder/PlayerData");
        if (csvFile == null)
        {
            Debug.LogError("PlayerData.csv not found in Resources folder.");
            return;
        }

        string[] lines = csvFile.text.Split("\r\n");
        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrEmpty(line))
                continue;

            string[] values = line.Split(',');

            PlayerData data = new PlayerData
            {
                Key = int.Parse(values[0]),
                Level = int.Parse(values[1]),
                Speed = float.Parse(values[2]),
                Experience = float.Parse(values[3]),
                Hp = int.Parse(values[4])
            };

            _playerDataDict[data.Level] = data;
        }
        Debug.Log($"Loaded {_playerDataDict.Count} player data entries.");
    }
}