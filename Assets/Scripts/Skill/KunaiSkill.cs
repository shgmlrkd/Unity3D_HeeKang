using System.Collections;
using UnityEngine;

public class KunaiSkill : Skill
{
    private int _kunaiIndexKey = 305;

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

        // ȭ�� ���� ����
        float spreadDegree = 10.0f;
        int mid = count / 2;

        for (int i = 0; i < count; i++)
        {
            float offset = i - mid;

            // ¦���� ��� �߽��� ������ ����
            if (count % 2 == 0)
            {
                offset += 0.5f;
            }

            // ���� ȸ��: Y�� �������� ȸ�� (���� �������� ����)
            Quaternion rot = Quaternion.AngleAxis(offset * spreadDegree, Vector3.up);
            Vector3 shotDir = rot * dir;

            print(offset * spreadDegree);

            WeaponManager.Instance.KunaiFire(transform.position, shotDir, _weaponData);
        }
    }
}   