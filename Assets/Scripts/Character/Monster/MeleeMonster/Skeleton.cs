using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.UI;

public class Skeleton : MeleeMonster
{
    private int _skeletonKey = 101;

    private void Awake()
    {
        base.Awake();
        _monsterHpBarOffset = new Vector3(0.0f, 1.5f, 0.0f);
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

        // 콜라이더가 꺼지면 공격 멈추기
        StopAttack();

        // 움직일 수 있는 상태에서만 동작
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
        bool isInHit = _monsterAnimStateInfo.IsName("Hit");
        bool isInDead = _monsterAnimStateInfo.IsName("Dead");

        print("Hit : " +  isInHit + "  " + "Dead : " + isInDead);
        // Hit이나 Dead 상태가 아니라면 true 반환
        return !(isInHit || isInDead);
    }
}