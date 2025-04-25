using System.Collections.Generic;
using UnityEngine;

public class AxeSkill : Skill
{
    private List<GameObject> _axes;
    private int _axeIndexKey = 315;

    private void Awake()
    {
        _axes = new List<GameObject>();
        _weaponData = WeaponDataManager.Instance.GetWeaponData(_axeIndexKey);
    }

    private void Start()
    {
        StartSpinAxe();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AxeLevelUp();
            StartSpinAxe();
        }
    }

    private void AxeLevelUp()
    {
        LevelUp();
        _weaponData = WeaponDataManager.Instance.GetWeaponData(_axeIndexKey + _level);
    }

    private void StartSpinAxe()
    {
        ClearSpinAxe();

        Vector3 center = transform.position; // �߽��� �÷��̾� ��ġ
        center.y = 0.0f;

        float radius = _weaponData.AttackRange; // ���� ������
        float angleStep = Mathf.PI * 2 / _weaponData.ProjectileCount; // ���� ��

        for (int i = 0; i < _weaponData.ProjectileCount; i++)
        {
            // ���� ������ angle����
            float angle = angleStep * i;

            // ������ �������� ��ġ ���
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;
            // ���� ��ġ
            Vector3 spawnPos = new Vector3(x, 0, z) + center;
            // �÷��̾� �������� ���� �� �����̱� ������ transform�� ����
            WeaponManager.Instance.StartAxeSpin(transform, spawnPos, _weaponData);
        }
    }

    // �ٽ� �����ϱ� ���� ��Ƽ�긦 ��
    private void ClearSpinAxe()
    {
        _axes = WeaponManager.Instance.GetObjects("Axe");

        foreach(GameObject axe in _axes)
        {
            if(axe.activeSelf)
                axe.SetActive(false);
        }
    }
}
