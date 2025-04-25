using UnityEngine;
using UnityEngine.UI;

public class SkillButtonController : MonoBehaviour
{
    private enum SkillBtn
    {
        SkillImage = 1, SkillText
    }

    private Transform[] _skillBtns;

    private int _skillBtnCount;

    private void Start()
    {
        _skillBtnCount = GetComponent<Transform>().childCount;

        _skillBtns = new Transform[_skillBtnCount];

        // 스킬 버튼 3개 세팅
        for (int i = 0; i < _skillBtnCount; i++)
        {
            int k = 1 + (i * 5);
            _skillBtns[i] = GetComponent<Transform>().GetChild(i);
            _skillBtns[i].GetComponent<SkillButton>().SetSkillUI(k);
            _skillBtns[i].GetComponent<Button>().onClick.AddListener(() => _skillBtns[i].GetComponent<SkillButton>().SkillLevelUp());
        }
    }
}