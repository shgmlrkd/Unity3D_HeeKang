using UnityEngine;

public class Axe : Weapon
{
    private Transform _playerTransform; // 플레이어 위치

    private float _angle = 0.0f;

    private void Update()
    {
        // 이 angle은 이동속도를 말함 (공전 속도)
        _angle += (_weaponSpeed * Time.deltaTime); // 각도 증가

        // 플레이어 기준으로 돌아야하므로 중점을 계속 갱신
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
        transform.position = pos;
        _weaponRange = data.AttackRange;
        _weaponSpeed = data.AttackSpeed;
        _weaponRotSpeed = data.RotSpeed;
        _weaponKnockBack = data.KnockBack;
        _weaponAttackPower = data.AttackPower;
        _weaponKnockBackLerpTime = data.KnockBackLerpTime;

        // 기준 점이 될 벡터
        Vector3 center = playerTransform.position + _spawnPosYOffset;

        // 플레이어 중심에서 시작 위치까지의 방향 벡터
        Vector3 dir = pos - center;
        // 처음 시작 각도 구하기
        _angle = Mathf.Atan2(dir.z, dir.x);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster") || other.CompareTag("Boss"))
        {
            SoundManager.Instance.PlayFX(SoundKey.AxeHitSound, 0.04f);
            DamageTextManager.Instance.ShowDamageText(other.transform, _weaponAttackPower, _color);
        }

        // 몬스터와 충돌하고 넉백 수치가 있을 때
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