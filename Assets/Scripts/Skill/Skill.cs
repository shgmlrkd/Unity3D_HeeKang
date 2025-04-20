using UnityEngine;

public class Skill : MonoBehaviour
{
    protected WeaponData _weaponData;

    protected int _level = 0;
    protected WaitForSeconds _fireInterval;

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
