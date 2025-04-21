using UnityEngine;

public class Slime : MeleeMonster
{
    private int _slimeKey = 102;

    void Start()
    {
        base.Start();

        // 키값에 따른 몬스터 데이터 세팅
        SetMonsterData(MonsterDataManager.Instance.GetMonsterData(_slimeKey));
    }

    void Update()
    {
        base.Update();

        StopAttack();

        if (CanMove())
        {
            Move();
            Attack();
        }
    }

    // 몬스터 애니메이션 상태로 움직일 수 있는지 확인
    private bool CanMove()
    { 
        // 애니메이션에 Base Layer를 가져온거고 Base Layer는 인덱스가 0 이어서 매개변수가 0임
        _monsterAnimStateInfo = _monsterAnimator.GetCurrentAnimatorStateInfo(0);

        bool isInDead = _monsterAnimStateInfo.IsName("Dead");
        
        // Dead 상태가 아니라면 true 반환
        return !isInDead;
    }
}
