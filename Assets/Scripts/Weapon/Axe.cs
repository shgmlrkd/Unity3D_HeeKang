using UnityEngine;

public class Axe : Weapon
{
    private Transform _playerTransform; // 플레이어 위치

    private readonly float _speedRate = 2.0f;
    private float _angle = 0f;

    private void Update()
    {
        // 이 angle은 이동속도를 말함 (공전 속도)
        _angle += (_speedRate * Time.deltaTime); // 각도 증가

        Vector3 center = _playerTransform.position + _spawnPosYOffset;

        // 원형 경로의 x, z 좌표 계산
        float x = Mathf.Cos(_angle) * _weaponRange;
        float z = Mathf.Sin(_angle) * _weaponRange;

        Vector3 newPosition = new Vector3(x, 0, z) + center;
        transform.position = newPosition;

        // (회전)자전 속도
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

        // 기준 점이 될 벡터
        Vector3 center = playerTransform.position + _spawnPosYOffset;

        // 시작 위치에서 플레이어 중심까지의 방향 벡터
        Vector3 dir = pos - center;
        _angle = Mathf.Atan2(dir.z, dir.x);

        // 각도에 따른 원형 경로의 x, z 좌표 계산
        float x = Mathf.Cos(_angle) * _weaponRange;
        float z = Mathf.Sin(_angle) * _weaponRange;

        // 계산된 x, z 좌표와 플레이어의 중심 위치를 더해 시작 위치로 설정
        transform.position = new Vector3(x, 0, z) + center;
    }
}