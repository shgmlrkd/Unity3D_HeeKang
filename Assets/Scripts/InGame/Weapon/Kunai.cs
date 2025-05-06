using UnityEngine;

public class Kunai : ThrowWeapon
{
    private int _pierce = 0;

    private void OnEnable()
    {
        base.OnEnable();
        _pierce = 0;
    }

    public void Fire(Vector3 pos, Vector3 dir, WeaponData data)
    {
        gameObject.SetActive(true);

        pos += _spawnPosYOffset;
        transform.position = pos;
        _direction = dir;
        _weaponSpeed = data.AttackSpeed;
        _weaponAttackPower = data.AttackPower;
        _weaponLifeTimer = data.LifeTime;
        _weaponPierce = data.Pierce;
        _direction.y = 0.0f;

        transform.rotation = Quaternion.LookRotation(_direction);
    }

    private void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if(other.CompareTag("Monster"))
        {
            if ( _pierce == _weaponPierce)
            {
                gameObject.SetActive(false);
            }

            _pierce++;
        }

        if(other.CompareTag("Boss"))
        {
            gameObject.SetActive(false);
        }
    }
    protected new void OnTriggerStay(Collider other)
    {
    }
}