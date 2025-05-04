using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Boss : FlashDamagedMonster
{
    private enum BossState
    {
        Idle, Run, Attack, Rush
    }

    private MonsterFireBallSkill _monsterFireBallSkill;

    private Vector3 _direction = new Vector3();

    private readonly float _introDuration = 2.0f;
    private readonly float _idleDuration = 1.0f;
    private readonly float _runDuration = 3.0f;
    private readonly float _rushDuration = 2.5f;
    private readonly float _attackDuration = 2.7f;
    private readonly float _slowMotionDuration = 2.5f;

    private float _rushSpeed = 0.0f;
    private float _timer = 0.0f;

    private readonly int _bossKey = 106;
    private readonly int _bossWeaponKey = 402;
    private readonly int _fireBallCount = 20;

    private int[] _bossStateTracker = new int[4];

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
        _rushSpeed = _monsterStatus.Speed * 3.3f;
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
            _monsterAnimator.SetTrigger("Idle");
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
            _monsterAnimator.SetTrigger("Fire");
            // Attack ���� bossStateTracker = [0, 0, 1, 0]
            _bossStateTracker[(int)BossState.Attack]++;

            // ���� fireball ��������
            List<GameObject> fireballs = WeaponManager.Instance.GetObjects("BossFireBall");
            // fireball �� 36���� �߻��ϱ� ���� ���� ������
            float angleStep = Mathf.PI * 2 / _fireBallCount;

            // 20���� 360�� �� �������� �߻�
            int fireBallCount = 0;
            foreach (GameObject fireball in fireballs)
            {
                if (!fireball.activeSelf)
                {
                    float angle = angleStep * fireBallCount;

                    float x = Mathf.Cos(angle);
                    float z = Mathf.Sin(angle);

                    Vector3 dir = new Vector3(x, 0.0f, z).normalized;

                    _monsterFireBallSkill.Fire("BossFireBall", dir);

                    fireBallCount++;
                    if (fireBallCount >= _fireBallCount)
                        break;
                }
            }
        }

        _timer += Time.deltaTime;

        if (_timer >= _attackDuration)
        {
            _timer -= _attackDuration;

            TransitionFromState((int)BossState.Attack);
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
        }
    }

    // bool�� ������ ����
    private void ResetStateFlags()
    {
        _isIdleState = false; 
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