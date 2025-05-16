using System.Collections;
using UnityEngine;

public class KunaiSkill : Skill
{
    private readonly float _spreadDegree = 10.0f;

    private int _kunaiIndexKey = 305;

    private void Awake()
    {
        _weaponData = WeaponDataManager.Instance.GetWeaponData(_kunaiIndexKey);
        InitInterval(_weaponData);
    }

    void Start()
    {
        _fireCoroutine = StartCoroutine(FireLoop());
    }

    public override void LevelUp()
    {
        base.LevelUp();
        _weaponData = WeaponDataManager.Instance.GetWeaponData(_kunaiIndexKey + _level);
        InitInterval(_weaponData);
    }

    private IEnumerator FireLoop()
    {
        while (true)
        {
            Fire();

            yield return _fireInterval;
        }
    }

    private void Fire()
    {
        GameObject target = MonsterManager.Instance.GetClosestMonster(transform.position);

        if (target == null)
            return;
        // 목표를 향한 기준 방향 계산
        Vector3 dir = (target.transform.position - transform.position).normalized;
        // 발사할 투사체 개수
        int count = _weaponData.ProjectileCount;
        // 중앙 인덱스 (대칭 기준점)
        int mid = (int)(count * 0.5f);

        for (int i = 0; i < count; i++)
        {
            // 각 투사체의 위치를 중심 기준으로 오프셋 계산
            float offset = i - mid;

            // 짝수 개일 경우, 정확한 중심이 없으므로 +0.5 보정하여 균형 맞춤
            if (count % 2 == 0)
            {
                offset += 0.5f;
            }
            // 오프셋에 각도 간격을 곱해 수평 각도 회전값 계산
            // Y축 기준 회전으로 수평 방향으로 퍼지게 함
            Quaternion rot = Quaternion.AngleAxis(offset * _spreadDegree, Vector3.up);
            Vector3 shotDir = rot * dir;
            // 회전된 방향으로 쿠나이 투사체 발사
            WeaponManager.Instance.KunaiFire(transform.position, shotDir, _weaponData);
        }
    }

    public override void StartSkill()
    {
        if (_fireCoroutine == null)
            _fireCoroutine = StartCoroutine(FireLoop());
    }

    public override void StopSkill()
    {
        if (_fireCoroutine != null)
        {
            StopCoroutine(_fireCoroutine);
            _fireCoroutine = null;
        }
    }
}   