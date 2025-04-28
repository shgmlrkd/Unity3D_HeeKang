using System.Linq;
using UnityEngine;

public class Turtle : Monster
{
    private ParticleSystem[] _trailParticles;

    private Vector3 _playerPosAtHit; // �¾��� �� �÷��̾� ��ġ ����
    private Vector3 _directionToPlayer; // �÷��̾�� ���ϴ� ���⺤�� ����

    private readonly float _rushDuration = 1.5f;
    private readonly float _rushSpeedRate = 3.0f;

    private float _rushTimer = 0.0f;
    private float _particalLifeTime;

    private bool _isReached = false; // �÷��̾ �־��� ��ġ���� �����ߴ��� üũ�ϴ� ����
   
    private int _turtleKey = 103;

    private void Awake()
    {
        base.Awake();
        // ��ƼŬ ������ Ÿ�� = ���� �ð�
        _particalLifeTime = _rushDuration;
    }

    private void OnEnable()
    {
        SetMonsterKey(_turtleKey);

        base.OnEnable();

        _rushTimer = 0.0f;

        _isReached = false;
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
            _rushTimer += Time.deltaTime;

            transform.Translate(_directionToPlayer * _monsterStatus.Speed * _rushSpeedRate * Time.deltaTime, Space.World);
            
            // ���� �ð� ���� ����
            if (_rushTimer >= _rushDuration)
            {
                _isReached = true;
                _rushTimer -= _rushDuration;
                _monsterCurrentState = MonsterStatus.Run;
                _monsterAnimator.SetBool("IsReached", _isReached);
            }
        }
    }

    protected override void HandleHitState()
    {
        // �������� �Ծ����� ���� ���� �غ� (��ġ ���)
        _isReached = false;
        SetRushState();
    }

    protected override void HandleDeadState()
    {
        StopParticle();
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

    private void StopParticle()
    {
        // ��ƼŬ ����
        foreach (ParticleSystem trailParticle in _trailParticles)
        {
            trailParticle.Stop();
        }
    }

    public override void MonsterGetDamage(float damage)
    {
        base.MonsterGetDamage(damage);

        // ���ÿ� �¾��� ��� �ִϸ��̼��� �Ѿ�� �ʴ°� ����
        if (_curHp <= 0)
        {
            _monsterAnimator.SetBool("IsDead", true);
        }
    }
}