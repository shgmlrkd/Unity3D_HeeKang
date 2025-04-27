using UnityEngine;

public class LichFireBallSkill : Skill
{
    private int _lichFireBallIndexKey = 401;

    private void Awake()
    {
        _monsterWeaponData = MonsterWeaponDataManager.Instance.GetMonsterWeaponData(_lichFireBallIndexKey);
        InitInterval(_monsterWeaponData);
    }

    public void Fire(Vector3 dir)
    {
        WeaponManager.Instance.ShootLichFireBall(transform.position, dir, _monsterWeaponData);
    }
}