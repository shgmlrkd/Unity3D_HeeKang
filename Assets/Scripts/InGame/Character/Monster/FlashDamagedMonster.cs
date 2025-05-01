using System.Collections.Generic;
using UnityEngine;

public class FlashDamagedMonster : Monster
{
    protected Renderer[] _monsterRenderers;

    protected Color _flashColor;

    private readonly float _colorFlashTime = 0.1f;

    private float _flashOnTimer = 0.0f;
    private bool _getDamaged = false;

    protected void Start()
    {
        base.Start();

        _monsterRenderers = GetComponentsInChildren<Renderer>();
       
        foreach (Renderer monsterRenderer in _monsterRenderers)
        {
            _monsterMaterials.Add(monsterRenderer.material);
        }
    }

    protected void Update()
    {
        base.Update();

        Flash();
    }

    private void Flash()
    {
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
    }

    private void ResetOriginalColor()
    {
        // ���� �� ���� ������� ����
        foreach (Renderer monsterMaterial in _monsterRenderers)
        {
            MaterialPropertyBlock block = new MaterialPropertyBlock();
            monsterMaterial.GetPropertyBlock(block);
            block.SetColor("_EmissionColor", Color.black);
            monsterMaterial.SetPropertyBlock(block);
        }
    }

    public override void MonsterGetDamage(float damage)
    {
        base.MonsterGetDamage(damage);

        _getDamaged = true;

        // ���� �ǰ� ����� ���� �߱� ��Ŵ 
        foreach (Renderer monsterMaterial in _monsterRenderers)
        {
            MaterialPropertyBlock block = new MaterialPropertyBlock();
            monsterMaterial.material.EnableKeyword("_EMISSION");
            monsterMaterial.GetPropertyBlock(block);
            block.SetColor("_EmissionColor", _flashColor);
            monsterMaterial.SetPropertyBlock(block);
        }
    }
}
