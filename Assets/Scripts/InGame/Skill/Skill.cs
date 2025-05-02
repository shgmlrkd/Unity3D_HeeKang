using UnityEngine;

public class Skill : MonoBehaviour
{
    protected WaitForSeconds _fireInterval;
    public WaitForSeconds FireInterval
    {
        get { return _fireInterval; }
    }

    protected WeaponData _weaponData;
    protected MonsterWeaponData _monsterWeaponData;

    protected readonly float _maxAlphaValue = 1.0f;
    protected readonly float _minAlphaValue = 0.0f;

    protected float _fadeLerpTime;
    protected float _attackInterval;
    public float AttackInterval
    {
        get { return _attackInterval; }
    }

    protected int _level = 0;
    public int Level
    { 
        get { return _level; } 
    }

    protected void InitInterval(WeaponData weaponData)
    {
        _fireInterval = new WaitForSeconds(weaponData.AttackInterval);
    }
    protected void InitInterval(MonsterWeaponData weaponData)
    {
        _fireInterval = new WaitForSeconds(weaponData.AttackInterval);
    }

    public virtual void LevelUp()
    {
        _level++;

        if (_level > 4)
            _level = 4;
    }
}
