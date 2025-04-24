using UnityEngine;

public class Weapon : MonoBehaviour
{
    protected Vector3 _direction;

    protected float _weaponAttackPower = 0.0f;
    public float WeaponAttackPower
    {
        get { return _weaponAttackPower; }
    }
    protected float _timer = 0.0f;
    protected float _weaponSpeed = 0.0f;
    protected float _weaponRange = 0.0f;
    protected float _weaponPierce = 0.0f;
    protected float _weaponLifeTimer = 0.0f;
    protected float _weaponKnockBack = 0.0f;
    protected float _weaponProjectileCount = 0.0f;

    protected readonly Vector3 _spawnPosYOffset = new Vector3(0.0f, 0.5f, 0.0f);

    protected void OnEnable()
    {
        _timer = 0.0f;
    }

    protected virtual void LifeTimer()
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
