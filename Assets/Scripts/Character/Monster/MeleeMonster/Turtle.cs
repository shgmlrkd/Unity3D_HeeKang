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

    private float _targetDistance;
    private float _particalLifeTime;
    private float _distanceOffset = 4.0f;

    private bool _isReached = false; // 플레이어가 있었던 위치까지 도달했는지 체크하는 변수
    private bool _getDamaged = false; // 데미지 입었는지 체크하는 변수
    private bool _isRushPrepared = false; // 했는지 체크하는 변수
   
    private int _turtleKey = 103;

    private void OnEnable()
    {
        SetMonsterKey(_turtleKey);

        base.OnEnable();

        // bool 변수 초기화
        _isReached = false;
        _getDamaged = false;
        _isRushPrepared = false;
    }

    private void Start()
    {
        base.Start();

        _trailParticles = GetComponentsInChildren<ParticleSystem>();
    }

    private void Update()
    {
        base.Update();

        if (_curHp <= 0) return;

        Attack();

        switch ((_monsterCurrentState))
        {
            case MonsterStatus.Run:
                if (CanMove())
                {
                    Move();
                }
                break;
            case MonsterStatus.Hit:
                // 데미지 입으면 돌진 목표 지점 설정
                SetRushTargetPos();
                break;
            case MonsterStatus.Rush:
                // 목표 지점까지 돌진
                Rush();
                break;
        }
    }

    private void Rush()
    {
        // 도착 할 때까지 돌진
        if (!_isReached)
        {
            transform.Translate(_directionToPlayer * _monsterStatus.Speed * _rushSpeedRate * Time.deltaTime, Space.World);

            // 일정 거리 되면 다시 Run 상태
            if (Vector3.Distance(transform.position, _rushTargetPos) <= _rushEndDistance)
            {
                _isRushPrepared = false;
                _isReached = true;
                _monsterCurrentState = MonsterStatus.Run;
                _monsterAnimator.SetBool("IsReached", _isReached);
            }
        }
    }

    private void SetRushTargetPos()
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
        _targetDistance = Vector3.Distance(transform.position, _player.position);
        // 파티클 라이프 타임 구함
        _particalLifeTime = _targetDistance / (_monsterStatus.Speed * _rushSpeedRate);

        // 체력이 남아 있다면
        if (_curHp > 0)
        {
           // 쉴드 파티클 플레이
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
        _monsterCurrentState = MonsterStatus.Rush;
    }

    // 몬스터 애니메이션 상태로 움직일 수 있는지 확인
    private bool CanMove()
    {
        // 애니메이션에 Base Layer를 가져온거고 Base Layer는 인덱스가 0 이어서 매개변수가 0임
        _monsterAnimStateInfo = _monsterAnimator.GetCurrentAnimatorStateInfo(0);
        bool isInDead = _monsterAnimStateInfo.IsName("Dead");
        bool isInHit = _monsterAnimStateInfo.IsName("Hit");

        // Hit이나 Dead 상태가 아니라면 true 반환
        return !(isInHit || isInDead);
    }

    public override void MonsterGetDamage(float damage)
    {
        _getDamaged = true;
        base.MonsterGetDamage(damage);

        // 동시에 맞았을 경우 애니메이션이 넘어가지 않는걸 방지
        if(_curHp <= 0)
        {
            _monsterAnimator.SetBool("IsDead", true);
        }
    }
}