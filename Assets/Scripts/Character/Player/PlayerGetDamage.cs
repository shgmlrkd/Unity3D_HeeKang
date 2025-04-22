using UnityEngine;

public class PlayerGetDamage : MonoBehaviour
{
    private PlayerHpBar _playerHpBar;
    private Renderer[] _playerMaterials;

    private readonly float _colorFlashTime = 0.1f;
    private float _flashOnTimer = 0.0f;
    private bool _getDamaged = false;

    private void Start()
    {
        _playerHpBar = GetComponent<PlayerHpBar>();
        _playerMaterials = GetComponentsInChildren<Renderer>();
    }

    private void Update()
    {
        // 피격 시 0.1초 후 모델 색상 복구
        if(_getDamaged)
        {
            _flashOnTimer += Time.deltaTime;
            if(_flashOnTimer >= _colorFlashTime)
            {
                _flashOnTimer -= _colorFlashTime;
                _getDamaged = false;
                ResetOriginalColor();
            }
        }
    }

    private void ResetOriginalColor()
    {
        // 플레이어 모델 색상 원래대로 복구
        foreach (Renderer playerMaterial in _playerMaterials)
        {
            //playerMaterial.material.color = Color.white;

            // 메모리 성능, 최적화 고려
            MaterialPropertyBlock block = new MaterialPropertyBlock();
            playerMaterial.GetPropertyBlock(block);
            block.SetColor("_BaseColor", Color.white);
            playerMaterial.SetPropertyBlock(block);
        }
    }

    public void GetDamage(float damage)
    {
        _getDamaged = true;

        // 플레이어 피격 연출로 모델을 빨간색으로 만듦 
        foreach (Renderer playerMaterial in _playerMaterials)
        {
            // playerMaterial.material.color = Color.red;

            // 메모리 성능, 최적화 고려
            MaterialPropertyBlock block = new MaterialPropertyBlock();
            playerMaterial.GetPropertyBlock(block);
            block.SetColor("_BaseColor", Color.red);
            playerMaterial.SetPropertyBlock(block);
        }

        // 플레이어 데미지 줌
        _playerHpBar.SetPlayerCurHp(damage);
    }
}
