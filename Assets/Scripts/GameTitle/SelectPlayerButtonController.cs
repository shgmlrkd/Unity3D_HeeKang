using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectPlayerButtonController : MonoBehaviour
{
    private Transform[] _selectPlayerBtns;

    private readonly int _selectPlayerStartIndex = 2001;

    private int _selectPlayerBtnCount;

    private void Start()
    {
        _selectPlayerBtnCount = GetComponent<Transform>().childCount;

        _selectPlayerBtns = new Transform[_selectPlayerBtnCount];

        for (int i = 0; i < _selectPlayerBtnCount; i++)
        {
            _selectPlayerBtns[i] = transform.GetChild(i);
            int index = i;
            _selectPlayerBtns[i].GetComponent<Button>().onClick.AddListener(() => OnButtonClick(index));
        }

        SetSelectPlayerBtnUI();
    }

    // ĳ���� ���� �� �ΰ������� �Ѿ
    private void OnButtonClick(int index)
    {
        int key = _selectPlayerStartIndex + index;
        SelectPlayerData selectPlayerData = SelectPlayerDataManager.Instance.GetSelectPlayerData(key);
        SoundManager.Instance.PlayFX(SoundKey.ButtonClickSound, 0.04f);
        GameManager.Instance.SetPlayer(selectPlayerData.PlayerKey, selectPlayerData.SkillIndex, selectPlayerData.PlayerName);
        SceneManager.LoadScene("InGameScene");
        SoundManager.Instance.StopBGM();
    }

    private void SetSelectPlayerBtnUI()
    {
        // ��ư �̹���, �ؽ�Ʈ �ٲٴ� �۾�
        for (int i = 0; i < _selectPlayerBtnCount; i++)
        {
            int key = _selectPlayerStartIndex + i;
            _selectPlayerBtns[i].GetComponent<SelectPlayerButton>().SetSelectPlayerUI(key);
        }
    }
}
