using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour
{
    private enum SkillUI
    { 
        Icon = 1, Text
    }

    private Transform[] _skillBtn;

    private Image _skillIcon;
    private TextMeshProUGUI _skillText;

    private void Awake()
    {
        _skillBtn = GetComponentsInChildren<Transform>();
        _skillIcon = _skillBtn[(int)SkillUI.Icon].GetComponent<Image>();
        _skillText = _skillBtn[(int)SkillUI.Text].GetComponent<TextMeshProUGUI>();
    }

    // 키 값에 따른 스킬 UI 세팅
    public void SetSkillUI(int key)
    {
        WeaponData data = WeaponDataManager.Instance.GetWeaponData(key);
        _skillIcon.sprite = Resources.Load<Sprite>(data.UIPath);
        _skillText.text = data.Description;
    }
}