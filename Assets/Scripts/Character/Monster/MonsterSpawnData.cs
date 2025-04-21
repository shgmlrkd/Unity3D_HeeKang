using System.Collections.Generic;
using UnityEngine;

// ������ ���͸� �����ϴ� ������ ����
public class MonsterSpawnData
{
    private string _monsterName;
    private float _spawnInterval;
    private List<GameObject> _pool;

    public string MonsterName
    {
        get { return _monsterName; }
        set { _monsterName = value; }
    }

    public float SpawnInterval
    {
        get { return _spawnInterval; }
        set { _spawnInterval = value; }
    }

    public List<GameObject> Pool
    {
        get { return _pool; }
        set { _pool = value; }
    }

    // �����ڿ��� ���� �̸�, ���� ����, Ǯ ����Ʈ �ʱ�ȭ
    public MonsterSpawnData(string name, float interval, List<GameObject> pool)
    {
        MonsterName = name;
        SpawnInterval = interval;
        Pool = pool;
    }
}
