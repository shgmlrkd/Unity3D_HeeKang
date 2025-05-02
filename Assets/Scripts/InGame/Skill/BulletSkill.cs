using System.Collections;
using UnityEngine;

public class BulletSkill : Skill
{
    private int _bulletIndexKey = 300;

    private void Awake()
    {
        _weaponData = WeaponDataManager.Instance.GetWeaponData(_bulletIndexKey);
        InitInterval(_weaponData);
    }

    void Start()
    {
        _fireCoroutine = StartCoroutine(FireLoop());
    }

    public override void LevelUp()
    {
        base.LevelUp();

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

        WeaponManager.Instance.BulletFire(transform.position, dir, _weaponData);
    }

    public override void StartSkill()
    {
        if (_fireCoroutine == null)
            _fireCoroutine = StartCoroutine(FireLoop());
    }

    public override void StopSkill()
    {
        if (_fireCoroutine != null)
        {
            StopCoroutine(_fireCoroutine);
            _fireCoroutine = null;
        }
    }
}