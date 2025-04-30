using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectPlayerButton : MonoBehaviour
{
    private enum SelectPlayerBtnUI
    {
        PlayerIcon = 1, SkillIcon = 3, PlayerName
    }

    private Transform[] _selectPlayerBtn;
    private Image[] _selectPlayerIcon;
    private TextMeshProUGUI _playerNameText;

    private string _playerPrefabName;
    public string PlayerPrefabName
    {
        get { return _playerPrefabName; }
    }
    private int _skillIndexKey;
    public int SkillIndexKey
    {
        get { return _skillIndexKey; } 
    }

    private void Start()
    {
        _selectPlayerBtn = GetComponentsInChildren<Transform>();
        _selectPlayerIcon = new Image[2];
        _selectPlayerIcon[0] = _selectPlayerBtn[(int)SelectPlayerBtnUI.PlayerIcon].GetComponent<Image>();
        _selectPlayerIcon[1] = _selectPlayerBtn[(int)SelectPlayerBtnUI.SkillIcon].GetComponent<Image>();
        _playerNameText = _selectPlayerBtn[(int)SelectPlayerBtnUI.PlayerName].GetComponent<TextMeshProUGUI>();
    }

    public void SetSelectPlayerUI(int key)
    {
        SelectPlayerData data = SelectPlayerDataManager.Instance.GetSelectPlayerData(key);
        _selectPlayerIcon[0].sprite = Resources.Load<Sprite>(data.PlayerTexturePath);
        _selectPlayerIcon[1].sprite = Resources.Load<Sprite>(data.SkillTexturePath);
        _playerNameText.text = data.PlayerName;
        _playerPrefabName = data.PlayerName;
        _skillIndexKey = data.SkillIndex;
    }
}
