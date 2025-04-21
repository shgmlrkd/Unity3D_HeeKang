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

        Move();
        Attack();
    }
}
