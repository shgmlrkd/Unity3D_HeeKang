using UnityEngine;

public class Axe : Weapon
{
    private Transform _playerTransform; // �÷��̾� ��ġ

    private readonly float _speedRate = 2.0f;
    private float _angle = 0f;

    private void Update()
    {
        // �� angle�� �̵��ӵ��� ���� (���� �ӵ�)
        _angle += (_speedRate * Time.deltaTime); // ���� ����

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
        _weaponRange = data.AttackRange;
        _weaponSpeed = data.AttackSpeed;
        _weaponRotSpeed = data.RotSpeed;
        _weaponKnockBack = data.Knockback; 
        _weaponAttackPower = data.AttackPower;

        // ���� ���� �� ����
        Vector3 center = playerTransform.position + _spawnPosYOffset;

        // ���� ��ġ���� �÷��̾� �߽ɱ����� ���� ����
        Vector3 dir = pos - center;
        _angle = Mathf.Atan2(dir.z, dir.x);

        // ������ ���� ���� ����� x, z ��ǥ ���
        float x = Mathf.Cos(_angle) * _weaponRange;
        float z = Mathf.Sin(_angle) * _weaponRange;

        // ���� x, z ��ǥ�� �÷��̾��� �߽� ��ġ�� ���� ���� ��ġ�� ����
        transform.position = new Vector3(x, 0, z) + center;
    }
}