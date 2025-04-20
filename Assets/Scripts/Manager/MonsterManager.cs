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
        foreach (GameObject monster in _monsterPool)
        {
            if (!monster.activeSelf) continue;

            // �÷��̾�� ������ �Ÿ� ���ϱ�
            float dist = Vector3.Distance(pos, monster.transform.position);

            // �� ������ ����
            if (dist < minDistance)
            {
                minDistance = dist;
                closest = monster;
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