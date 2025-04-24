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

    protected override bool CanMove()
    {
        _monsterAnimStateInfo = _monsterAnimator.GetCurrentAnimatorStateInfo(0);

        bool isInHit = _monsterAnimStateInfo.IsName("Hit");
        bool isInDead = _monsterAnimStateInfo.IsName("Dead");

        return !(isInHit || isInDead);
    }
}