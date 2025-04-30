using UnityEngine;

public class MonsterFireBallSkill : Skill
{
    [SerializeField]
    private int _monsterFireBallIndexKey;

    private void Awake()
    {
        _monsterWeaponData = MonsterWeaponDataManager.Instance.GetMonsterWeaponData(_monsterFireBallIndexKey);
        InitInterval(_monsterWeaponData);
        _attackInterval = _monsterWeaponData.AttackInterval;
    }

    public void Fire(Vector3 dir)
    {
        WeaponManager.Instance.ShootLichFireBall(transform.position, dir, _monsterWeaponData);
    }
}