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
    }

    protected override bool CanMove()
    {
        _monsterAnimStateInfo = _monsterAnimator.GetCurrentAnimatorStateInfo(0);

        bool isInAttack = _monsterAnimStateInfo.IsName("Attack");
        bool isInDead = _monsterAnimStateInfo.IsName("Dead");

        return !(isInDead || isInAttack);
    }
}