using UnityEngine;
using UnityEngine.UI;

public class SkillButtonController : MonoBehaviour
{
    private enum SkillBtn
    {
        SkillImage = 1, SkillText
    }

    // Ű�� �� �������� - �� ó���� + 1 / ���� �� = ��ų ����

    private Transform[] _skillBtns;

    private int _skillBtnCount;

    private void Start()
    {
        _skillBtnCount = GetComponent<Transform>().childCount;

        _skillBtns = new Transform[_skillBtnCount];

        // ��ų ��ư 3�� ����
        for (int i = 0; i < _skillBtnCount; i++)
        {
            int k = 1 + (i * 5);
            _skillBtns[i] = GetComponent<Transform>().GetChild(i);
            _skillBtns[i].GetComponent<SkillButton>().SetSkillUI(k);

            int index = i;
            _skillBtns[i].GetComponent<Button>().onClick.AddListener(() => OnButtonClick(index));
        }
    }

    private void OnButtonClick(int index)
    {
        _skillBtns[index].GetComponent<SkillButton>().SkillLevelUp();

        InGameUIManager.Instance.SkillPanelOff();
        Time.timeScale = 1;
    }
}