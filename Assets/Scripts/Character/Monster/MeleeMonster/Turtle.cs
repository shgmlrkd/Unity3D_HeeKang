using System.Linq;
using UnityEngine;

public class Turtle : MeleeMonster
{
    private Renderer[] _turleMaterial; 
    private ParticleSystem[] _shieldParticles;

    private Vector3 _playerPosAtHit; // 맞았을 떄 플레이어 위치 변수
    private Vector3 _directionToPlayer; // 플레이어로 향하는 방향벡터 변수
    private Vector3 _dashTargetPos; // 돌진 목표 위치

    private Color _startColor = Color.white; // 흰색
    private Color _targetColor = Color.red; // 빨간색

    private readonly float _rageEndDistance = 1.0f;
    private readonly float _rageSpeedRate = 5.0f;

    private float _targetDistance;
    private float _particalLifeTime;
    private float _colorLerpTime = 1.0f;
    private float _colorLerpTimer = 0.0f;
    private float _distanceOffset = 4.0f;

    private bool _isRagePrepared = false; // 분노했는지 체크하는 변수
    private bool _getDamaged = false; // 데미지 입었는지 체크하는 변수
    private bool _isNoDamage = false; // 무적상태 체크하는 변수
    private bool _isRedColor = false; // 빨간색인지 체크하는 변수
    private bool _isReached = false; // 플레이어가 있었던 위치까지 도달했는지 체크하는 변수

    private int _turtleKey = 103;

    private void OnEnable()
    {
        base.OnEnable();

        _colorLerpTimer = 0.0f;

        // bool 변수 초기화
        _isRagePrepared = false;
        _getDamaged = false;
        _isNoDamage = false;
        _isRedColor = false;
        _isReached = false;

        // 혹시 모르니 원래 색상 세팅
        if (_turleMaterial != null)
        {
            foreach (Renderer turleMaterial in _turleMaterial)
            {
                MaterialPropertyBlock block = new MaterialPropertyBlock();
                turleMaterial.GetPropertyBlock(block);
                block.SetColor("_BaseColor", _startColor);
                turleMaterial.SetPropertyBlock(block);
            }
        }
    }

    private void Start()
    {
        base.Start();

        _turleMaterial = GetComponentsInChildren<Renderer>();

        _shieldParticles = GetComponentsInChildren<ParticleSystem>();

        // 키값에 따른 몬스터 데이터 세팅
        SetMonsterData(MonsterDataManager.Instance.GetMonsterData(_turtleKey));
    }

    private void Update()
    {
        base.Update();

        // 다시 모델의 원래색으로 바꿈
        ResetOriginalColor();

        if (_curHp <= 0) return;

        StopAttack();

        // 데미지 입으면 모델 빨간색으로 바꿈
        DamageColorEffect();
        // 무적일 때는 이 함수로 움직임
        ImmortalMove();

        Attack();

        if (CanMove() && !_isRedColor)
        {
            Move();
        }
    }

    private void ImmortalMove()
    {
        // 빨간색이고 플레이어가 있었던 위치에
        // 도달하지 못했을 경우 이 방향으로 빠르게 움직임
        if (_isRedColor && !_isReached)
        {
            transform.Translate(_directionToPlayer * _speed * _rageSpeedRate * Time.deltaTime, Space.World);

            // 일정 거리 되면 무적 해제, 분노 해제
            if (Vector3.Distance(transform.position, _dashTargetPos) <= _rageEndDistance)
            {
                _isNoDamage = false;
                _isRedColor = false;
                _isRagePrepared = false;
                _isReached = true;
                print("무적 끝, 도착");
            }
        }
    }

    private void ResetOriginalColor()
    {
        // 플레이어가 있었던 위치에 도달했다면
        if (_isReached)
        {
            _colorLerpTimer += Time.deltaTime;
            // 클램프01은 0에서 1사이로 값을 제한함
            float colorLerpTimer = Mathf.Clamp01(_colorLerpTimer / _colorLerpTime);
            // 거북이 돌격 후 원래색으로 보간
            foreach (Renderer turleMaterial in _turleMaterial)
            {
                MaterialPropertyBlock block = new MaterialPropertyBlock();
                turleMaterial.GetPropertyBlock(block);
                Color currentColor = Color.Lerp(_targetColor, _startColor, colorLerpTimer);
                block.SetColor("_BaseColor", currentColor);
                turleMaterial.SetPropertyBlock(block);
            }

            // 원래색으로 되면 초기화
            if (colorLerpTimer >= _colorLerpTime)
            {
                _isRedColor = false;
                _isReached = false;
                _colorLerpTimer = 0.0f;
                print("isreached 해제, 원래 색상으로 돌아옴");
            }
        }
    }

    private void DamageColorEffect()
    {
        // 데미지를 입었으면 그 자리에서 빨간색으로 material 색 변경
        if (_getDamaged)
        {
            _colorLerpTimer += Time.deltaTime;
            // 클램프01은 0에서 1사이로 값을 제한함
            float colorLerpTimer = Mathf.Clamp01(_colorLerpTimer / _colorLerpTime);
            // 거북이 피격 연출로 모델을 빨간색으로 보간
            foreach (Renderer turleMaterial in _turleMaterial)
            {
                MaterialPropertyBlock block = new MaterialPropertyBlock();
                turleMaterial.GetPropertyBlock(block);
                Color currentColor = Color.Lerp(_startColor, _targetColor, colorLerpTimer);
                block.SetColor("_BaseColor", currentColor);
                turleMaterial.SetPropertyBlock(block);
            }

            // 분노 상태 준비가 안되었으면 한 번만 실행
            if (!_isRagePrepared)
            {
                PrepareRageState();
                _isRagePrepared = true;  // PrepareRageState() 실행 후에는 다시 실행되지 않도록 설정
            }

            // 빨간색으로 되면 초기화
            if (colorLerpTimer >= _colorLerpTime)
            {
                _isRedColor = true;
                _getDamaged = false;
                _colorLerpTimer = 0.0f;
            }
        }
    }

    private void PrepareRageState()
    {
        _playerPosAtHit = _player.position;
        // 플레이어가 있었던 위치로 방향 벡터 구함
        _directionToPlayer = (_playerPosAtHit - transform.position).normalized;
        _directionToPlayer.y = 0.0f;
        // 돌진 할 목표 위치 설정
        _dashTargetPos = _playerPosAtHit + _directionToPlayer * _distanceOffset;
        // 플레이어가 있었던 위치 기준 거리 계산
        _targetDistance = Vector3.Distance(transform.position, _player.position);
        // 파티클 라이프 타임 구함 (_colorLerpTime 동안 그 자리에 가만히 있기 때문에 이 시간도 더함)
        _particalLifeTime = _targetDistance / (_speed * _rageSpeedRate) + _colorLerpTime;

        // 체력이 남아 있다면
        if (_curHp > 0)
        {
            // 쉴드 파티클 플레이
            foreach (ParticleSystem shieldparticle in _shieldParticles)
            {
                // 파티클 실행 시간을 설정 후 플레이
                ParticleSystem.MainModule particleLifeTime = shieldparticle.main;
                particleLifeTime.startLifetime = new ParticleSystem.MinMaxCurve(_particalLifeTime);
                shieldparticle.Play();
            }
        }
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

    protected override void MonsterGetDamage(float damage)
    {
        if (!_isNoDamage)
        {
            base.MonsterGetDamage(damage);
            // 무적 상태 On
            _isNoDamage = true;
        }

        // material이 빨간색이 아니면 true
        if (!_isRedColor)
        {
            _getDamaged = true;
        }
    }
}