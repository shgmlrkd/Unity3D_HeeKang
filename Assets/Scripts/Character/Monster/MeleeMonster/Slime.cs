using UnityEngine;

public class Slime : MeleeMonster
{
    private int _slimeKey = 102;

    void Start()
    {
        base.Start();

        // Ű���� ���� ���� ������ ����
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

    // ���� �ִϸ��̼� ���·� ������ �� �ִ��� Ȯ��
    private bool CanMove()
    { 
        // �ִϸ��̼ǿ� Base Layer�� �����°Ű� Base Layer�� �ε����� 0 �̾ �Ű������� 0��
        _monsterAnimStateInfo = _monsterAnimator.GetCurrentAnimatorStateInfo(0);

        bool isInDead = _monsterAnimStateInfo.IsName("Dead");
        
        // Dead ���°� �ƴ϶�� true ��ȯ
        return !isInDead;
    }
}
