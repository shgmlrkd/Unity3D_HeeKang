using System.Collections;
using UnityEngine;

public class SwordSkill : Skill
{
    private int _swordIndexKey = 310;

    private void Awake()
    {
        _weaponData = WeaponDataManager.Instance.GetWeaponData(_swordIndexKey);
        InitInterval(_weaponData);
    }

    void Start()
    {
        StartCoroutine(FireLoop());
    }

    public override void LevelUp()
    {
        base.LevelUp();
        _weaponData = WeaponDataManager.Instance.GetWeaponData(_swordIndexKey + _level);
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

        Vector3 dir = (target.transform.position - transform.position).normalized;

        WeaponManager.Instance.ThrowSpinningSword(transform.position, dir, _weaponData);
    }
}
