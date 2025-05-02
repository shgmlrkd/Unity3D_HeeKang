using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour
{
    private enum SkillUI
    { 
        Icon = 1, Description, Level
    }

    private Transform[] _skillBtn;

    private Image _skillIcon;
    private TextMeshProUGUI _skillText;
    private TextMeshProUGUI _skillLevel;

    private void Awake()
    {
        _skillBtn = GetComponentsInChildren<Transform>();
        _skillIcon = _skillBtn[(int)SkillUI.Icon].GetComponent<Image>();
        _skillText = _skillBtn[(int)SkillUI.Description].GetComponent<TextMeshProUGUI>();
        _skillLevel = _skillBtn[(int)SkillUI.Level].GetComponent<TextMeshProUGUI>();
    }

    // 키 값에 따른 스킬 UI 세팅
    public void SetSkillUI(int key, int levelKey)
    {
        WeaponData data = WeaponDataManager.Instance.GetWeaponData(key);
        _skillIcon.sprite = Resources.Load<Sprite>(data.UIPath);
        _skillText.text = data.Description;
        _skillLevel.text = "Level " + levelKey.ToString();
    }   
}