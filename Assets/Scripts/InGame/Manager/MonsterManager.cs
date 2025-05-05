using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MonsterManager : Singleton<MonsterManager>
{
    private Transform _player;
    private List<GameObject> _monsterPool;
    private Dictionary<string, MonsterSpawnerData> _monsterSpawnDataDict;

    private LayerMask _groundLayer;

    private readonly float _bossSpawnRange = 130.0f;
    private readonly float _bossSpawnOffsetY = 0.5f;
    private readonly float _monstersInactiveTime = 300.0f;
    public float InitTime
    {
        get { return _monstersInactiveTime; }
    }

    private float _inGameTime;
    private float _offset = 0.18f;

    private bool _isDeadTime = false;

    private enum ScreenSide
    {
        Left, Right, Up, Down
    }

    private enum SpawnType
    {
        Normal, Circle, Boss
    }

    private void Awake()
    {
        _monsterPool = new List<GameObject>();
        _monsterSpawnDataDict = new Dictionary<string, MonsterSpawnerData>();
        _groundLayer = LayerMask.GetMask("Ground");
    }

    private void OnEnable()
    {
        _player = InGameManager.Instance.Player.transform;
    }

    private void Update()
    {
        // 인게임 시간 받아오기
        _inGameTime = InGameUIManager.Instance.GetInGameTimer();

        /*if(Input.GetKeyDown(KeyCode.I))
        {
            SpawnBoss(_monsterSpawnDataDict["Boss"]);
            print(_monsterSpawnDataDict["Boss"].Pool[0].transform.position);
        }*/

        if(_inGameTime >= _monstersInactiveTime && !_isDeadTime)
        {
            _isDeadTime = true;
            AllMonstersInactive();
        }
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

        switch((SpawnType)spawnData.Type)
        {
            case SpawnType.Normal:
                // MonsterSpawnData에 입력된 스폰 간격에 맞춰 몬스터를 주기적으로 스폰
                data.SpawnCoroutine = StartCoroutine(SpawnRoutine(data));
                break;
            case SpawnType.Circle:
                // 원형으로 플레이어를 조여오는 패턴으로 스폰
                data.SpawnCoroutine = StartCoroutine(SpawnCircleRoutine(data));
                break;
            case SpawnType.Boss:
                // 일정 시간 지나면 잡몹들 사라지고 보스 소환
                data.SpawnCoroutine = StartCoroutine(SpawnBossRoutine(data));
                break;
        }
    }

    private IEnumerator SpawnBossRoutine(MonsterSpawnerData data)
    {
        while (true)
        {
            // 보스 스폰할 시간이라면
            if(InGameUIManager.Instance.IsBossSpawnTime())
            {
                SpawnBoss(data);
                yield break;
            }

            yield return null;
        }
    }

    private void SpawnBoss(MonsterSpawnerData data)
    {
        Vector3 playerPos = InGameManager.Instance.Player.transform.position;

        // insideUnitCircle은 반지름이 1인 원 안의 랜덤한 점을 줌
        Vector2 randomCircleDirection = Random.insideUnitCircle.normalized; // 방향만 필요
        Vector3 spawnDirection = new Vector3(randomCircleDirection.x, 0.0f, randomCircleDirection.y);

        // 이 방향으로 일정 거리만큼 떨어진 위치
        Vector3 spawnPos = playerPos + spawnDirection * _bossSpawnRange;
        spawnPos.y = _bossSpawnOffsetY; // Y 위치 설정

        _monsterPool = data.Pool;
        _monsterPool[0].transform.position = spawnPos;
        _monsterPool[0].SetActive(true);
    }

    private IEnumerator SpawnRoutine(MonsterSpawnerData data)
    {
        // 줄어드는 타이머이기 때문에 조건을 반대로함
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

    private IEnumerator SpawnCircleRoutine(MonsterSpawnerData data)
    {
        // 줄어드는 타이머이기 때문에 조건을 반대로함
        while (_inGameTime < data.SpawnData.SpawnStartTime)
        {
            _inGameTime = InGameUIManager.Instance.GetInGameTimer();  // 시간 갱신
            yield return null;
        }

        // 일정 시간마다 몬스터를 원형으로 배치하는 반복 코루틴
        while (_inGameTime <= data.SpawnData.SpawnEndTime)
        {
            // 원형 배치 실행
            SpawnCircleMonster(data);

            // 스폰 간격만큼 대기
            yield return new WaitForSeconds(data.SpawnData.SpawnInterval);

            // 매번 시간을 업데이트하고 다시 확인
            _inGameTime = InGameUIManager.Instance.GetInGameTimer();  // 시간 갱신
        }
    }

    // 몬스터 풀 개수만큼 한번에 원형으로 플레이어를 가둠
    private void SpawnCircleMonster(MonsterSpawnerData data)
    {
        int monsterPoolCount = data.Pool.Count;
        _monsterPool = data.Pool;

        // 원에서 각 몬스터 간 각도 계산 (라디안)
        float angleStep = Mathf.PI * 2 / monsterPoolCount;

        for(int i = 0; i < monsterPoolCount; i++)
        {
            // 라디안 anlge
            float angle = angleStep * i;

            float x = Mathf.Cos(angle) * data.SpawnData.SpawnRange;
            float z = Mathf.Sin(angle) * data.SpawnData.SpawnRange;

            Vector3 newPos = new Vector3(x, 0.0f, z) + _player.position;

            if (!_monsterPool[i].activeSelf)
            {
                _monsterPool[i].transform.position = newPos;
                _monsterPool[i].SetActive(true);
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
    
    private void AllMonstersInactive()
    {
        foreach (KeyValuePair<string, MonsterSpawnerData> pair in _monsterSpawnDataDict)
        {
            if (pair.Key == "Boss")
                continue;

            if (pair.Value.SpawnCoroutine != null)
            {
                StopCoroutine(pair.Value.SpawnCoroutine);
            }

            foreach (GameObject monster in pair.Value.Pool)
            {
                if (monster.activeSelf)
                {
                    Renderer renderer = monster.GetComponentInChildren<Renderer>();

                    Animator anim = monster.GetComponent<Animator>();
                    anim.SetTrigger("Dead");

                    if (renderer != null)
                    {
                        Material mat = renderer.material;
                        StartCoroutine(FadeOutAndDisable(monster, mat, 1.5f));
                    }
                    else
                    {
                        monster.SetActive(false);
                    }
                }
            }
        }
    }

    private IEnumerator FadeOutAndDisable(GameObject monster, Material monsterMaterial, float duration)
    {
        Color originalColor = monsterMaterial.color;
        float time = 0.0f;

        while (time < duration)
        {
            float t = time / duration;
            Color newColor = originalColor;
            newColor.a = Mathf.Lerp(1.0f, 0.0f, t);
            monsterMaterial.color = newColor;
            monster.GetComponent<Monster>().SetMonsterHpBarAlpha(newColor.a);
            time += Time.deltaTime;
            yield return null;
        }

        Color finalColor = originalColor;
        finalColor.a = 0.0f;
        monster.GetComponent<Monster>().SetMonsterHpBarAlpha(finalColor.a);
        monsterMaterial.color = finalColor;

        monster.SetActive(false);
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
    }
}