using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MonsterManager : Singleton<MonsterManager>
{
    private GameObject _inGameTimer;
    private List<GameObject> _monsterPool;
    private Dictionary<string, MonsterSpawnerData> _monsterSpawnDataDict;

    private LayerMask _groundLayer;

    private float _inGameTime;
    private float _offset = 0.15f;

    private enum ScreenSide
    {
        Left, Right, Up, Down
    }

    private void Awake()
    {
        _monsterPool = new List<GameObject>();
        _monsterSpawnDataDict = new Dictionary<string, MonsterSpawnerData>();
        _groundLayer = LayerMask.GetMask("Ground");
    }

    private void Start()
    {
        // 인게임 시간을 가져오기 위해 InGamePlayTimer 오브젝트를 찾음
        _inGameTimer = GameObject.Find("InGamePlayTimer");
    }

    private void Update()
    {
        // 인게임 시간 받아오기
        _inGameTime = _inGameTimer.GetComponent<InGameTime>().InGameTimer;
    }

    public void CreateMonsters(int poolSize, string monsterName)
    {
        // 몬스터 이름에 맞는 프리팹 로드 후 풀링으로 만들기
        GameObject monsterPrefab = Resources.Load<GameObject>($"Prefabs/Monster/{monsterName}");
        PoolingManager.Instance.Add(monsterName, poolSize, monsterPrefab, transform);
        List<GameObject> pool = PoolingManager.Instance.GetObjects(monsterName);
        // 각 몬스터 스폰 관련 데이터 받기
        MonsterSpawnData spawnData = MonsterDataManager.Instance.GetMonsterSpawnData(monsterName);
        // 각 정보를 MonsterSpawnData 클래스에서 초기화
        MonsterSpawnerData data = new MonsterSpawnerData(monsterName, pool, spawnData);
        // 딕셔너리에 추가
        _monsterSpawnDataDict.Add(monsterName, data);
        // MonsterSpawnData에 입력된 스폰 간격에 맞춰 몬스터를 주기적으로 스폰
        StartCoroutine(SpawnRoutine(data));
    }

    private IEnumerator SpawnRoutine(MonsterSpawnerData data)
    {
        // 스폰이 시작되기 전에는 계속 기다리기
        while (_inGameTime < data.SpawnData.SpawnStartTime)
        {
            yield return null;
        }

        // 각 데이터의 스폰 간격으로 SpawnMonster 실행
        while (_inGameTime <= data.SpawnData.SpawnEndTime)
        {
            yield return new WaitForSeconds(data.SpawnData.SpawnInterval);
            SpawnMonster(data);
        }
    }

    private void SpawnMonster(MonsterSpawnerData data)
    {
        // 카메라 외곽에서 스폰위치 찾음
        Vector3 spawnPos = GetRandomOffscreenWorldPos();
        spawnPos.y = 0.0f;

        _monsterPool = data.Pool;
        // 몬스터 풀 데이터 받아서 스폰
        for (int i = 0; i < data.Pool.Count; i++)
        {
            if (!_monsterPool[i].activeSelf)
            {
                _monsterPool[i].transform.position = spawnPos;
                _monsterPool[i].SetActive(true);
                return;
            }
        }
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
        foreach (KeyValuePair<string, MonsterSpawnerData> monsterPool in _monsterSpawnDataDict)
        {
            foreach (GameObject monster in monsterPool.Value.Pool)
            {
                if (!monster.activeSelf) continue;
                if (!monster.gameObject.GetComponent<Collider>().enabled) continue;

                // 플레이어와 몬스터의 거리 구하기
                float dist = Vector3.Distance(pos, monster.transform.position);

                // 더 작으면 갱신
                if (dist < minDistance)
                {
                    minDistance = dist;
                    closest = monster;
                }
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