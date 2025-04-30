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

    // 캐릭터 선택 시 인게임으로 넘어감
    private void OnButtonClick(int index)
    {
        int key = _selectPlayerStartIndex + index;
        SelectPlayerData selectPlayerData = SelectPlayerDataManager.Instance.GetSelectPlayerData(key);

        GameManager.Instance.SetPlayer(selectPlayerData.PlayerKey, selectPlayerData.SkillIndex, selectPlayerData.PlayerName);
        SceneManager.LoadScene("InGameScene");
    }

    private void SetSelectPlayerBtnUI()
    {
        // 버튼 이미지, 텍스트 바꾸는 작업
        for (int i = 0; i < _selectPlayerBtnCount; i++)
        {
            int key = _selectPlayerStartIndex + i;
            _selectPlayerBtns[i].GetComponent<SelectPlayerButton>().SetSelectPlayerUI(key);
        }
    }
}
