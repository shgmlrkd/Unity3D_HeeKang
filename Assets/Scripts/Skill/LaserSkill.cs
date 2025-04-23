using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserSkill : Skill
{
    // HashSet�� �� �迭�� �̹� �����ϴ� �μ��� �ڵ����� �ɷ��� (�ߺ� ����)
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
        // �÷��̾� ���� ���� ���� ���͵� Collider ã��
        Collider[] targetColliders = Physics.OverlapSphere(transform.position, _weaponData.AttackRange, LayerMask.GetMask("Monster"));

        if (targetColliders.Length == 0)
            return;

        // ���� �ȿ� ���Ͱ� �� ���� �� �����Ƿ� �� ���� ���� ������
        int projectileCount = Mathf.Min(_weaponData.ProjectileCount, targetColliders.Length);
        
        // �̸� �ѹ� Ŭ����
        _selectedIndexes.Clear();

        // �ߺ����� �ʴ� ���� �ε����� projectileCount ������ŭ ���� ������ �ݺ�
        while (_selectedIndexes.Count < projectileCount)
        {
            int randomIndex = Random.Range(0, targetColliders.Length);
            _selectedIndexes.Add(randomIndex); // <- HashSet���� �ߺ� �ڵ� ����
        }

        // ���õ� ���͵鿡�� �߻�
        foreach (int index in _selectedIndexes)
        {
            Collider target = targetColliders[index];
            WeaponManager.Instance.LaserFire(target.transform.position, _weaponData);
            target.gameObject.GetComponent<Monster>().MonsterGetDamage(_weaponData.AttackPower);
        }
    }
}