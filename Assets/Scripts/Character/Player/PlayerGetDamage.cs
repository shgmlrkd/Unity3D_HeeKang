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
        // �ǰ� �� 0.1�� �� �� ���� ����
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
        // �÷��̾� �� ���� ������� ����
        foreach (Renderer playerMaterial in _playerMaterials)
        {
            //playerMaterial.material.color = Color.white;

            // �޸� ����, ����ȭ ���
            MaterialPropertyBlock block = new MaterialPropertyBlock();
            playerMaterial.GetPropertyBlock(block);
            block.SetColor("_BaseColor", Color.white);
            playerMaterial.SetPropertyBlock(block);
        }
    }

    public void GetDamage(float damage)
    {
        _getDamaged = true;

        // �÷��̾� �ǰ� ����� ���� ���������� ���� 
        foreach (Renderer playerMaterial in _playerMaterials)
        {
            // playerMaterial.material.color = Color.red;

            // �޸� ����, ����ȭ ���
            MaterialPropertyBlock block = new MaterialPropertyBlock();
            playerMaterial.GetPropertyBlock(block);
            block.SetColor("_BaseColor", Color.red);
            playerMaterial.SetPropertyBlock(block);
        }

        // �÷��̾� ������ ��
        _playerHpBar.SetPlayerCurHp(damage);
    }
}
