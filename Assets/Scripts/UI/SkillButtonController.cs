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

        /*// 버튼의 자식인 이미지, 텍스트 가져오기
        _skillBtns = ;
        _skillImage = _skillBtn[(int)SkillBtn.SkillImage].GetComponent<Image>();
        _skillText = _skillBtn[(int)SkillBtn.SkillText].GetComponent<TextMeshProUGUI>();*/
    }

    // 키 값에 따른 스킬 UI 세팅
    public void SetSkillUI(int key)
    {
        WeaponData data = WeaponDataManager.Instance.GetWeaponData(key);
        _skillImage.sprite = Resources.Load<Sprite>(data.UIPath);
        _skillText.text = data.Description;
    }
}