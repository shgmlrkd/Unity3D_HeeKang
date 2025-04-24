using System.Linq;
using UnityEngine;

public class Turtle : MeleeMonster
{
    private ParticleSystem[] _trailParticles;

    private Vector3 _playerPosAtHit; // 맞았을 떄 플레이어 위치 변수
    private Vector3 _directionToPlayer; // 플레이어로 향하는 방향벡터 변수
    private Vector3 _rushTargetPos; // 돌진 목표 위치

    private readonly float _rushEndDistance = 1.0f;
    private readonly float _rushSpeedRate = 5.0f;

    private float _particalLifeTime;
    private float _distanceOffset = 4.0f;

    private bool _isReached = false; // 플레이어가 있었던 위치까지 도달했는지 체크하는 변수
    private bool _getDamaged = false; // 데미지 입었는지 체크하는 변수
   
    private int _turtleKey = 103;

    private void OnEnable()
    {
        SetMonsterKey(_turtleKey);

        base.OnEnable();

        // bool 변수 초기화
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
        // 도착 할 때까지 돌진
        if (!_isReached)
        {
            transform.Translate(_directionToPlayer * _monsterStatus.Speed * _rushSpeedRate * Time.deltaTime, Space.World);

            // 일정 거리 되면 다시 Run 상태
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
        // 데미지를 입었으면
        if (_getDamaged)
        {
            // 돌진 상태 준비 (위치 잡기)
            _isReached = false;
            SetRushState();
            _getDamaged = false;
        }
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

        // 돌진 할 목표 위치 설정
        _rushTargetPos = _playerPosAtHit + _directionToPlayer * _distanceOffset;
        // 플레이어가 있었던 위치 기준 거리 계산
        float targetDistance = Vector3.Distance(transform.position, _player.position);
        // 파티클 라이프 타임 구함
        _particalLifeTime = targetDistance / (_monsterStatus.Speed * _rushSpeedRate);

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

    public override void MonsterGetDamage(float damage)
    {
        _getDamaged = true;
        base.MonsterGetDamage(damage);

        // 동시에 맞았을 경우 애니메이션이 넘어가지 않는걸 방지
        if (_curHp <= 0)
        {
            _monsterAnimator.SetBool("IsDead", true);
        }
    }
}