using UnityEngine;

public class Weapon : MonoBehaviour
{
    protected Vector3 _direction;

    protected float _weaponAttackPower = 0.0f;
    public float WeaponAttackPower
    {
        get { return _weaponAttackPower; }
    }
    protected float _weaponSpeed = 0.0f;
    protected float _weaponLifeTimer = 0.0f;
    protected float _timer = 0.0f;
    protected float _weaponPierce = 0.0f;
    protected float _weaponProjectileCount = 0.0f;

    protected void LifeTimer()
    {
        if (gameObject.activeSelf)
        {
            _timer += Time.deltaTime;

            if (_timer >= _weaponLifeTimer)
            {
                _timer -= _weaponLifeTimer;
                gameObject.SetActive(false);
            }
        }
    }
}
