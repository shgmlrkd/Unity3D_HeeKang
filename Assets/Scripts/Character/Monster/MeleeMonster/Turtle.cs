using System.Linq;
using UnityEngine;

public class Turtle : Monster
{
    private ParticleSystem[] _trailParticles;

    private Vector3 _playerPosAtHit; // 맞았을 떄 플레이어 위치 변수
    private Vector3 _directionToPlayer; // 플레이어로 향하는 방향벡터 변수

    private readonly float _rushDuration = 1.5f;
    private readonly float _rushSpeedRate = 3.0f;

    private float _rushTimer = 0.0f;
    private float _particalLifeTime;

    private bool _isReached = false; // 플레이어가 있었던 위치까지 도달했는지 체크하는 변수
   
    private int _turtleKey = 103;

    private void Awake()
    {
        base.Awake();
        // 파티클 라이프 타임 = 돌진 시간
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
        // 도착 할 때까지 돌진
        if (!_isReached)
        {
            _rushTimer += Time.deltaTime;

            transform.Translate(_directionToPlayer * _monsterStatus.Speed * _rushSpeedRate * Time.deltaTime, Space.World);
            
            // 일정 시간 동안 돌진
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
        // 데미지를 입었으면 돌진 상태 준비 (위치 잡기)
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

        // 플레이어가 있었던 위치 방향 벡터 구함
        _directionToPlayer = (_playerPosAtHit - transform.position).normalized;
        _directionToPlayer.y = 0.0f;

        // 플레이어 방향으로 회전
        if (_directionToPlayer != Vector3.zero)
        {
            transform.forward = _directionToPlayer;
        }

        // 체력이 남아 있다면
        if (_curHp > 0)
        {
            // 돌진 파티클 플레이
            PlayParticle();
        }

        _monsterCurrentState = MonsterStatus.Rush;
    }

    private void PlayParticle()
    {
        foreach (ParticleSystem trailParticle in _trailParticles)
        {
            // 파티클이 안켜졌다면
            if (!trailParticle.isPlaying)
            {
                // 파티클 실행 시간을 설정 후 플레이
                ParticleSystem.MainModule main = trailParticle.main;
                main.startLifetime = _particalLifeTime;
                trailParticle.Play();
            }
            else
            {
                // 파티클이 이미 플레이 되있다면 실행 시간을 새로 설정
                ParticleSystem.MainModule main = trailParticle.main;
                main.startLifetime = _particalLifeTime;
            }
        }
    }

    private void StopParticle()
    {
        // 파티클 멈춤
        foreach (ParticleSystem trailParticle in _trailParticles)
        {
            trailParticle.Stop();
        }
    }

    public override void MonsterGetDamage(float damage)
    {
        base.MonsterGetDamage(damage);

        // 동시에 맞았을 경우 애니메이션이 넘어가지 않는걸 방지
        if (_curHp <= 0)
        {
            _monsterAnimator.SetBool("IsDead", true);
        }
    }
}