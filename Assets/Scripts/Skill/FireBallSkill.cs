using System.Collections;
using UnityEngine;

public class FireBallSkill : Skill
{
    private int _kunaiIndexKey = 321;

    private void Awake()
    {
        _weaponData = WeaponDataManager.Instance.GetWeaponData(_kunaiIndexKey);
        InitInterval(_weaponData);
    }

    private void Start()
    {
        StartCoroutine(FireLoop());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            FireBallLevelUp();
        }
    }

    private void FireBallLevelUp()
    {
        LevelUp();
        _weaponData = WeaponDataManager.Instance.GetWeaponData(_kunaiIndexKey + _level);
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

        WeaponManager.Instance.ShootFireBall(transform.position, dir, _weaponData);
    }
}
