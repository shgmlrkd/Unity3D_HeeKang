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

    private readonly int _weaponStartIndexKey = 300;

    private int _weaponKey;

    private void Awake()
    {
        _skillBtn = GetComponentsInChildren<Transform>();
        _skillIcon = _skillBtn[(int)SkillUI.Icon].GetComponent<Image>();
        _skillText = _skillBtn[(int)SkillUI.Text].GetComponent<TextMeshProUGUI>();
    }

    public void SkillLevelUp()
    {
        _weaponKey++;
        WeaponData data = WeaponDataManager.Instance.GetWeaponData(_weaponKey);
        _skillIcon.sprite = Resources.Load<Sprite>(data.UIPath);
        _skillText.text = data.Description;
    }

    // 키 값에 따른 스킬 UI 세팅
    public void SetSkillUI(int key)
    {
        _weaponKey = _weaponStartIndexKey + key;
        WeaponData data = WeaponDataManager.Instance.GetWeaponData(_weaponKey);
        _skillIcon.sprite = Resources.Load<Sprite>(data.UIPath);
        _skillText.text = data.Description;
    }
}