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

        Vector3 center = transform.position; // 중심은 플레이어 위치
        center.y = 0.0f;

        float radius = _weaponData.AttackRange; // 도는 반지름
        float angleStep = Mathf.PI * 2 / _weaponData.ProjectileCount; // 라디안 값

        for (int i = 0; i < _weaponData.ProjectileCount; i++)
        {
            // 라디안 값으로 angle구함
            float angle = angleStep * i;

            // 각도를 기준으로 위치 계산
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;
            // 스폰 위치
            Vector3 spawnPos = new Vector3(x, 0, z) + center;
            // 플레이어 기준으로 스폰 후 움직이기 때문에 transform도 전달
            WeaponManager.Instance.StartAxeSpin(transform, spawnPos, _weaponData);
        }
    }

    // 다시 세팅하기 위해 액티브를 끔
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
