using System.Collections.Generic;
using UnityEngine;

// 각각의 몬스터를 스폰하는 데이터 저장
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

    // 생성자에서 몬스터 이름, 스폰 간격, 풀 리스트 초기화
    public MonsterSpawnData(string name, float interval, List<GameObject> pool)
    {
        MonsterName = name;
        SpawnInterval = interval;
        Pool = pool;
    }
}
