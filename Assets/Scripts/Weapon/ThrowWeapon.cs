using UnityEngine;

public class ThrowWeapon : Weapon
{
    protected void Update()
    {
        LifeTimer();
        transform.Translate(Vector3.forward * _weaponSpeed * Time.deltaTime);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            gameObject.SetActive(false);
        }
    }
}
