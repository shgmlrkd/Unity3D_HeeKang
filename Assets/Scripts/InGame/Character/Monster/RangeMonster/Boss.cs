using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : FlashDamagedMonster
{
    private enum BossState
    {
        Idle, Run, Attack, Rush, Intro, Dead, Roar, None
    }

    private enum BossAttack
    {
        BurstFireBall, SpinFireBall, BossAttackCount
    }

    private PlayerMove _playerMove;
    private PlayerSkill _playerSkill;
    private Coroutine _attackRoutine = null;
    private MonsterFireBallSkill _monsterFireBallSkill;

    private Vector3 _direction = new Vector3();

    private BossState _bossState = BossState.Intro;

    private readonly float _toPercent = 100.0f;
    private readonly float _thirty = 30.0f;
    private readonly float _introDuration = 2.0f;
    private readonly float _idleDuration = 1.0f;
    private readonly float _runDuration = 4.0f;
    private readonly float _rushDuration = 2.5f;
    private readonly float _roarDuration = 2.0f;
    private readonly float _attackDuration = 2.7f;
    private readonly float _spinFireInterval = 0.01f;
    private readonly float _shootFireInterval = 0.73f;
    private readonly float _spinRotDuration = 0.3f; // Spin Attack 할 때 부드러운 회전 시간
    private readonly float _slowMotionDuration = 1.8f;
    private readonly float _fireballAngleOffset = 120.0f;

    private float _timer = 0.0f;
    private float _rushSpeed = 0.0f;

    private readonly int _bossKey = 106;
    private readonly int _bossWeaponKey = 402;
    private readonly int _burstFireBallCount = 20;
    private readonly int _spinFireBallCount = 40;

    private int[] _bossStateTracker = new int[4];

    private bool _isRoar = false;
    public bool IsBossAngry
    {
        get { return _isRoar; }
    }
    private bool _isIntroEnd = false;
    private bool _isIdleState = false;
    private bool _isRunState = false;
    private bool _isRushState = false;
    private bool _isAttackState = false;
    private bool _isBossDead = false;
    private bool _isBossRoar = false;
    private bool _isRoarEnd = false;

    private void Awake()
    {
        base.Awake();

        _flashColor = Color.red;
    }

    private void Start()
    {
        base.Start();

        _playerMove = InGameManager.Instance.Player.GetComponent<PlayerMove>();
        _playerSkill = _player.gameObject.GetComponent<PlayerSkill>();
        _rushSpeed = _monsterStatus.Speed * 2.8f;
        _monsterFireBallSkill = GetComponent<MonsterFireBallSkill>();
        _monsterFireBallSkill.SetMonsterWeaponData(_bossWeaponKey);
    }

    private void OnEnable()
    {
        SetMonsterKey(_bossKey);

        base.OnEnable();
    }

    protected override void Action()
    {
        // 보스 체력 30% 이하
        if (_curHp / _maxHp * _toPercent <= _thirty && !_isRoar)
        {
            _isRoar = true;
            _bossState = BossState.Roar;
        }

        switch(_bossState)
        {
            case BossState.Intro:
                _monsterCurrentState = MonsterStatus.None;
                Intro();
                break;
            case BossState.Idle:
                _monsterCurrentState = MonsterStatus.Idle;
                HandleIdleState();
                break;
            case BossState.Run:
                _monsterCurrentState = MonsterStatus.Run;
                if (CanMove())
                {
                    Move();
                }
                break;
            case BossState.Attack:
                _monsterCurrentState = MonsterStatus.Attack;
                HandleAttackState();
                break;
            case BossState.Rush:
                _monsterCurrentState = MonsterStatus.Rush;
                HandleRushState();
                break;
            case BossState.Dead:
                HandleDeadState();
                break;
            case BossState.Roar:
                _monsterCurrentState = MonsterStatus.None;
                HandleRoarState();
                break;
            case BossState.None:
                _monsterCurrentState = MonsterStatus.None;
                break;
        }
    }

    private void Intro()
    {
        Vector3 direction = (_player.position - transform.position).normalized;
        direction.y = 0.0f;

        float distance = Vector3.Distance(_player.position, transform.position);

        // 플레이어와 일정거리가 될 때까지 이동
        if (distance >= _monsterStatus.Range)
        {
            transform.Translate(direction * _monsterStatus.Speed * Time.deltaTime, Space.World);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * _monsterStatus.RotSpeed);
        }
        else
        {
            // 범위에 도달하면 인트로 상태를 true로 전환
            _isIntroEnd = true;
        }

        if (_isIntroEnd)
        {
            _timer += Time.deltaTime;

            if (_timer >= _introDuration)
            {
                _timer = 0.0f;
                _bossState = BossState.Attack;
            }
        }
    }

    private void HandleIdleState()
    {
        if (!_isIdleState)
        {
            _isIdleState = true;
            // Idle 애니메이션 실행
            if (_isRoar)
            {
                _isRoarEnd = true;
                _monsterAnimator.SetTrigger("AngryIdle");
            }
            else
            {
                _monsterAnimator.SetTrigger("Idle");
            }
            // Idle 실행 bossStateTracker = [1, 0, 0, 0]
            _bossStateTracker[(int)BossState.Idle]++;
        }

        _timer += Time.deltaTime;

        if(_timer >= _idleDuration)
        {
            _timer = 0.0f;
            
            TransitionFromState((int)BossState.Idle);
        }
    }

    protected override void Move()
    {
        if(!_isRunState)
        {
            _isRunState = true;
            if (_isRoar)
            {
                _monsterAnimator.speed = 1.5f;
            }
            _monsterAnimator.SetTrigger("Run");
            // Run 실행 bossStateTracker = [0, 1, 0, 0]
            _bossStateTracker[(int)BossState.Run]++;
        }

        // 플레이어 방향으로 이동, 회전
        Vector3 direction = (_player.position - transform.position).normalized;
        direction.y = 0;

        transform.Translate(direction * _monsterStatus.Speed * Time.deltaTime, Space.World);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * _monsterStatus.RotSpeed);
       
        _timer += Time.deltaTime;

        if (_timer >= _runDuration)
        {
            _timer = 0.0f;
            _monsterAnimator.speed = 1.0f;
            TransitionFromState((int)BossState.Run);
        }
    }

    protected override void HandleRushState()
    {
        if (!_isRushState)
        {
            _isRushState = true;
            _monsterAnimator.SetTrigger("Rush");
            SoundManager.Instance.PlayFX(SoundKey.BossRushSound, 0.025f);
            // Rush 실행 bossStateTracker = [0, 0, 0, 1]
            _bossStateTracker[(int)BossState.Rush]++;

            // 이때 저장한 방향으로
            _direction = (_player.position - transform.position).normalized;
            _direction.y = 0.0f;
        }

        // 회전
        if (_direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(_direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _monsterStatus.RotSpeed); // 5는 회전 속도
        }

        // 이동
        transform.Translate(_direction * _rushSpeed * Time.deltaTime, Space.World);

        _timer += Time.deltaTime;

        if (_timer >= _rushDuration)
        {
            _timer = 0.0f;
            TransitionFromState((int)BossState.Rush);
        }
    }

    protected override void HandleAttackState()
    {
        if (!_isAttackState)
        {
            _isAttackState = true;
            // Attack 실행 bossStateTracker = [0, 0, 1, 0]
            _bossStateTracker[(int)BossState.Attack]++;

            // 보스 fireball 가져오기
            List<GameObject> fireballs = WeaponManager.Instance.GetObjects("BossFireBall");

            int randomAttack = Random.Range(0, (int)BossAttack.BossAttackCount);

            switch((BossAttack)randomAttack)
            {
                case BossAttack.BurstFireBall:
                    if (_attackRoutine == null)
                        _attackRoutine = StartCoroutine(FireballSequence(fireballs));
                    break;
                case BossAttack.SpinFireBall:
                    if (_attackRoutine == null)
                        _attackRoutine = StartCoroutine(SpinFireballSequence(fireballs));
                    break;
            }
        }

        if (_attackRoutine == null)
        {
            _timer += Time.deltaTime;

            if (_timer >= _attackDuration)
            {
                _timer = 0.0f;

                TransitionFromState((int)BossState.Attack);
            }
        }
    }

    // 360도 방향으로 일정 수의 파이어볼을 동시에 발사하는 함수
    private void ShootFireballsInCircle(List<GameObject> fireBalls, float offset = 0.0f)
    {
        _monsterAnimator.SetTrigger("BurstFire");
        SoundManager.Instance.PlayFX(SoundKey.BossAttackSound, 0.04f);

        // fireball 중 20개만 발사하기 위해 각도 나누기
        float angleStep = Mathf.PI * 2 / _burstFireBallCount;

        // 20개를 360도 각 방향으로 발사
        int fireBallCount = 0;

        foreach (GameObject fireball in fireBalls)
        {
            if (!fireball.activeSelf)
            {
                float angle = offset + angleStep * fireBallCount;

                float x = Mathf.Cos(angle);
                float z = Mathf.Sin(angle);

                Vector3 dir = new Vector3(x, 0.0f, z).normalized;

                _monsterFireBallSkill.Fire("BossFireBall", dir);

                fireBallCount++;
                if (fireBallCount >= _burstFireBallCount)
                    break;
            }
        }
    }

    private IEnumerator FireballSequence(List<GameObject> fireBalls)
    {
        if (!_isRoar)
        {
            // 포효 전 fireball 발사
            _monsterAnimator.SetTrigger("BurstFire");
            ShootFireballsInCircle(fireBalls);  // 포효 전 발사
            yield return null;
        }
        else
        {
            float angleStep = Mathf.PI * 2 / _burstFireBallCount;

            // 포효 후 연속해서 fireball 두 번 발사
            for (int i = 0; i < 2; i++)
            { 
                if(_curHp <= 0.0f)
                {
                    _attackRoutine = null;
                    yield break;
                }

                _monsterAnimator.SetTrigger("BurstFire");
                float offset = (i == 1) ? angleStep * 0.5f : 0.0f;
                ShootFireballsInCircle(fireBalls, offset);
                yield return new WaitForSeconds(_shootFireInterval);  // 두 번째 발사 사이에 잠시 대기
            }
        }

        _attackRoutine = null;
    }

    // 540도 방향으로 일정 수의 파이어볼을 딜레이를 줘서 발사하는 함수
    private IEnumerator SpinFireballSequence(List<GameObject> fireBalls)
    {
        _monsterAnimator.SetTrigger("DelayFireStart");

        // fireball 중 40개만 발사하기 위해 각도 나누기
        float angleStep = Mathf.PI * 3 / _spinFireBallCount;

        Vector3 playerPosition = _player.transform.position;

        int fireBallCount = 0;

        foreach (GameObject fireball in fireBalls)
        {
            if (!fireball.activeSelf)
            {
                // 플레이어 방향으로 방향 벡터를 구함
                Vector3 directionToPlayer = (playerPosition - transform.position).normalized;
                
                // 플레이어 방향 기준 각도 계산
                float startAngle = Mathf.Atan2(directionToPlayer.z, directionToPlayer.x); 

                float angle = startAngle + angleStep * fireBallCount;

                float x = Mathf.Cos(angle);
                float z = Mathf.Sin(angle);

                Vector3 dir = new Vector3(x, 0.0f, z).normalized;

                // 지금 위치에서 targetRot까지 보간을 이용해서 회전
                Quaternion startRot = transform.rotation;
                Quaternion targetRot = Quaternion.LookRotation(dir);

                float lerpTimer = 0.0f;
                
                while (lerpTimer < _one)
                {
                    lerpTimer += Time.deltaTime / _spinRotDuration;
                    transform.rotation = Quaternion.Lerp(startRot, targetRot, lerpTimer);
                    yield return null;
                }

                _monsterFireBallSkill.Fire("BossFireBall", dir);
                SoundManager.Instance.PlayFX(SoundKey.BossAttackSound, 0.02f);

                // 보스가 포효를 한다면 코루틴도 멈춤
                if ((!_isRoarEnd && _isBossRoar) || _curHp <= 0.0f)
                {
                    _attackRoutine = null;
                    _isAttackState = false;
                    _bossStateTracker[(int)BossState.Attack] = 0;
                    yield break;
                }

                if (_isRoar)
                {
                    // 좌우 방향 (±120도 회전)
                    float angleOffset = _fireballAngleOffset * Mathf.Deg2Rad;

                    Vector3 leftDir = new Vector3(Mathf.Cos(angle - angleOffset), 0.0f, Mathf.Sin(angle - angleOffset)).normalized;
                    Vector3 rightDir = new Vector3(Mathf.Cos(angle + angleOffset), 0.0f, Mathf.Sin(angle + angleOffset)).normalized;

                    _monsterFireBallSkill.Fire("BossFireBall", leftDir);
                    _monsterFireBallSkill.Fire("BossFireBall", rightDir);
                }

                fireBallCount++;
                if (fireBallCount >= _spinFireBallCount)
                {
                    _attackRoutine = null;
                    _monsterAnimator.SetTrigger("DelayFireEnd");
                    _monsterAnimator.SetBool("IsAngry", _isRoar);
                    yield break;
                }

                yield return new WaitForSeconds(_spinFireInterval);
            }
        }
    }
    
    private void HandleRoarState()
    {
        if(!_isBossRoar)
        {
            // 포효상태가 되면 상태 변환이 일어나기 전까지
            // 더해지고 있던 _timer 시간 초기화
            _timer = 0.0f;
            _isBossRoar = true;
            _playerMove.IsMoveStop = true;
            _monsterAnimator.Play("Roar");
            SoundManager.Instance.PlayFX(SoundKey.BossRoarSound, 0.04f);
            Time.timeScale = 0.5f;
            // 다른 상태에서 상태 초기화가 일어나지 않은 상황에서
            // Roar 상태로 넘어올 수가 있음 그래서 초기화 한번 함
            InitBossStateTracker();
            // 보스가 포효하는 동안 플레이어 스킬은 다시 잠궈놓음
            _playerSkill.DisablePlayerSkills();
            // 보스 월드 위치 -> 스크린 위치 변환
            Vector3 pos = Camera.main.WorldToScreenPoint(transform.position);
            // 레이디얼 블러 위치 넘겨주기
            InGameUIManager.Instance.SetRadialBlurImage(pos);
        }

        // 슬로우된 시간에 영향을 안받게
        _timer += Time.unscaledDeltaTime;

        if (_timer >= _roarDuration)
        {
            _timer = 0.0f;
            _playerMove.IsMoveStop = false;
            Time.timeScale = 1.0f;
            _bossState = BossState.Idle;
            // 레이디얼 블러 클리어
            InGameUIManager.Instance.ClearRadialBlur();
            _playerSkill.EnablePlayerSkills(); // 플레이어 스킬 다시 해제
        }
    }

    protected override void HandleDeadState()
    {
        // 슬로우 모션으로 보스가 죽는 애니메이션 실행
        if (!_isBossDead)
        {
            // 다른 state에서 더해지고 있던 _timer 시간 초기화
            _timer = 0.0f;
            _isBossDead = true;
            SoundManager.Instance.PlayFX(SoundKey.BossDeathSound, 0.04f);
            Time.timeScale = 0.25f;
        }

        // 슬로우된 시간에 영향을 안받게
        _timer += Time.unscaledDeltaTime;

        if (_timer >= _slowMotionDuration)
        {
             _timer = 0.0f;
            Time.timeScale = 1.0f;
            SoundManager.Instance.StopBGM();
            SoundManager.Instance.PlayFX(SoundKey.VictorySound, 0.03f);
            _bossState = BossState.None;
            _monsterCurrentState = MonsterStatus.None;
        }
    }

    public override void OnInActive()
    {
        base.OnInActive();
        InGameUIManager.Instance.OnGameClearPanel();
    }

    protected override void HandleHitState()
    {
        if (_curHp <= 0.0f)
        { 
            _monsterCurrentState = MonsterStatus.Dead;
            return;
        }

        base.HandleHitState();
    }

    private void TransitionFromState(int prevState)
    {
        // 상태 바뀌면 일단 bool형 변수들 리셋 시킴
        ResetStateFlags();

        // 이미 실행된 상태를 뺀 나머지 목록을 생성
        List<int> availableStates = new List<int>();
        for (int i = 0; i < _bossStateTracker.Length; i++)
        {
            if (_bossStateTracker[i] == 0) // 이미 실행된 상태는 제외
            {
                availableStates.Add(i);
            }
        }

        // 나머지 상태에서 랜덤으로 선택
        if (availableStates.Count > 0)
        {
            int randomIndex = Random.Range(0, availableStates.Count);
            int selectedState = availableStates[randomIndex];

            // 이전에 실행된 상태를 0으로 초기화
            _bossStateTracker[prevState] = 0; // 이전 상태 0으로 초기화
            _bossStateTracker[selectedState]++; // 새 상태 +1

            _bossState = (BossState)selectedState;
        }
    }

    // 포효 후 보스 상태 선택을 리셋하기 위해 보스 상태 배열 초기화
    private void InitBossStateTracker()
    {
        for (int i = 0; i < _bossStateTracker.Length; i++)
        {
            _bossStateTracker[i] = 0;
        }
    }

    // bool형 변수들 리셋
    private void ResetStateFlags()
    {
        _isIdleState = false;
        _isRunState = false;
        _isRushState = false;
        _isAttackState = false;
    }

    public override void MonsterGetDamage(float damage)
    {
        base.MonsterGetDamage(damage);
        if (_monsterCurrentState == MonsterStatus.Dead)
        {
            _timer = 0.0f;
            _bossState = BossState.Dead;
        }
    }
}