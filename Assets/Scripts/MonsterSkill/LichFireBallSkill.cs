using UnityEngine;

public class LichFireBallSkill : Skill
{
    private int _lichFireBallIndexKey = 401;

    private void Awake()
    {
        _monsterWeaponData = MonsterWeaponDataManager.Instance.GetMonsterWeaponData(_lichFireBallIndexKey);
        InitInterval(_monsterWeaponData);
        _attackInterval = _monsterWeaponData.AttackInterval;
    }

    public void Fire(Vector3 dir)
    {
        WeaponManager.Instance.ShootLichFireBall(transform.position, dir, _monsterWeaponData);
    }
}