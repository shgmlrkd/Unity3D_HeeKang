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
    
    private BossState _bossState = BossState.Idle;

    private readonly float _idleDuration = 2.5f;
    private readonly float _slowMotionDuration = 2.5f;

    private float _timer = 0.0f;

    private readonly int _bossKey = 106;
    private readonly int _bossWeaponKey = 402;
    private readonly int _fireBallCount = 20;

    private int[] _bossStateTracker = new int[4];

    private bool _isIdleSate = false;

    private void Awake()
    {
        base.Awake();

        _flashColor = Color.red;
    }

    private void Start()
    {
        base.Start();
        _monsterFireBallSkill = GetComponent<MonsterFireBallSkill>();
        _monsterFireBallSkill.SetMonsterWeaponData(_bossWeaponKey);
    }

    private void OnEnable()
    {
        SetMonsterKey(_bossKey);

        base.OnEnable();

        _monsterCurrentState = MonsterStatus.BossIntro;
    }

    protected override bool CanMove()
    {
        _monsterAnimStateInfo = _monsterAnimator.GetCurrentAnimatorStateInfo(0);

        bool isInRush = _monsterAnimStateInfo.IsName("Rush");
        bool isInAttack = _monsterAnimStateInfo.IsName("Attack");
        bool isInDead = _monsterAnimStateInfo.IsName("Dead");

        return !(isInDead || isInRush || isInAttack);
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
        Vector3 direction = (_player.position - transform.position).normalized;
        direction.y = 0;

        float distance = Vector3.Distance(_player.position, transform.position);

        if (direction.sqrMagnitude > 0 && distance >= _monsterStatus.Range)
        {
            transform.Translate(direction * _monsterStatus.Speed * Time.deltaTime, Space.World);

            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * _monsterStatus.RotSpeed);
        }
        else
        {
            _monsterCurrentState = MonsterStatus.Idle;
        }
    }

    private void HandleIdleState()
    {
        if (!_isIdleSate)
        {
            _isIdleSate = true;
            // Idle 모선 실행
            _monsterAnimator.SetTrigger("Idle");
            // Idle은 실행 했음 bossStateTracker = [1, 0, 0, 0]
            _bossStateTracker[(int)BossState.Idle]++;
        }

        _timer += Time.deltaTime;

        if(_timer >= _idleDuration)
        {
            _timer -= _idleDuration;
            _monsterCurrentState = MonsterStatus.Attack;
            //BossNextState((int)BossState.Idle);
        }
    }

    protected override void HandleAttackState()
    {
        // 보스 fireball 가져오기
        List<GameObject> fireballs = WeaponManager.Instance.GetObjects("BossFireBall");
        // fireball 중 36개만 발사하기 위해 각도 나누기
        float angleStep = Mathf.PI * 2 / _fireBallCount;
        // 36개를 360도 모든 방향으로 발사
        for(int i = 0; i < _fireBallCount; i++)
        {
            float angle = angleStep * i;

            float x = Mathf.Cos(angle);
            float z = Mathf.Sin(angle);

            Vector3 newPos = new Vector3(x, 0.0f, z);
            Vector3 dir = newPos.normalized;
            
            if (!fireballs[i].activeSelf)
            {
                _monsterFireBallSkill.Fire("BossFireBall", dir);
                fireballs[i].SetActive(true);
            }
        }
    }

    protected override void HandleDeadState()
    {
        Time.timeScale = 0.25f;

        _timer += Time.unscaledDeltaTime;

        if(_timer >= _slowMotionDuration)
        {
            _timer -= _slowMotionDuration;
            Time.timeScale = 1.0f;
            _monsterCurrentState = MonsterStatus.None;
        }
    }

    private void BossNextState(int prevState)
    {
        // 이미 실행된 상태를 뺀 나머지 목록을 생성
        List<int> availableStates = new List<int>();
        for (int i = 0; i < _bossStateTracker.Length; i++)
        {
            if (_bossStateTracker[i] == 0)  // 이미 실행된 상태는 제외
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
            _bossStateTracker[prevState] = 0;  // 이전 상태 0으로 초기화
            _bossStateTracker[selectedState]++;        // 새 상태 +1

            _monsterCurrentState = ConvertBossStateToMonsterStatus((BossState)selectedState);

            print((BossState)selectedState);
        }
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