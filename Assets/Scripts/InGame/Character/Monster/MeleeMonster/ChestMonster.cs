using System.Collections;
using UnityEngine;

public class ChestMonster : FlashDamagedMonster
{
    private float _fadeLerpTimer = 15.0f;
    private float _activeTimer = 0.0f;

    private int _chestMonsterKey = 104;
    private bool _isInactive = false;

    private void Awake()
    {
        base.Awake();

        _flashColor = Color.red;
    }

    private void OnEnable()
    {
        SetMonsterKey(_chestMonsterKey);

        base.OnEnable();

        _activeTimer = 0.0f; 
        _isInactive = false;

        if (_monsterMaterials != null)
        {    
            // ���İ� 1�� ��������
            foreach (Material monsterMaterial in _monsterMaterials)
            {
                Color finalColor = monsterMaterial.color;
                finalColor.a = _maxAlphaValue;
                monsterMaterial.color = finalColor;
            }
        }
    }

    private void Update()
    {
        base.Update();

        _activeTimer += Time.deltaTime;
        // �̹��� �������� ���ͼ� ���� �ð� �� �����
        if(_activeTimer >= _monsterStatus.LifeTime && !_isInactive)
        {
            _isInactive = true;
            _monsterCollider.enabled = false;
            _monsterAnimator.SetTrigger("Dead");
            _monsterCurrentState = MonsterStatus.Dead;
            StartCoroutine(FadeChestMonster());
        }
    }

    private IEnumerator FadeChestMonster()
    {
        if (_monsterMaterials != null)
        {
            // ���̵� �ƿ�
            float elapsed = 0.0f;
            while (elapsed < _fadeLerpTimer)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / _fadeLerpTimer;

                foreach(Material monsterMaterial in _monsterMaterials)
                {
                    // ���� �� ����
                    Color color = monsterMaterial.color;
                    color.a = Mathf.Lerp(color.a, _minAlphaValue, t);
                    SetMonsterHpBarAlpha(color.a);
                    monsterMaterial.color = color;
                }

                yield return null;
            }

            foreach (Material monsterMaterial in _monsterMaterials)
            {
                // ���� ���İ� ����
                Color finalColor = monsterMaterial.color;
                finalColor.a = _minAlphaValue;
                SetMonsterHpBarAlpha(finalColor.a);
                monsterMaterial.color = finalColor;
            }

            gameObject.SetActive(false);
        }
    }
}
