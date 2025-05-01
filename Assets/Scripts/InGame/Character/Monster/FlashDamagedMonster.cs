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
        // 피격 시 0.1초 후 모델 색상 복구
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
        // 몬스터 모델 색상 원래대로 복구
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

        // 몬스터 피격 연출로 모델을 발광 시킴 
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
