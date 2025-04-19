using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MonsterManager : Singleton<MonsterManager>
{
    private List<GameObject> _monsterPool;

    private LayerMask _groundLayer;

    private float _offset = 0.2f;
    private float _spawnInterval;

    private enum ScreenSide
    {
        Left, Right, Up, Down
    }

    private void Awake()
    {
        _monsterPool = new List<GameObject>();
        _groundLayer = LayerMask.GetMask("Ground");
    }

    private void Start()
    {
        StartCoroutine(Spawner());
    }

    private IEnumerator Spawner()
    {
        while (true)
        {
            SpawnMonster();
            yield return new WaitForSeconds(_spawnInterval);
        }
    }

    private void SpawnMonster()
    {
        Vector3 spawnPos = GetValidSpawnPos();

        for (int i = 0; i < _monsterPool.Count; i++)
        {
            if (!_monsterPool[i].activeSelf)
            {
                _monsterPool[i].transform.position = spawnPos;
                _monsterPool[i].SetActive(true);
                return;
            }
        }
    }

    private Vector3 GetValidSpawnPos()
    {
        Vector3 pos = GetRandomOffscreenWorldPos();

        if (pos == Vector3.zero)
            return GetValidSpawnPos(); // 재귀 호출

        return pos;
    }

    public void CreateMonsters(int poolSize, string monsterName)
    {
        GameObject monsterPrefab = Resources.Load<GameObject>($"Prefabs/Monster/{monsterName}");
        PoolingManager.Instance.Add(monsterName, poolSize, monsterPrefab, transform);
        _monsterPool = PoolingManager.Instance.GetObjects(monsterName);
        _spawnInterval = MonsterDataManager.Instance.GetMonsterSpawnIntervalData(monsterName);
    }

    Vector3 GetRandomOffscreenWorldPos()
    {
        Camera cam = Camera.main;

        // 화면 밖 4방향 중 랜덤 선택
        int side = Random.Range(0, 4);
        Vector2 viewportPos = Vector2.zero;

        switch ((ScreenSide)side)
        {
            case ScreenSide.Up:
                viewportPos = new Vector2(Random.value, 1 + _offset); 
                break; 
            case ScreenSide.Down: 
                viewportPos = new Vector2(Random.value, -_offset); 
                break;    
            case ScreenSide.Left: 
                viewportPos = new Vector2(-_offset, Random.value); 
                break;
            case ScreenSide.Right: 
                viewportPos = new Vector2(1 + _offset, Random.value); 
                break; 
        }

        Ray ray = cam.ViewportPointToRay(new Vector3(viewportPos.x, viewportPos.y, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000.0f, _groundLayer))
        {
            return hit.point;
        }

        return Vector3.zero;
    }
}
