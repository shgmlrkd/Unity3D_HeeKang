using UnityEngine;
using System.Collections.Generic;

public class PlayerSkill : MonoBehaviour
{
    private List<Skill> _skills;
    public List<Skill> Skills
    {
        get { return _skills; }
    }

    private List<int> _skillLevel;
    public List<int> SkillsLevel
    {
        get { return _skillLevel; }
    }

    private readonly int _weaponStartKey = 300;

    private bool _isStart = false;

    private void Awake()
    {
        _skills = new List<Skill>();
        _skillLevel = new List<int>();
    }

    private void OnEnable()
    {
        if (!_isStart)
        {
            _skills.Add(gameObject.AddComponent<BulletSkill>());
            _skills.Add(gameObject.AddComponent<KunaiSkill>());
            _skills.Add(gameObject.AddComponent<SwordSkill>());
            _skills.Add(gameObject.AddComponent<AxeSkill>());
            _skills.Add(gameObject.AddComponent<FireBallSkill>());
            _skills.Add(gameObject.AddComponent<LaserSkill>());

            foreach (Skill skill in _skills)
            {
                skill.enabled = false;
                _skillLevel.Add(0);
            }

            _skills[GameManager.Instance.SkillIndex].enabled = true;
            _skillLevel[GameManager.Instance.SkillIndex]++;
        }
    }

    public void PlayerSkillUnlockOrLevelUp(int key)
    {
        // _skills 리스트의 인덱스 번호
        int index = (key - _weaponStartKey) / WeaponDataManager.Instance.WeaponMaxLevel;

        _skillLevel[index]++;

        // 안켜져 있으면 키고
        if (!_skills[index].enabled)
        {
            _skills[index].enabled = true;
        }
        else // 켜져있으면 레벨업
        {
            _skills[index].LevelUp();
        }

        foreach (Skill skill in _skills)
        {
            print(skill.enabled + " " + skill.Level);
        }
    }
}