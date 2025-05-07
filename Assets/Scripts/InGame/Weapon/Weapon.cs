using UnityEngine;

public class Weapon : MonoBehaviour
{
    protected Collider _weaponCollider;

    protected Color _color = Color.white;
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

    protected void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster") || other.CompareTag("Boss"))
        {
            SoundManager.Instance.PlayFX(SoundKey.NormalWeaponHitSound, 0.04f);
            DamageTextManager.Instance.ShowDamageText(other.transform, _weaponAttackPower, _color);
        }
    }

    protected void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Monster") || other.CompareTag("Boss"))
        {
            DamageTextManager.Instance.ShowDamageText(other.transform, _weaponAttackPower, _color);
        }
    }

    // 무기들 데미지 주는 범위 출력
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
