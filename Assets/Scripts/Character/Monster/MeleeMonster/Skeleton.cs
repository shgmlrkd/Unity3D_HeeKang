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

        // Ű���� ���� ���� ������ ����
        SetMonsterData(MonsterDataManager.Instance.GetMonsterData(_skeletonKey));
    }

    private void Update()
    { 
        base.Update();

        // �ִϸ��̼ǿ� Base Layer�� �����°Ű� Base Layer�� �ε����� 0 �̾ �Ű������� 0��
        _monsterAnimStateInfo = _monsterAnimator.GetCurrentAnimatorStateInfo(0);

        // ������ ���� ���¿� ���� �ൿ ó��
        switch (_monsterCurrentState)
        {
            // ���Ͱ� Run ���¸� �̵��� ����
            case MonsterStatus.Run:
                if (CanMove())
                {
                    Move();
                    Attack();
                }
                break;
            // ���Ͱ� Hit ���¸� �ִϸ��̼� üũ�ؼ� Run ���·� ����
            case MonsterStatus.Hit:
                if(IsHitAnimationFinished())
                {
                    _monsterCurrentState = MonsterStatus.Run;
                }
                break;
        }
    }
    
    // ���� �ִϸ��̼� ���·� ������ �� �ִ��� Ȯ��
    private bool CanMove()
    {
        bool isInHit = _monsterAnimStateInfo.IsName("Hit");
        bool isInDead = _monsterAnimStateInfo.IsName("Dead");

        // Hit�̳� Dead ���°� �ƴ϶�� true ��ȯ
        return !(isInHit || isInDead);
    }

    private bool IsHitAnimationFinished()
    {
        bool isInHit = _monsterAnimStateInfo.IsName("Hit");

        return !isInHit; // Hit �ִϸ��̼� �������� true ��ȯ 
    }
}