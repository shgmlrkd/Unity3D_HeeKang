using UnityEngine;

public class Axe : Weapon
{
    private Transform _playerTransform; // �÷��̾� ��ġ

    private float _angle = 0.0f;

    private void Update()
    {
        // �� angle�� �̵��ӵ��� ���� (���� �ӵ�)
        _angle += (_weaponSpeed * Time.deltaTime); // ���� ����

        // �÷��̾� �������� ���ƾ��ϹǷ� ������ ��� ����
        Vector3 center = _playerTransform.position + _spawnPosYOffset;

        // ���� ����� x, z ��ǥ ���
        float x = Mathf.Cos(_angle) * _weaponRange;
        float z = Mathf.Sin(_angle) * _weaponRange;

        Vector3 newPosition = new Vector3(x, 0, z) + center;
        transform.position = newPosition;

        // (ȸ��)���� �ӵ�
        transform.Rotate(Vector3.forward * _weaponRotSpeed * Time.deltaTime);
    }

    public void SpinAround(Transform playerTransform, Vector3 pos, WeaponData data)
    {
        gameObject.SetActive(true);

        _playerTransform = playerTransform;
        transform.position = pos;
        _weaponRange = data.AttackRange;
        _weaponSpeed = data.AttackSpeed;
        _weaponRotSpeed = data.RotSpeed;
        _weaponKnockBack = data.KnockBack;
        _weaponAttackPower = data.AttackPower;
        _weaponKnockBackLerpTime = data.KnockBackLerpTime;

        // ���� ���� �� ����
        Vector3 center = playerTransform.position + _spawnPosYOffset;

        // �÷��̾� �߽ɿ��� ���� ��ġ������ ���� ����
        Vector3 dir = pos - center;
        // ó�� ���� ���� ���ϱ�
        _angle = Mathf.Atan2(dir.z, dir.x);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster") || other.CompareTag("Boss"))
        {
            SoundManager.Instance.PlayFX(SoundKey.AxeHitSound, 0.04f);
            DamageTextManager.Instance.ShowDamageText(other.transform, _weaponAttackPower, _color);
        }

        // ���Ϳ� �浹�ϰ� �˹� ��ġ�� ���� ��
        if (other.CompareTag("Monster") && _weaponKnockBack > 0)
        {
            Monster target = other.gameObject.GetComponent<Monster>();

            Vector3 knockBackDir = (target.transform.position - transform.position).normalized;
            knockBackDir.y = 0.0f;

            target.MonsterKnockBack(knockBackDir, _weaponKnockBack, _weaponKnockBackLerpTime);
        }
    }

    protected new void OnTriggerStay(Collider other)
    {
    }
}