using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.UI;

public class Skeleton : MeleeMonster
{
    private int _skeletonKey = 102;

    private void OnEnable()
    {
        SetMonsterKey(_skeletonKey);

        base.OnEnable();
    }

    private void Start()
    {
        base.Start();

        // 키값에 따른 몬스터 데이터 세팅
        SetMonsterData(MonsterDataManager.Instance.GetMonsterData(_skeletonKey));
    }

    private void Update()
    { 
        base.Update();

        // 애니메이션에 Base Layer를 가져온거고 Base Layer는 인덱스가 0 이어서 매개변수가 0임
        _monsterAnimStateInfo = _monsterAnimator.GetCurrentAnimatorStateInfo(0);

        // 몬스터의 현재 상태에 따라 행동 처리
        switch (_monsterCurrentState)
        {
            // 몬스터가 Run 상태면 이동과 공격
            case MonsterStatus.Run:
                if (CanMove())
                {
                    Move();
                    Attack();
                }
                break;
            // 몬스터가 Hit 상태면 애니메이션 체크해서 Run 상태로 돌림
            case MonsterStatus.Hit:
                if(IsHitAnimationFinished())
                {
                    _monsterCurrentState = MonsterStatus.Run;
                }
                break;
        }
    }
    
    // 몬스터 애니메이션 상태로 움직일 수 있는지 확인
    private bool CanMove()
    {
        bool isInHit = _monsterAnimStateInfo.IsName("Hit");
        bool isInDead = _monsterAnimStateInfo.IsName("Dead");

        // Hit이나 Dead 상태가 아니라면 true 반환
        return !(isInHit || isInDead);
    }

    private bool IsHitAnimationFinished()
    {
        bool isInHit = _monsterAnimStateInfo.IsName("Hit");

        return !isInHit; // Hit 애니메이션 끝났으면 true 반환 
    }
}