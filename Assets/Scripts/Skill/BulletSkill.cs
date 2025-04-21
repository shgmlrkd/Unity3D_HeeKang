using System.Collections;
using UnityEngine;

public class BulletSkill : Skill
{
    private int _bulletIndexKey = 301;

    private void Awake()
    {
        _weaponData = WeaponDataManager.Instance.GetWeaponData(_bulletIndexKey);
        InitInterval(_weaponData);
    }

    void Start()
    {
        StartCoroutine(FireLoop());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            BulletLevelUp();
        }
    }

    private void BulletLevelUp()
    {
        LevelUp();
        _weaponData = WeaponDataManager.Instance.GetWeaponData(_bulletIndexKey + _level);
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
        GameObject target = MonsterManager.Instance.GetClosestMonster(transform.position);

        if (target == null)
            return;

        Vector3 dir = target.transform.position - transform.position;

        print(dir.x + " " + dir.y + " " + dir.z);

        WeaponManager.Instance.BulletFire(transform.position, dir, _weaponData);
    }
}
