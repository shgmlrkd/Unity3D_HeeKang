using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PlayerSkill : MonoBehaviour
{
    private enum AddSkill
    {
        Bullet = 300, Kunai = 305, Sword = 310, Axe = 315, FireBall = 320, Laser = 325
    }

    private List<Skill> _skills;

    private int _skillStartIndex;
        
    private void Awake()
    {
        _skills = new List<Skill>();
        _skillStartIndex = (int)AddSkill.Bullet;
    }

    private void Start()
    {
        _skills.Add(gameObject.AddComponent<BulletSkill>());
        /*_skills.Add(gameObject.AddComponent<KunaiSkill>());
        _skills.Add(gameObject.AddComponent<LaserSkill>());
        _skills.Add(gameObject.AddComponent<FireBallSkill>());
        _skills.Add(gameObject.AddComponent<AxeSkill>());
        _skills.Add(gameObject.AddComponent<SwordSkill>());*/
    }

    public void PlayerSkillUnlockOrLevelUp(int key)
    {
        /*switch((AddSkill)key)
        {
            case AddSkill.Bullet:
                // _skills ����Ʈ�� BulletSkill�� �ϳ��� �ִ��� üũ
                if (!_skills.Any(skill => skill is BulletSkill))
                    _skills.Add(gameObject.AddComponent<BulletSkill>());
                else
                {
                    Skill bulletSkill = _skills.FirstOrDefault(skill => skill is BulletSkill);
                    bulletSkill.GetComponent<BulletSkill>().LevelUp
                }
                break;
            case AddSkill.Kunai:
                // _skills ����Ʈ�� KunaiSkill�� �ϳ��� �ִ��� üũ
                if (!_skills.Any(skill => skill is KunaiSkill))
                    _skills.Add(gameObject.AddComponent<KunaiSkill>());
                else
                {

                }
                break;
            case AddSkill.FireBall:
                // _skills ����Ʈ�� FireBallSkill�� �ϳ��� �ִ��� üũ
                if (!_skills.Any(skill => skill is FireBallSkill))
                    _skills.Add(gameObject.AddComponent<FireBallSkill>());
                else
                {

                }
                break;
            case AddSkill.Laser:
                // _skills ����Ʈ�� LaserSkill�� �ϳ��� �ִ��� üũ
                if (!_skills.Any(skill => skill is LaserSkill))
                    _skills.Add(gameObject.AddComponent<LaserSkill>());
                else
                {

                }
                break;
        }*/
    }
}