using UnityEngine;

public class Axe : Weapon
{
    private float _rotSpeed = 25.0f;

    private void Update()
    {
        transform.Rotate(Vector3.up * _rotSpeed * Time.deltaTime);
    }

    public void SpinAround(Vector3 pos, WeaponData data)
    {
        gameObject.SetActive(true);

        pos += _spawnPosYOffset;
        transform.position = pos;
        _weaponRange = data.AttackRange;
        _weaponSpeed = data.AttackSpeed;
        _weaponKnockBack = data.Knockback; 
        _weaponAttackPower = data.AttackPower;
    }
}