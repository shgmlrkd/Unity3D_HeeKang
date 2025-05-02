using System.Collections;
using UnityEngine;

public class Boss : FlashDamagedMonster
{
    private MonsterFireBallSkill _monsterFireBallSkill;

    private int _beholderKey = 106;

    private void Awake()
    {
        base.Awake();

        _flashColor = Color.red;
    }

    private void Start()
    {
        base.Start();
        _monsterFireBallSkill = GetComponent<MonsterFireBallSkill>();
    }

    private void OnEnable()
    {
        SetMonsterKey(_beholderKey);

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
                break;
            case MonsterStatus.Run:
                break;
            case MonsterStatus.Attack:
                break;
            case MonsterStatus.Rush:
                break;
            case MonsterStatus.Hit:
                break;
            case MonsterStatus.Dead:
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
        }
    }
}