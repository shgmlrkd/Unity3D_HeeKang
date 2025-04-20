using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MonsterManager : Singleton<MonsterManager>
{
    private List<GameObject> _monsterPool;

    private LayerMask _groundLayer;

    private float _offset = 0.15f;
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
        Vector3 spawnPos = GetRandomOffscreenWorldPos();

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

    public void CreateMonsters(int poolSize, string monsterName)
    {
        GameObject monsterPrefab = Resources.Load<GameObject>($"Prefabs/Monster/{monsterName}");
        PoolingManager.Instance.Add(monsterName, poolSize, monsterPrefab, transform);
        _monsterPool = PoolingManager.Instance.GetObjects(monsterName);
        _spawnInterval = MonsterDataManager.Instance.GetMonsterSpawnIntervalData(monsterName);
    }

    private Vector3 GetRandomOffscreenWorldPos()
    {
        Camera cam = Camera.main;

        // 화면 밖 4방향 중 랜덤 선택
        int side = Random.Range(0, 4);
        Vector2 viewportPos = Vector2.zero;
        Vector3 worldPos = Vector3.zero;

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
            worldPos = hit.point;
        }

        return worldPos;
    }
    
    public GameObject GetClosestMonster(Vector3 pos)
    {
        GameObject closest = null;
        float minDistance = float.MaxValue;

        // 몬스터 풀을 돌려서 활성화 된 애들 중 가장 가까운거 찾기
        foreach (GameObject monster in _monsterPool)
        {
            if (!monster.activeSelf) continue;

            // 플레이어와 몬스터의 거리 구하기
            float dist = Vector3.Distance(pos, monster.transform.position);

            // 더 작으면 갱신
            if (dist < minDistance)
            {
                minDistance = dist;
                closest = monster;
            }
        }

        return closest;

        /*// 카메라에서 pos까지의 거리 계산 (z 값으로 사용됨)
        float distanceFromCamera = Vector3.Distance(Camera.main.transform.position, pos);

        // 뷰포트 좌표 (0, 0.5): 왼쪽 중앙 / (1, 0.5): 오른쪽 중앙 + 오프셋 범위
        // 해당 z 거리 위치에서 화면 좌우 끝이 월드에서 어디인지 계산
        Vector3 worldLeft = Camera.main.ViewportToWorldPoint(new Vector3(-_offset, 0.5f, distanceFromCamera));
        Vector3 worldRight = Camera.main.ViewportToWorldPoint(new Vector3(1 + _offset, 0.5f, distanceFromCamera));

        // 위에서 구한 좌우 월드 좌표의 거리 = 해당 거리에서 카메라가 보여주는 가로 너비
        float screenWidthInWorld = Vector3.Distance(worldLeft, worldRight);

        // 카메라가 현재 비추는 화면의 가로 길이를 기준으로 반지름을 설정
        float halfScreenWidth = screenWidthInWorld * 0.5f;

        // 화면 안쪽의 몬스터만 찾기
        Collider[] monsters = Physics.OverlapSphere(pos, halfScreenWidth, LayerMask.GetMask("Monster"));

        GameObject closest = null;
        float minDistance = float.MaxValue;

        foreach (Collider monster in monsters)
        {
            // 플레이어와 몬스터의 거리 구하기
            float dist = Vector3.Distance(pos, monster.transform.position);

            // 더 작으면 갱신
            if (dist < minDistance)
            {
                minDistance = dist;
                closest = monster.gameObject;
            }
        }

        return closest;*/

        /* 그냥 범위 고정으로 상수 변수 줘서 하는 방법
            Collider[] monsters = Physics.OverlapSphere(pos, 상수 변수, LayerMask.GetMask("Monster"));

            GameObject closest = null;
            float minDistance = float.MaxValue;

            foreach (Collider monster in monsters)
            {
                // 플레이어와 몬스터의 거리 구하기
                float dist = Vector3.Distance(pos, monster.transform.position);

                // 더 작으면 갱신
                if (dist < minDistance)
                {
                    minDistance = dist;
                    closest = monster.gameObject;
                }
            }

            return closest;
        */
    }
}