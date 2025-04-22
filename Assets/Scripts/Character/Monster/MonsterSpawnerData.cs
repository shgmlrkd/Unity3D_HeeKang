using System.Collections.Generic;
using UnityEngine;

// 각각의 몬스터 풀, 스폰 데이터 저장
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

    // 생성자에서 몬스터 이름, 풀 리스트, 스폰 데이터 초기화
    public MonsterSpawnerData(string name, List<GameObject> pool, MonsterSpawnData SpawnData)
    {
        MonsterName = name;
        Pool = pool;
        _spawnData = SpawnData;
    }
}
