using System.Linq;
using UnityEngine;

public class Turtle : MeleeMonster
{
    private ParticleSystem[] _trailParticles;

    private Vector3 _playerPosAtHit; // �¾��� �� �÷��̾� ��ġ ����
    private Vector3 _directionToPlayer; // �÷��̾�� ���ϴ� ���⺤�� ����
    private Vector3 _rushTargetPos; // ���� ��ǥ ��ġ

    private readonly float _rushEndDistance = 1.0f;
    private readonly float _rushSpeedRate = 5.0f;

    private float _particalLifeTime;
    private float _distanceOffset = 4.0f;

    private bool _isReached = false; // �÷��̾ �־��� ��ġ���� �����ߴ��� üũ�ϴ� ����
    private bool _getDamaged = false; // ������ �Ծ����� üũ�ϴ� ����
   
    private int _turtleKey = 103;

    private void OnEnable()
    {
        SetMonsterKey(_turtleKey);

        base.OnEnable();

        // bool ���� �ʱ�ȭ
        _isReached = false;
        _getDamaged = false;
    }

    private void Start()
    {
        base.Start();

        _trailParticles = GetComponentsInChildren<ParticleSystem>();
    }

    protected override void HandleRushState()
    {
        // ���� �� ������ ����
        if (!_isReached)
        {
            transform.Translate(_directionToPlayer * _monsterStatus.Speed * _rushSpeedRate * Time.deltaTime, Space.World);

            // ���� �Ÿ� �Ǹ� �ٽ� Run ����
            if (Vector3.Distance(transform.position, _rushTargetPos) <= _rushEndDistance)
            {
                _isReached = true;
                _monsterCurrentState = MonsterStatus.Run;
                _monsterAnimator.SetBool("IsReached", _isReached);
            }
        }
    }

    protected override void HandleHitState()
    {
        // �������� �Ծ�����
        if (_getDamaged)
        {
            // ���� ���� �غ� (��ġ ���)
            _isReached = false;
            SetRushState();
            _getDamaged = false;
        }
    }

    private void SetRushState()
    {
        _playerPosAtHit = _player.position;

        // �÷��̾ �־��� ��ġ ���� ���� ����
        _directionToPlayer = (_playerPosAtHit - transform.position).normalized;
        _directionToPlayer.y = 0.0f;

        // �÷��̾� �������� ȸ��
        if (_directionToPlayer != Vector3.zero)
        {
            transform.forward = _directionToPlayer;
        }

        // ���� �� ��ǥ ��ġ ����
        _rushTargetPos = _playerPosAtHit + _directionToPlayer * _distanceOffset;
        // �÷��̾ �־��� ��ġ ���� �Ÿ� ���
        float targetDistance = Vector3.Distance(transform.position, _player.position);
        // ��ƼŬ ������ Ÿ�� ����
        _particalLifeTime = targetDistance / (_monsterStatus.Speed * _rushSpeedRate);

        // ü���� ���� �ִٸ�
        if (_curHp > 0)
        {
            // ���� ��ƼŬ �÷���
            PlayParticle();
        }
        _monsterCurrentState = MonsterStatus.Rush;
    }

    private void PlayParticle()
    {
        foreach (ParticleSystem trailParticle in _trailParticles)
        {
            // ��ƼŬ�� �������ٸ�
            if (!trailParticle.isPlaying)
            {
                // ��ƼŬ ���� �ð��� ���� �� �÷���
                ParticleSystem.MainModule main = trailParticle.main;
                main.startLifetime = _particalLifeTime;
                trailParticle.Play();
            }
            else
            {
                // ��ƼŬ�� �̹� �÷��� ���ִٸ� ���� �ð��� ���� ����
                ParticleSystem.MainModule main = trailParticle.main;
                main.startLifetime = _particalLifeTime;
            }
        }
    }

    public override void MonsterGetDamage(float damage)
    {
        _getDamaged = true;
        base.MonsterGetDamage(damage);

        // ���ÿ� �¾��� ��� �ִϸ��̼��� �Ѿ�� �ʴ°� ����
        if (_curHp <= 0)
        {
            _monsterAnimator.SetBool("IsDead", true);
        }
    }
}