using UnityEngine;

public class ThrowWeapon : Weapon
{
    protected void Update()
    {
        LifeTimer();
        Move();
    }

    protected virtual void Move()
    {
        transform.Translate(Vector3.forward * _weaponSpeed * Time.deltaTime);
    }
}