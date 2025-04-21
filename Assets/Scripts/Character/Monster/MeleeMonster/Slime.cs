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

        Move();
        Attack();
    }
}
