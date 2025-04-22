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
        // �ΰ��� �ð��� �������� ���� InGamePlayTimer ������Ʈ�� ã��
        _inGameTimer = GameObject.Find("InGamePlayTimer");
    }

    private void Update()
    {
        // �ΰ��� �ð� �޾ƿ���
        _inGameTime = _inGameTimer.GetComponent<InGameTime>().InGameTimer;
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
        // MonsterSpawnData�� �Էµ� ���� ���ݿ� ���� ���͸� �ֱ������� ����
        StartCoroutine(SpawnRoutine(data));
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

        /*// ī�޶󿡼� pos������ �Ÿ� ��� (z ������ ����)
        float distanceFromCamera = Vector3.Distance(Camera.main.transform.position, pos);

        // ����Ʈ ��ǥ (0, 0.5): ���� �߾� / (1, 0.5): ������ �߾� + ������ ����
        // �ش� z �Ÿ� ��ġ���� ȭ�� �¿� ���� ���忡�� ������� ���
        Vector3 worldLeft = Camera.main.ViewportToWorldPoint(new Vector3(-_offset, 0.5f, distanceFromCamera));
        Vector3 worldRight = Camera.main.ViewportToWorldPoint(new Vector3(1 + _offset, 0.5f, distanceFromCamera));

        // ������ ���� �¿� ���� ��ǥ�� �Ÿ� = �ش� �Ÿ����� ī�޶� �����ִ� ���� �ʺ�
        float screenWidthInWorld = Vector3.Distance(worldLeft, worldRight);

        // ī�޶� ���� ���ߴ� ȭ���� ���� ���̸� �������� �������� ����
        float halfScreenWidth = screenWidthInWorld * 0.5f;

        // ȭ�� ������ ���͸� ã��
        Collider[] monsters = Physics.OverlapSphere(pos, halfScreenWidth, LayerMask.GetMask("Monster"));

        GameObject closest = null;
        float minDistance = float.MaxValue;

        foreach (Collider monster in monsters)
        {
            // �÷��̾�� ������ �Ÿ� ���ϱ�
            float dist = Vector3.Distance(pos, monster.transform.position);

            // �� ������ ����
            if (dist < minDistance)
            {
                minDistance = dist;
                closest = monster.gameObject;
            }
        }

        return closest;*/

        /* �׳� ���� �������� ��� ���� �༭ �ϴ� ���
            Collider[] monsters = Physics.OverlapSphere(pos, ��� ����, LayerMask.GetMask("Monster"));

            GameObject closest = null;
            float minDistance = float.MaxValue;

            foreach (Collider monster in monsters)
            {
                // �÷��̾�� ������ �Ÿ� ���ϱ�
                float dist = Vector3.Distance(pos, monster.transform.position);

                // �� ������ ����
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