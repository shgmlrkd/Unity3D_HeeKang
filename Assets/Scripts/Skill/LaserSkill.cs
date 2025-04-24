using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserSkill : Skill
{
    // HashSet은 이 배열에 이미 존재하는 인수는 자동으로 걸러짐 (중복 제거)
    private HashSet<int> _selectedIndexes;

    private int _laserIndexKey = 326;

    private void Awake()
    {
        _selectedIndexes = new HashSet<int>();
        _weaponData = WeaponDataManager.Instance.GetWeaponData(_laserIndexKey);
        InitInterval(_weaponData);
    }

    private void Start()
    {
        StartCoroutine(FireLoop());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            LaserLevelUp();
        }
    }

    private void LaserLevelUp()
    {
        LevelUp();
        _weaponData = WeaponDataManager.Instance.GetWeaponData(_laserIndexKey + _level);
        InitInterval(_weaponData);
    }

    private IEnumerator FireLoop()
    {
        while (true)
        {
            Fire();

            yield return _fireInterval;
        }
    }

    private void Fire()
    {
        // 플레이어 기준 범위 내의 몬스터들 Collider 찾기
        Collider[] targetColliders = Physics.OverlapSphere(transform.position, _weaponData.AttackRange, LayerMask.GetMask("Monster"));

        if (targetColliders.Length == 0)
            return;

        // 범위 안에 몬스터가 더 적을 수 있으므로 더 작은 수를 가져옴
        int projectileCount = Mathf.Min(_weaponData.ProjectileCount, targetColliders.Length);
        
        // 미리 한번 클리어
        _selectedIndexes.Clear();

        // 중복되지 않는 랜덤 인덱스를 projectileCount 개수만큼 뽑을 때까지 반복
        while (_selectedIndexes.Count < projectileCount)
        {
            int randomIndex = Random.Range(0, targetColliders.Length);
            _selectedIndexes.Add(randomIndex); // <- HashSet으로 중복 자동 제거
        }

        // 선택된 몬스터들에게 발사
        foreach (int index in _selectedIndexes)
        {
            Collider target = targetColliders[index];
            WeaponManager.Instance.LaserFire(target.transform.position, _weaponData);
            target.gameObject.GetComponent<Monster>().MonsterGetDamage(_weaponData.AttackPower);
        }
    }
}