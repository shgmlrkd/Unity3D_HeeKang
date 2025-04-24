using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SkillButtonController : MonoBehaviour
{
    private enum SkillBtn
    {
        SkillImage = 1, SkillText
    }

    private Transform[] _skillBtns;

    private Image _skillImage;
    private TextMeshProUGUI _skillText;

    private int _skillBtnCount;

    private void Start()
    {
        _skillBtnCount = GetComponent<Transform>().childCount;

        for(int i = 0 ; i < _skillBtnCount; i++)
        {

        }

        /*// ��ư�� �ڽ��� �̹���, �ؽ�Ʈ ��������
        _skillBtns = ;
        _skillImage = _skillBtn[(int)SkillBtn.SkillImage].GetComponent<Image>();
        _skillText = _skillBtn[(int)SkillBtn.SkillText].GetComponent<TextMeshProUGUI>();*/
    }

    // Ű ���� ���� ��ų UI ����
    public void SetSkillUI(int key)
    {
        WeaponData data = WeaponDataManager.Instance.GetWeaponData(key);
        _skillImage.sprite = Resources.Load<Sprite>(data.UIPath);
        _skillText.text = data.Description;
    }
}