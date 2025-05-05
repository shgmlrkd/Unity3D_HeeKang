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
    private readonly float _spinRotDuration = 0.3f; // Spin Attack �� �� �ε巯�� ȸ�� �ð�
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
        // ���� ü�� 30% ����
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

            return; // �� �̻� �̵����� ����
        }

        Vector3 direction = (_player.position - transform.position).normalized;
        direction.y = 0.0f;

        float distance = Vector3.Distance(_player.position, transform.position);

        // �÷��̾�� �����Ÿ��� �� ������ �̵�
        if (distance >= _monsterStatus.Range)
        {
            transform.Translate(direction * _monsterStatus.Speed * Time.deltaTime, Space.World);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * _monsterStatus.RotSpeed);
        }
        else
        {
            // ������ �����ϸ� ��Ʈ�� ���¸� true�� ��ȯ
            _isIntroEnd = true;
        }
    }

    private void HandleIdleState()
    {
        if (!_isIdleState)
        {
            _isIdleState = true;
            // Idle �ִϸ��̼� ����
            if (_isAngry)
            {
                _monsterAnimator.SetTrigger("AngryIdle");
            }
            else
            {
                _monsterAnimator.SetTrigger("Idle");
            }
            // Idle ���� bossStateTracker = [1, 0, 0, 0]
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
            // Run ���� bossStateTracker = [0, 1, 0, 0]
            _bossStateTracker[(int)BossState.Run]++;
        }

        // �÷��̾� �������� �̵�, ȸ��
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
            // Rush ���� bossStateTracker = [0, 0, 0, 1]
            _bossStateTracker[(int)BossState.Rush]++;

            // �̶� ������ ��������
            _direction = (_player.position - transform.position).normalized;
            _direction.y = 0.0f;
        }

        // ȸ��
        if (_direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(_direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _monsterStatus.RotSpeed); // 5�� ȸ�� �ӵ�
        }

        // �̵�
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
            // Attack ���� bossStateTracker = [0, 0, 1, 0]
            _bossStateTracker[(int)BossState.Attack]++;

            // ���� fireball ��������
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

    // 360�� �������� ���� ���� ���̾�� ���ÿ� �߻��ϴ� �Լ�
    private void ShootFireballsInCircle(List<GameObject> fireBalls)
    {
        _monsterAnimator.SetTrigger("BurstFire");

        // fireball �� 20���� �߻��ϱ� ���� ���� ������
        float angleStep = Mathf.PI * 2 / _burstFireBallCount;

        // 20���� 360�� �� �������� �߻�
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

    // 540�� �������� ���� ���� ���̾�� �����̸� �༭ �߻��ϴ� �Լ�
    private IEnumerator SpinFireballSequence(List<GameObject> fireBalls)
    {
        _monsterAnimator.SetTrigger("DelayFireStart");

        // fireball �� 40���� �߻��ϱ� ���� ���� ������
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
        // ���ο� ������� ������ �״� �ִϸ��̼� ����
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
        // ���� �ٲ�� �ϴ� bool�� ������ ���� ��Ŵ
        ResetStateFlags();

        // �̹� ����� ���¸� �� ������ ����� ����
        List<int> availableStates = new List<int>();
        for (int i = 0; i < _bossStateTracker.Length; i++)
        {
            if (_bossStateTracker[i] == 0) // �̹� ����� ���´� ����
            {
                availableStates.Add(i);
            }
        }

        // ������ ���¿��� �������� ����
        if (availableStates.Count > 0)
        {
            int randomIndex = Random.Range(0, availableStates.Count);
            int selectedState = availableStates[randomIndex];

            // ������ ����� ���¸� 0���� �ʱ�ȭ
            _bossStateTracker[prevState] = 0; // ���� ���� 0���� �ʱ�ȭ
            _bossStateTracker[selectedState]++; // �� ���� +1

            // Enum�� BossState���� MonsterState�� �ٲٴ°�
            _monsterCurrentState = ConvertBossStateToMonsterStatus((BossState)selectedState);
            print((BossState)selectedState);
        }
    }

    // bool�� ������ ����
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