using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PlayerSkill : MonoBehaviour
{
    private List<Skill> _skills;
    public List<Skill> Skills
    {
        get { return _skills; }
    }

    private readonly int _weaponStartKey = 300;

    private void Awake()
    {
        _skills = new List<Skill>();
    }

    private void Start()
    {
        _skills.Add(gameObject.AddComponent<BulletSkill>());
        _skills.Add(gameObject.AddComponent<KunaiSkill>());
        _skills.Add(gameObject.AddComponent<SwordSkill>());
        _skills.Add(gameObject.AddComponent<AxeSkill>());
        _skills.Add(gameObject.AddComponent<FireBallSkill>());
        _skills.Add(gameObject.AddComponent<LaserSkill>());

        foreach(Skill skill in _skills)
        {
            skill.enabled = false;
        }

        _skills[0].enabled = true;
    }

    public void PlayerSkillUnlockOrLevelUp(int key)
    {
        // _skills ����Ʈ�� �ε��� ��ȣ
        int index = (key - _weaponStartKey) / WeaponDataManager.Instance.WeaponMaxLevel;
        
        // ������ ������ Ű��
        if (!_skills[index].enabled)
        {
            _skills[index].enabled = true;
        }
        else // ���������� ������
        {
            _skills[index].LevelUp();
        }
    }
}