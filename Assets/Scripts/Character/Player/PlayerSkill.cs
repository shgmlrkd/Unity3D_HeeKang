using UnityEngine;
using System.Collections.Generic;

public class PlayerSkill : Player
{
    private List<Skill> _skills = new List<Skill>();

    private void Start()
    {
        _skills.Add(gameObject.AddComponent<BulletSkill>());
        //_skills.Add(gameObject.AddComponent<KunaiSkill>());
    }
}