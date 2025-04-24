using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class FireBall : ThrowWeapon
{
    private enum FireBallParticle
    {
        Flight, Explosion
    }

    private List<ParticleSystem> psList;
    private ParticleSystem[] _fireBallParticles;
    private ParticleSystem[] _flightParticles;
    private ParticleSystem[] _explosionParticles;

    private float _isExplosionParticleTimer;

    private bool _isExplosionParticlePlay = false;
    private bool _isParticleSystemListAdded = false;

    private void Awake()
    {
        psList = new List<ParticleSystem>();
    }

    private void OnEnable()
    {
        base.OnEnable();

        // 처음에만 리스트에 넣음
        if (!_isParticleSystemListAdded)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                ParticleSystem particleStstem = transform.GetChild(i).GetComponent<ParticleSystem>();
                psList.Add(particleStstem);
            }
            _isParticleSystemListAdded = true;
            _fireBallParticles = psList.ToArray();
        }

        _fireBallParticles[(int)FireBallParticle.Flight].gameObject.SetActive(true);
        _fireBallParticles[(int)FireBallParticle.Explosion].gameObject.SetActive(false);

        _flightParticles = _fireBallParticles[(int)FireBallParticle.Flight].GetComponentsInChildren<ParticleSystem>();
        _explosionParticles = _fireBallParticles[(int)FireBallParticle.Explosion].GetComponentsInChildren<ParticleSystem>();

        _isExplosionParticlePlay = false;
    }

    private void Update()
    {
        base.Update();

        // 파티클을 플레이 했으면 _weaponLifeTimer 후 오브젝트 비활성화
        if (_isExplosionParticlePlay)
        {
            _timer += Time.deltaTime;

            if (_timer >= _isExplosionParticleTimer)
            {
                gameObject.SetActive(false);
            }
        }
    }

    protected override void LifeTimer()
    {
        // 날아가는 파티클이 활성화 상태면 _weaponLifeTimer 이후 비활성화
        if (_fireBallParticles[(int)FireBallParticle.Flight].gameObject.activeSelf)
        {
            _timer += Time.deltaTime;

            if (_timer >= _weaponLifeTimer)
            {
                _timer -= _weaponLifeTimer;
                _fireBallParticles[(int)FireBallParticle.Flight].gameObject.SetActive(false);
            }
        }
    }

    public void Fire(Vector3 pos, Vector3 dir, WeaponData data)
    {
        gameObject.SetActive(true);

        // 파티클 플레이
        foreach (ParticleSystem flightParticle in _flightParticles)
        {
            flightParticle.Play();
        }

        pos += _spawnPosYOffset;
        transform.position = pos;
        _direction = dir.normalized;
        _weaponRange = data.AttackRange;
        _weaponSpeed = data.AttackSpeed;
        _weaponKnockBack = data.Knockback;
        _weaponAttackPower = data.AttackPower;
        _weaponLifeTimer = data.LifeTime;
        _isExplosionParticleTimer = _weaponLifeTimer * 0.1f; // 라이프 타임의 1 / 10
        _direction.y = 0.0f;

        transform.rotation = Quaternion.LookRotation(_direction);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster") && !_isExplosionParticlePlay)
        {
            _timer = 0.0f;
            _isExplosionParticlePlay = true;
            // 날아가는 파이어볼 파티클 비활성화
            _fireBallParticles[(int)FireBallParticle.Flight].gameObject.SetActive(false);
            // 터지는 파티클 활성화
            _fireBallParticles[(int)FireBallParticle.Explosion].gameObject.SetActive(true);
            // 터지는 파티클 플레이
            foreach (ParticleSystem exposionParticle in _explosionParticles)
            {
                exposionParticle.Play();
            }

            // 플레이어 기준 범위 내의 몬스터들 Collider 찾기
            Collider[] targetColliders = Physics.OverlapSphere(transform.position, _weaponRange, LayerMask.GetMask("Monster"));
            // 모든 몬스터들 데미지 주기
            foreach(Collider targetCollider in targetColliders)
            {
                targetCollider.gameObject.GetComponent<Monster>().MonsterGetDamage(_weaponAttackPower);
            }

            // 파이어 볼은 넉백도 있어서 해줘야함
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _weaponRange);
    }
}
