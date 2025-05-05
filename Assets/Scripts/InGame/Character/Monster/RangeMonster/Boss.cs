using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Boss : FlashDamagedMonster
{
    private enum BossState
    {
        Idle, Run, Attack, Rush, Roar
    }

    private enum BossAttack
    {
        BurstFireBall, SpinFireBall, BossAttackCount
    }

    private Coroutine _attackRoutine = null;
    private MonsterFireBallSkill _monsterFireBallSkill;

    private Vector3 _direction = new Vector3();

    private readonly float _toPercent = 100.0f;
    private readonly float _thirty = 30.0f;
    private readonly float _introDuration = 2.0f;
    private readonly float _idleDuration = 1.0f;
    private readonly float _runDuration = 4.0f;
    private readonly float _rushDuration = 2.5f;
    private readonly float _attackDuration = 2.7f;
    private readonly float _spinFireInterval = 0.01f;
    private readonly float _spinRotDuration = 0.3f; // Spin Attack 할 때 부드러운 회전 시간
    private readonly float _slowMotionDuration = 2.5f;

    private float _rushSpeed = 0.0f;
    private float _timer = 0.0f;

    private readonly int _bossKey = 106;
    private readonly int _bossWeaponKey = 402;
    private readonly int _burstFireBallCount = 20;
    private readonly int _spinFireBallCount = 40;

    private int[] _bossStateTracker = new int[4];

    private bool _isAngry = false;
    private bool _isIntroEnd = false;
    private bool _isIdleState = false;
    private bool _isRunState = false;
    private bool _isRushState = false;
    private bool _isAttackState = false;

    private void Awake()
    {
        base.Awake();

        _flashColor = Color.red;
    }

    private void Start()
    {
        base.Start();
        _rushSpeed = _monsterStatus.Speed * 2.8f;
        _monsterFireBallSkill = GetComponent<MonsterFireBallSkill>();
        _monsterFireBallSkill.SetMonsterWeaponData(_bossWeaponKey);
    }

    private void OnEnable()
    {
        SetMonsterKey(_bossKey);

        base.OnEnable();

        _monsterCurrentState = MonsterStatus.BossIntro;
    }

    protected override void Action()
    {
        // 보스 체력 30% 이하
        if (_curHp / _maxHp * _toPercent <= _thirty)
        {
            _isAngry = true;
        }

        switch(_monsterCurrentState)
        {
            case MonsterStatus.BossIntro:
                Intro();
                break;
            case MonsterStatus.Idle:
                HandleIdleState();
                break;
            case MonsterStatus.Run:
                if (CanMove())
                {
                    Move();
                }
                break;
            case MonsterStatus.Attack:
                HandleAttackState();
                break;
            case MonsterStatus.Rush:
                HandleRushState();
                break;
            case MonsterStatus.Hit:
                HandleHitState();
                break;
            case MonsterStatus.Dead:
                HandleDeadState();
                break;
            case MonsterStatus.None:
                break;
        }
    }

    private void Intro()
    {
        if (_isIntroEnd)
        {
            _timer += Time.deltaTime;

            if (_timer >= _introDuration)
            {
                _timer = 0f;
                _monsterCurrentState = MonsterStatus.Idle;
            }

            return; // 더 이상 이동하지 않음
        }

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
    }

    private void HandleIdleState()
    {
        if (!_isIdleState)
        {
            _isIdleState = true;
            // Idle 애니메이션 실행
            if (_isAngry)
            {
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
            _timer -= _idleDuration;
            
            TransitionFromState((int)BossState.Idle);
        }
    }

    protected override void Move()
    {
        if(!_isRunState)
        {
            _isRunState = true;
            if (_isAngry)
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
            _timer -= _runDuration;
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
            _timer -= _rushDuration;
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

            int randomAttack = 1;// Random.Range(0, (int)BossAttack.BossAttackCount);

            switch((BossAttack)randomAttack)
            {
                case BossAttack.BurstFireBall:
                    ShootFireballsInCircle(fireballs);
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
                _timer -= _attackDuration;

                TransitionFromState((int)BossState.Attack);
            }
        }
    }

    // 360도 방향으로 일정 수의 파이어볼을 동시에 발사하는 함수
    private void ShootFireballsInCircle(List<GameObject> fireBalls)
    {
        _monsterAnimator.SetTrigger("BurstFire");

        // fireball 중 20개만 발사하기 위해 각도 나누기
        float angleStep = Mathf.PI * 2 / _burstFireBallCount;

        // 20개를 360도 각 방향으로 발사
        int fireBallCount = 0;
        foreach (GameObject fireball in fireBalls)
        {
            if (!fireball.activeSelf)
            {
                float angle = angleStep * fireBallCount;

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

    // 540도 방향으로 일정 수의 파이어볼을 딜레이를 줘서 발사하는 함수
    private IEnumerator SpinFireballSequence(List<GameObject> fireBalls)
    {
        _monsterAnimator.SetTrigger("DelayFireStart");

        // fireball 중 40개만 발사하기 위해 각도 나누기
        float angleStep = Mathf.PI * 3 / _spinFireBallCount;

        int fireBallCount = 0;
        foreach (GameObject fireball in fireBalls)
        {
            if (!fireball.activeSelf)
            {
                float angle = angleStep * fireBallCount;

                float x = Mathf.Cos(angle);
                float z = Mathf.Sin(angle);

                Vector3 dir = new Vector3(x, 0.0f, z).normalized;

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

                fireBallCount++;
                if (fireBallCount >= _spinFireBallCount)
                {
                    _attackRoutine = null;
                    _monsterAnimator.SetTrigger("DelayFireEnd");
                    _monsterAnimator.SetBool("IsAngry", _isAngry);
                    yield break;
                }

                yield return new WaitForSeconds(_spinFireInterval);
            }
        }
    }

    protected override void HandleDeadState()
    {
        // 슬로우 모션으로 보스가 죽는 애니메이션 실행
        Time.timeScale = 0.25f;

        _timer += Time.unscaledDeltaTime;

        if(_timer >= _slowMotionDuration)
        {
            _timer -= _slowMotionDuration;
            Time.timeScale = 1.0f;
            _monsterCurrentState = MonsterStatus.None;
        }
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

            // Enum값 BossState에서 MonsterState로 바꾸는거
            _monsterCurrentState = ConvertBossStateToMonsterStatus((BossState)selectedState);
            print((BossState)selectedState);
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

    private MonsterStatus ConvertBossStateToMonsterStatus(BossState state)
    {
        switch (state)
        {
            case BossState.Run:
                return MonsterStatus.Run;
            case BossState.Attack:
                return MonsterStatus.Attack;
            case BossState.Rush:
                return MonsterStatus.Rush;
            default:
                return MonsterStatus.Idle;
        }
    }
}