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

        if (CanMove())
        {
            Move();
            Attack();
        }
    }

    private void ResetOriginalColor()
    {
        // 슬라임 모델 색상 원래대로 복구
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

        // 슬라임 피격 연출로 모델을 흰색 발광 시킴 
        foreach (Renderer slimeMaterial in _slimeMaterial)
        {
            MaterialPropertyBlock block = new MaterialPropertyBlock();
            slimeMaterial.material.EnableKeyword("_EMISSION");
            slimeMaterial.GetPropertyBlock(block);
            block.SetColor("_EmissionColor", Color.white);
            slimeMaterial.SetPropertyBlock(block);
        }
    }

    // 몬스터 애니메이션 상태로 움직일 수 있는지 확인
    private bool CanMove()
    { 
        // 애니메이션에 Base Layer를 가져온거고 Base Layer는 인덱스가 0 이어서 매개변수가 0임
        _monsterAnimStateInfo = _monsterAnimator.GetCurrentAnimatorStateInfo(0);

        bool isInDead = _monsterAnimStateInfo.IsName("Dead");
        
        // Dead 상태가 아니라면 true 반환
        return !isInDead;
    }
}
