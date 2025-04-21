using UnityEngine;

public class ThrowWeapon : Weapon
{
    protected void Update()
    {
        LifeTimer();
        transform.Translate(Vector3.forward * _weaponSpeed * Time.deltaTime);
    }
}