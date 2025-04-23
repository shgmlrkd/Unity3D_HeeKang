using System.Collections;
using UnityEngine;

public class KunaiSkill : Skill
{
    private int _kunaiIndexKey = 306;

    private void Awake()
    {
        _weaponData = WeaponDataManager.Instance.GetWeaponData(_kunaiIndexKey);
        InitInterval(_weaponData);
    }

    void Start()
    {
        StartCoroutine(FireLoop());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            KunaiLevelUp();
        }
    }

    private void KunaiLevelUp()
    {
        LevelUp();
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

        Vector3 dir = (target.transform.position - transform.position).normalized;

        int count = _weaponData.ProjectileCount;

        // 화살 간의 각도
        float spreadDegree = 10f;
        int mid = count / 2;

        for (int i = 0; i < count; i++)
        {
            int offset = i - mid;

            // 짝수일 경우 중심이 없으니 조정
            if (count % 2 == 0 && i >= mid)
            {
                offset += 1;
            }

            // 각도 회전: Y축 기준으로 회전 (수평 방향으로 퍼짐)
            Quaternion rot = Quaternion.AngleAxis(offset * spreadDegree, Vector3.up);
            Vector3 shotDir = rot * dir;

            WeaponManager.Instance.KunaiFire(transform.position, shotDir, _weaponData);
        }
    }
}   