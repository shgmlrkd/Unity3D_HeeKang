using UnityEngine;

public class Weapon : MonoBehaviour
{
    protected Collider _weaponCollider;

    protected Vector3 _direction;

    protected readonly float _maxAlphaValue = 1.0f;
    protected readonly float _minAlphaValue = 0.0f;

    protected float _fadeLerpTime;
    protected float _weaponAttackPower = 0.0f;
    public float WeaponAttackPower
    {
        get { return _weaponAttackPower; }
    }
    protected float _timer = 0.0f;
    protected float _weaponSpeed = 0.0f;
    protected float _weaponRange = 0.0f;
    protected float _weaponPierce = 0.0f;
    protected float _weaponRotSpeed = 0.0f;
    protected float _weaponLifeTimer = 0.0f;
    protected float _weaponKnockBack = 0.0f;
    protected float _weaponProjectileCount = 0.0f;
    protected float _weaponKnockBackLerpTime = 0.0f;

    protected readonly Vector3 _spawnPosYOffset = new Vector3(0.0f, 0.5f, 0.0f);

    protected void OnEnable()
    {
        _timer = 0.0f;
    }

    protected void Start()
    {
        _weaponCollider = GetComponent<Collider>();
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

    // ����� ������ �ִ� ���� ���
    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        if (_weaponCollider is SphereCollider sphere)
        {
            Vector3 worldCenter = transform.position + sphere.center;
            float worldRadius = sphere.radius * transform.lossyScale.x;

            Gizmos.DrawWireSphere(worldCenter, worldRadius);
        }
        else if (_weaponCollider is CapsuleCollider capsule)
        {
            Vector3 worldCenter = transform.position + capsule.center;
            float worldRadius = capsule.radius * transform.lossyScale.x;

            Gizmos.DrawWireSphere(worldCenter, worldRadius);
        }
    }
}
