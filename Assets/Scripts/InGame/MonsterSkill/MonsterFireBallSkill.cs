using UnityEngine;

public class MonsterFireBallSkill : Skill
{
    public void SetMonsterWeaponData(int key)
    {
        _monsterWeaponData = MonsterWeaponDataManager.Instance.GetMonsterWeaponData(key);
        InitInterval(_monsterWeaponData);
        _attackInterval = _monsterWeaponData.AttackInterval;
    }

    public void Fire(string key, Vector3 dir)
    {
        WeaponManager.Instance.ShootMonsterFireBall(key, transform.position, dir, _monsterWeaponData);
    }

    public override void StartSkill()
    {
    }

    public override void StopSkill()
    {
    }
}