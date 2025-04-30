using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MonsterManager : Singleton<MonsterManager>
{
    private Transform _player;
    private List<GameObject> _monsterPool;
    private Dictionary<string, MonsterSpawnerData> _monsterSpawnDataDict;

    private LayerMask _groundLayer;

    private float _inGameTime;
    private float _offset = 0.15f;

    private enum ScreenSide
    {
        Left, Right, Up, Down
    }

    private enum SpawnType
    {
        Normal, Circle
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

    private void Start()
    {
        // �ΰ��� �ð� �޾ƿ���
        _inGameTime = InGameUIManager.Instance.GetInGameTimer();
    }

    private void Update()
    {
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
                StartCoroutine(SpawnRoutine(data));
                break;
            case SpawnType.Circle:
                StartCoroutine(SpawnCircleRoutine(data));
                break;
        }
    }

    private IEnumerator SpawnRoutine(MonsterSpawnerData data)
    {
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
        while (_inGameTime < data.SpawnData.SpawnStartTime)
        {
            _inGameTime = InGameUIManager.Instance.GetInGameTimer();  // �ð� ����
            yield return null;
        }

        // ���� �ð����� ���͸� �������� ��ġ�ϴ� �ݺ� �ڷ�ƾ
        while (_inGameTime < data.SpawnData.SpawnEndTime)
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