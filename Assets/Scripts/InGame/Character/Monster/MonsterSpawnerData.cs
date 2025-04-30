using System.Collections.Generic;
using UnityEngine;

// ������ ���� Ǯ, ���� ������ ����
public class MonsterSpawnerData
{
    private string _monsterName;
    private List<GameObject> _pool;
    private MonsterSpawnData _spawnData;

    public string MonsterName
    {
        get { return _monsterName; }
        set { _monsterName = value; }
    }

    public List<GameObject> Pool
    {
        get { return _pool; }
        set { _pool = value; }
    }

    public MonsterSpawnData SpawnData
    {
        get { return _spawnData; }
        set { _spawnData = value; }
    }

    // �����ڿ��� ���� �̸�, Ǯ ����Ʈ, ���� ������ �ʱ�ȭ
    public MonsterSpawnerData(string name, List<GameObject> pool, MonsterSpawnData SpawnData)
    {
        MonsterName = name;
        Pool = pool;
        _spawnData = SpawnData;
    }
}
