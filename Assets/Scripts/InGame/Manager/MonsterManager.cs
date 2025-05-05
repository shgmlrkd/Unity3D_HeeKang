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
        // �ΰ��� �ð� �޾ƿ���
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
        // ���� �̸��� �´� ������ �ε� �� Ǯ������ �����
        GameObject monsterPrefab = Resources.Load<GameObject>($"Prefabs/Monster/{monsterName}");
        PoolingManager.Instance.Add(monsterName, poolSize, monsterPrefab, transform);
        List<GameObject> pool = PoolingManager.Instance.GetObjects(monsterName);
        // �� ���� ���� ���� ������ �ޱ�
        MonsterSpawnData spawnData = MonsterDataManager.Instance.GetMonsterSpawnData(monsterName);
        // �� ������ MonsterSpawnData Ŭ�������� �ʱ�ȭ
        MonsterSpawnerData data = new MonsterSpawnerData(monsterName, pool, spawnData);
        // ��ųʸ��� �߰�
        _monsterSpawnDataDict.Add(monsterName, data);

        switch((SpawnType)spawnData.Type)
        {
            case SpawnType.Normal:
                // MonsterSpawnData�� �Էµ� ���� ���ݿ� ���� ���͸� �ֱ������� ����
                data.SpawnCoroutine = StartCoroutine(SpawnRoutine(data));
                break;
            case SpawnType.Circle:
                // �������� �÷��̾ �������� �������� ����
                data.SpawnCoroutine = StartCoroutine(SpawnCircleRoutine(data));
                break;
            case SpawnType.Boss:
                // ���� �ð� ������ ����� ������� ���� ��ȯ
                data.SpawnCoroutine = StartCoroutine(SpawnBossRoutine(data));
                break;
        }
    }

    private IEnumerator SpawnBossRoutine(MonsterSpawnerData data)
    {
        while (true)
        {
            // ���� ������ �ð��̶��
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

        // insideUnitCircle�� �������� 1�� �� ���� ������ ���� ��
        Vector2 randomCircleDirection = Random.insideUnitCircle.normalized; // ���⸸ �ʿ�
        Vector3 spawnDirection = new Vector3(randomCircleDirection.x, 0.0f, randomCircleDirection.y);

        // �� �������� ���� �Ÿ���ŭ ������ ��ġ
        Vector3 spawnPos = playerPos + spawnDirection * _bossSpawnRange;
        spawnPos.y = _bossSpawnOffsetY; // Y ��ġ ����

        _monsterPool = data.Pool;
        _monsterPool[0].transform.position = spawnPos;
        _monsterPool[0].SetActive(true);
    }

    private IEnumerator SpawnRoutine(MonsterSpawnerData data)
    {
        // �پ��� Ÿ�̸��̱� ������ ������ �ݴ����
        // ������ ���۵Ǳ� ������ ��� ��ٸ���
        while (_inGameTime < data.SpawnData.SpawnStartTime)
        {
            yield return null;
        }

        // �� �������� ���� �������� SpawnMonster ����
        while (_inGameTime <= data.SpawnData.SpawnEndTime)
        {
            yield return new WaitForSeconds(data.SpawnData.SpawnInterval);
            SpawnMonster(data);
        }
    }

    private void SpawnMonster(MonsterSpawnerData data)
    {
        // ī�޶� �ܰ����� ������ġ ã��
        Vector3 spawnPos = GetRandomOffscreenWorldPos();
        spawnPos.y = 0.0f;

        _monsterPool = data.Pool;
        // ���� Ǯ ������ �޾Ƽ� ����
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
        // �پ��� Ÿ�̸��̱� ������ ������ �ݴ����
        while (_inGameTime < data.SpawnData.SpawnStartTime)
        {
            _inGameTime = InGameUIManager.Instance.GetInGameTimer();  // �ð� ����
            yield return null;
        }

        // ���� �ð����� ���͸� �������� ��ġ�ϴ� �ݺ� �ڷ�ƾ
        while (_inGameTime <= data.SpawnData.SpawnEndTime)
        {
            // ���� ��ġ ����
            SpawnCircleMonster(data);

            // ���� ���ݸ�ŭ ���
            yield return new WaitForSeconds(data.SpawnData.SpawnInterval);

            // �Ź� �ð��� ������Ʈ�ϰ� �ٽ� Ȯ��
            _inGameTime = InGameUIManager.Instance.GetInGameTimer();  // �ð� ����
        }
    }

    // ���� Ǯ ������ŭ �ѹ��� �������� �÷��̾ ����
    private void SpawnCircleMonster(MonsterSpawnerData data)
    {
        int monsterPoolCount = data.Pool.Count;
        _monsterPool = data.Pool;

        // ������ �� ���� �� ���� ��� (����)
        float angleStep = Mathf.PI * 2 / monsterPoolCount;

        for(int i = 0; i < monsterPoolCount; i++)
        {
            // ���� anlge
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

        // ȭ�� �� 4���� �� ���� ����
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

        // ���� Ǯ�� ������ Ȱ��ȭ �� �ֵ� �� ���� ������ ã��
        foreach (KeyValuePair<string, MonsterSpawnerData> monsterPool in _monsterSpawnDataDict)
        {
            foreach (GameObject monster in monsterPool.Value.Pool)
            {
                if (!monster.activeSelf) continue;
                if (!monster.gameObject.GetComponent<Collider>().enabled) continue;

                // �÷��̾�� ������ �Ÿ� ���ϱ�
                float dist = Vector3.Distance(pos, monster.transform.position);

                // �� ������ ����
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