using UnityEngine;

public class Sword : ThrowWeapon
{
    private void Update()
    {
        transform.Rotate(Vector3.forward * _weaponRotSpeed * Time.deltaTime);
    }

    public void Fire(Vector3 pos, Vector3 dir, WeaponData data)
    {
        gameObject.SetActive(true);

        pos += _spawnPosYOffset;
        transform.position = pos;
        _direction = dir;
        _weaponRange = data.AttackRange;
        _weaponSpeed = data.AttackSpeed;
        _weaponLifeTimer = data.LifeTime;
        _weaponAttackPower = data.AttackPower;
        _direction.y = 0.0f;

        transform.rotation = Quaternion.LookRotation(_direction);
    }
}