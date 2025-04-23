using UnityEngine;
using System.Collections.Generic;

public class PlayerSkill : MonoBehaviour
{
    private List<Skill> _skills = new List<Skill>();

    private void Start()
    {
        //_skills.Add(gameObject.AddComponent<BulletSkill>());
        //_skills.Add(gameObject.AddComponent<KunaiSkill>());
        _skills.Add(gameObject.AddComponent<LaserSkill>());
    }
}