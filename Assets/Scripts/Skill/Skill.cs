using UnityEngine;

public class Skill : MonoBehaviour
{
    protected WaitForSeconds _fireInterval;

    protected WeaponData _weaponData;

    protected readonly float _maxAlphaValue = 1.0f;
    protected readonly float _minAlphaValue = 0.0f;

    protected float _fadeLerpTime;

    protected int _level = 0;

    public void InitInterval(WeaponData weaponData)
    {
        _fireInterval = new WaitForSeconds(weaponData.AttackInterval);
    }

    public void LevelUp()
    {
        _level++;

        if (_level > 4)
            _level = 4;
    }
}
