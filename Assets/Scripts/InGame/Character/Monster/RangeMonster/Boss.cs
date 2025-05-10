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
    private readonly float _spinRotDuration = 0.3f; // Spin Attack �� �� �ε巯�� ȸ�� �ð�
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
        // ���� ü�� 30% ����
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
            // Idle �ִϸ��̼� ����
            if (_isRoar)
            {
                _isRoarEnd = true;
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
            _timer = 0.0f;
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

    // 360�� �������� ���� ���� ���̾�� ���ÿ� �߻��ϴ� �Լ�
    private void ShootFireballsInCircle(List<GameObject> fireBalls, float offset = 0.0f)
    {
        _monsterAnimator.SetTrigger("BurstFire");
        SoundManager.Instance.PlayFX(SoundKey.BossAttackSound, 0.04f);

        // fireball �� 20���� �߻��ϱ� ���� ���� ������
        float angleStep = Mathf.PI * 2 / _burstFireBallCount;

        // 20���� 360�� �� �������� �߻�
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
            // ��ȿ �� fireball �߻�
            _monsterAnimator.SetTrigger("BurstFire");
            ShootFireballsInCircle(fireBalls);  // ��ȿ �� �߻�
            yield return null;
        }
        else
        {
            float angleStep = Mathf.PI * 2 / _burstFireBallCount;

            // ��ȿ �� �����ؼ� fireball �� �� �߻�
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
                yield return new WaitForSeconds(_shootFireInterval);  // �� ��° �߻� ���̿� ��� ���
            }
        }

        _attackRoutine = null;
    }

    // 540�� �������� ���� ���� ���̾�� �����̸� �༭ �߻��ϴ� �Լ�
    private IEnumerator SpinFireballSequence(List<GameObject> fireBalls)
    {
        _monsterAnimator.SetTrigger("DelayFireStart");

        // fireball �� 40���� �߻��ϱ� ���� ���� ������
        float angleStep = Mathf.PI * 3 / _spinFireBallCount;

        Vector3 playerPosition = _player.transform.position;

        int fireBallCount = 0;

        foreach (GameObject fireball in fireBalls)
        {
            if (!fireball.activeSelf)
            {
                // �÷��̾� �������� ���� ���͸� ����
                Vector3 directionToPlayer = (playerPosition - transform.position).normalized;
                
                // �÷��̾� ���� ���� ���� ���
                float startAngle = Mathf.Atan2(directionToPlayer.z, directionToPlayer.x); 

                float angle = startAngle + angleStep * fireBallCount;

                float x = Mathf.Cos(angle);
                float z = Mathf.Sin(angle);

                Vector3 dir = new Vector3(x, 0.0f, z).normalized;

                // ���� ��ġ���� targetRot���� ������ �̿��ؼ� ȸ��
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

                // ������ ��ȿ�� �Ѵٸ� �ڷ�ƾ�� ����
                if ((!_isRoarEnd && _isBossRoar) || _curHp <= 0.0f)
                {
                    _attackRoutine = null;
                    _isAttackState = false;
                    _bossStateTracker[(int)BossState.Attack] = 0;
                    yield break;
                }

                if (_isRoar)
                {
                    // �¿� ���� (��120�� ȸ��)
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
            // ��ȿ���°� �Ǹ� ���� ��ȯ�� �Ͼ�� ������
            // �������� �ִ� _timer �ð� �ʱ�ȭ
            _timer = 0.0f;
            _isBossRoar = true;
            _playerMove.IsMoveStop = true;
            _monsterAnimator.Play("Roar");
            SoundManager.Instance.PlayFX(SoundKey.BossRoarSound, 0.04f);
            Time.timeScale = 0.5f;
            // �ٸ� ���¿��� ���� �ʱ�ȭ�� �Ͼ�� ���� ��Ȳ����
            // Roar ���·� �Ѿ�� ���� ���� �׷��� �ʱ�ȭ �ѹ� ��
            InitBossStateTracker();
            // ������ ��ȿ�ϴ� ���� �÷��̾� ��ų�� �ٽ� ��ų���
            _playerSkill.DisablePlayerSkills();
            // ���� ���� ��ġ -> ��ũ�� ��ġ ��ȯ
            Vector3 pos = Camera.main.WorldToScreenPoint(transform.position);
            // ���̵�� �� ��ġ �Ѱ��ֱ�
            InGameUIManager.Instance.SetRadialBlurImage(pos);
        }

        // ���ο�� �ð��� ������ �ȹް�
        _timer += Time.unscaledDeltaTime;

        if (_timer >= _roarDuration)
        {
            _timer = 0.0f;
            _playerMove.IsMoveStop = false;
            Time.timeScale = 1.0f;
            _bossState = BossState.Idle;
            // ���̵�� �� Ŭ����
            InGameUIManager.Instance.ClearRadialBlur();
            _playerSkill.EnablePlayerSkills(); // �÷��̾� ��ų �ٽ� ����
        }
    }

    protected override void HandleDeadState()
    {
        // ���ο� ������� ������ �״� �ִϸ��̼� ����
        if (!_isBossDead)
        {
            // �ٸ� state���� �������� �ִ� _timer �ð� �ʱ�ȭ
            _timer = 0.0f;
            _isBossDead = true;
            SoundManager.Instance.PlayFX(SoundKey.BossDeathSound, 0.04f);
            Time.timeScale = 0.25f;
        }

        // ���ο�� �ð��� ������ �ȹް�
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

            _bossState = (BossState)selectedState;
        }
    }

    // ��ȿ �� ���� ���� ������ �����ϱ� ���� ���� ���� �迭 �ʱ�ȭ
    private void InitBossStateTracker()
    {
        for (int i = 0; i < _bossStateTracker.Length; i++)
        {
            _bossStateTracker[i] = 0;
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