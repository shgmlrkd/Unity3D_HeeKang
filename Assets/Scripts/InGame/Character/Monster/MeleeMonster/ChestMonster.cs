using System.Collections;
using UnityEngine;

public class ChestMonster : FlashDamagedMonster
{
    private float _activeTimer = 0.0f;

    private int _chestMonsterKey = 104;
    private bool _isInactive = false;

    private void Awake()
    {
        base.Awake();
        _fadeLerpTimer = 15.0f;
        _flashColor = Color.red;
    }

    private void OnEnable()
    {
        SetMonsterKey(_chestMonsterKey);

        base.OnEnable();

        _activeTimer = 0.0f; 
        _isInactive = false;
    }

    private void Update()
    {
        base.Update();

        _activeTimer += Time.deltaTime;

        // �̹��� �������� ���ͼ� ���� �ð� �� �Ǵ� ü�� 0 �����϶� �����
        if(!_isInactive && (_activeTimer >= _monsterStatus.LifeTime || _curHp <= 0))
        {
            _isInactive = true;
            _monsterCollider.enabled = false;
            
            _monsterCurrentState = MonsterStatus.Dead; 
            StartCoroutine(FadeOutOnDeath());
        }
    }
}
