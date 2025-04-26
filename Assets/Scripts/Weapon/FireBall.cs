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

        // ó������ ����Ʈ�� ����
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

        // ��ƼŬ�� �÷��� ������ _weaponLifeTimer �� ������Ʈ ��Ȱ��ȭ
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
        // ���ư��� ��ƼŬ�� Ȱ��ȭ ���¸� _weaponLifeTimer ���� ��Ȱ��ȭ
        if (_fireBallParticles[(int)FireBallParticle.Flight].gameObject.activeSelf)
        {
            _timer += Time.deltaTime;

            if (_timer >= _weaponLifeTimer)
            {
                _timer -= _weaponLifeTimer;
                gameObject.SetActive(false);
            }
        }
    }

    protected override void Move()
    {
        // ���ư��� �߿��� Move
        if (!_isExplosionParticlePlay)
        { 
            transform.Translate(Vector3.forward * _weaponSpeed * Time.deltaTime);
        }
    }

    public void Fire(Vector3 pos, Vector3 dir, WeaponData data)
    {
        gameObject.SetActive(true);

        // ��ƼŬ �÷���
        foreach (ParticleSystem flightParticle in _flightParticles)
        {
            flightParticle.Play();
        }

        pos += _spawnPosYOffset;
        transform.position = pos;
        _direction = dir.normalized;
        _weaponRange = data.AttackRange;
        _weaponSpeed = data.AttackSpeed;
        _weaponLifeTimer = data.LifeTime;
        _weaponKnockBack = data.KnockBack;
        _weaponAttackPower = data.AttackPower;
        _weaponKnockBackLerpTime = data.KnockBackLerpTime;
        _isExplosionParticleTimer = _weaponLifeTimer * 0.1f; // ������ Ÿ���� 1 / 10
        _direction.y = 0.0f;

        transform.rotation = Quaternion.LookRotation(_direction);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster") && !_isExplosionParticlePlay)
        {
            _timer = 0.0f;
            _isExplosionParticlePlay = true;
            // ���ư��� ���̾ ��ƼŬ ��Ȱ��ȭ
            _fireBallParticles[(int)FireBallParticle.Flight].gameObject.SetActive(false);
            // ������ ��ƼŬ Ȱ��ȭ
            _fireBallParticles[(int)FireBallParticle.Explosion].gameObject.SetActive(true);
            // ������ ��ƼŬ �÷���
            foreach (ParticleSystem exposionParticle in _explosionParticles)
            {
                exposionParticle.Play();
            }

            // �÷��̾� ���� ���� ���� ���͵� Collider ã��
            Collider[] targetColliders = Physics.OverlapSphere(transform.position, _weaponRange, LayerMask.GetMask("Monster"));
            // ��� ���͵� ������ �ֱ�
            foreach(Collider targetCollider in targetColliders)
            {
                // ���� ����
                Monster target = targetCollider.gameObject.GetComponent<Monster>();
                // �з����� ����
                Vector3 knockBackDir = (target.transform.position - transform.position).normalized;
                knockBackDir.y = 0.0f;

                // ������ �ְ� �˹� ��Ű��
                target.MonsterGetDamage(_weaponAttackPower);
                target.MonsterKnockBack(knockBackDir, _weaponKnockBack, _weaponKnockBackLerpTime);
            }
        }
    }

    // ���� ������ �ִ� ���� ���
    protected override void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _weaponRange);
    }
}
