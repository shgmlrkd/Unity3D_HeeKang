using UnityEngine;

public class Slime : MeleeMonster
{
    private Renderer[] _slimeMaterial;

    private readonly float _colorFlashTime = 0.1f;
    private float _flashOnTimer = 0.0f;
    private bool _getDamaged = false;

    private int _slimeKey = 101;

    private void OnEnable()
    {
       SetMonsterKey(_slimeKey);

        base.OnEnable();
    }

    private void Start()
    {
        base.Start();
        _slimeMaterial = GetComponentsInChildren<Renderer>();
    }

    private void Update()
    {
        base.Update();

        // �ǰ� �� 0.1�� �� �� ���� ����
        if (_getDamaged)
        {
            _flashOnTimer += Time.deltaTime;
            if (_flashOnTimer >= _colorFlashTime)
            {
                _flashOnTimer -= _colorFlashTime;
                _getDamaged = false;
                ResetOriginalColor();
            }
        }

        if (CanMove())
        {
            Move();
            Attack();
        }
    }

    private void ResetOriginalColor()
    {
        // ������ �� ���� ������� ����
        foreach (Renderer slimeMaterial in _slimeMaterial)
        {
            MaterialPropertyBlock block = new MaterialPropertyBlock();
            slimeMaterial.GetPropertyBlock(block);
            block.SetColor("_EmissionColor", Color.black);
            slimeMaterial.SetPropertyBlock(block);
        }
    }

    public override void MonsterGetDamage(float damage)
    {
        base.MonsterGetDamage(damage);

        _getDamaged = true;

        // ������ �ǰ� ����� ���� ��� �߱� ��Ŵ 
        foreach (Renderer slimeMaterial in _slimeMaterial)
        {
            MaterialPropertyBlock block = new MaterialPropertyBlock();
            slimeMaterial.material.EnableKeyword("_EMISSION");
            slimeMaterial.GetPropertyBlock(block);
            block.SetColor("_EmissionColor", Color.white);
            slimeMaterial.SetPropertyBlock(block);
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
