using TMPro;
using UnityEngine;

public class InGameUIManager : Singleton<InGameUIManager>
{
    private enum InGamePanel
    {
        PlayerPanel = 1, SkillPanel, KillCntPanel
    }

    private enum PlayerPanel
    {
        HpBar, ExpBar, InGameTimer
    }

    private enum KillCountPanel
    {
        KillCount = 1
    }

    private Transform[] _inGamePanels;
    private Transform[] _playerUIs;
    private Transform[] _killCountUIs;
    private Transform _inGameCanvas;

    private int _panelChildrenCount;
    private int _playerPanelChildrenCount;
    private int _killCountPanelChildrenCount;

    private int _killCount = 0;

    private bool _isStart = false;

    private void OnEnable()
    {
        if(!_isStart)
        {
            _isStart = true;
            _inGameCanvas = GameObject.Find("InGameCanvas").GetComponent<Transform>();

            // 인게임 패널 세팅
            SetInGamePanel();
            // 플레이어 UI 패널 세팅
            SetPlayerPanel();
            // 몬스터 킬 수 패널 세팅
            SetKillCountPanel();
            // 몬스터 킬 수 초기화
            SetKillCountText();
            _inGamePanels[(int)InGamePanel.SkillPanel].gameObject.SetActive(false);
        }
    }

    // InGameCanvas 하위의 패널들을 배열로 정리하는 함수
    private void SetInGamePanel()
    {
        // InGameCanvas의 자식 수를 저장
        _panelChildrenCount = _inGameCanvas.childCount;
        // 초기화
        _inGamePanels = new Transform[_panelChildrenCount];
        // 자식(패널)을 배열에 저장
        for (int i = 0; i < _panelChildrenCount; i++)
        {
            _inGamePanels[i] = _inGameCanvas.GetChild(i);
        }
    }

    // PlayerPanel의 자식 UI들을 정리하는 함수
    private void SetPlayerPanel()
    {
        // PlayerPanel의 자식 수를 저장
        _playerPanelChildrenCount = _inGamePanels[(int)InGamePanel.PlayerPanel].childCount;
        // 초기화
        _playerUIs = new Transform[_playerPanelChildrenCount];
        // 각 UI를 배열에 저장
        for (int i = 0; i < _playerPanelChildrenCount; i++)
        {
            _playerUIs[i] = _inGamePanels[(int)InGamePanel.PlayerPanel].GetChild(i);
        }
    }

    private void SetKillCountPanel()
    {
        _killCountPanelChildrenCount = _inGamePanels[(int)InGamePanel.KillCntPanel].childCount;
        _killCountUIs = new Transform[_killCountPanelChildrenCount];
        for (int i = 0; i < _killCountPanelChildrenCount; i++)
        {
            _killCountUIs[i] = _inGamePanels[(int)InGamePanel.KillCntPanel].GetChild(i);
        }
    }

    // 플레이어 체력바에 관련된 모든 UI 요소
    public Transform[] GetPlayerHpBarUI()
    {
        return _playerUIs[(int)PlayerPanel.HpBar].GetComponentsInChildren<Transform>();
    }

    // 플레이어 경험치바에 관련된 모든 UI 요소
    public Transform[] GetPlayerExpBarUI()
    {
        return _playerUIs[(int)PlayerPanel.ExpBar].GetComponentsInChildren<Transform>();
    }

    // 인게임 시간 정보 가져오기
    public float GetInGameTimer()
    {
        return _playerUIs[(int)PlayerPanel.InGameTimer].GetComponent<Transform>().GetComponent<InGameTime>().InGameTimer;   
    }

    // 스킬 패널 활성화
    public void SkillPanelOn()
    {
        _inGamePanels[(int)InGamePanel.SkillPanel].gameObject.SetActive(true);
    }

    // 스킬 패널 비활성화
    public void SkillPanelOff()
    {
        _inGamePanels[(int)InGamePanel.SkillPanel].gameObject.SetActive(false);
    }

    public void SetKillCountText()
    {
        _killCountUIs[(int)KillCountPanel.KillCount].GetComponent<TextMeshProUGUI>().text = "X " + _killCount++.ToString();
    }
}