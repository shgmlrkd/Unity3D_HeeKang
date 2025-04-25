using UnityEngine;

public class InGameUIManager : Singleton<InGameUIManager>
{
    private enum InGamePanel
    {
        PlayerPanel = 1, SkillPanel
    }

    private enum PlayerPanel
    {
        HpBar, ExpBar, InGameTimer
    }

    private enum SkillPanel
    {
        SkillPanel
    }

    private Transform[] _inGamePanels;
    private Transform[] _playerUIs;
    private Transform[] _skillUIs;
    private Transform _inGameCanvas;

    private readonly int _cardButtonCount = 3;

    private int _panelChildrenCount;
    private int _playerPanelChildrenCount;
    private int _skillPanelChildrenCount;

    private void OnEnable()
    {
        _inGameCanvas = GameObject.Find("InGameCanvas").GetComponent<Transform>();

        // 인게임 패널 세팅
        SetInGamePanel();
        // 플레이어 UI 패널 세팅
        SetPlayerPanel();
        // 스킬 UI 패널 세팅
        SetSkillPanel();
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

    // SkillPanel의 자식 UI들을 정리하고 스킬 버튼을 동적으로 생성하는 함수
    private void SetSkillPanel()
    {
        // SkillPanel의 자식 수를 저장
        _skillPanelChildrenCount = _inGamePanels[(int)InGamePanel.SkillPanel].childCount;
        // 초기화
        _skillUIs = new Transform[_skillPanelChildrenCount];
        // 각 UI를 배열에 저장
        for (int i = 0; i < _skillPanelChildrenCount; i++)
        {
            _skillUIs[i] = _inGamePanels[(int)InGamePanel.SkillPanel].GetChild(i);
        }

        // 프리팹 생성
        GameObject skillBtnPrefab = Resources.Load<GameObject>("Prefabs/UI/SkillUIButton");

        // 스킬 버튼 3개 추가
        for(int i = 0; i < _cardButtonCount; i++)
        {
            GameObject skillBtn = Instantiate(skillBtnPrefab, _skillUIs[(int)SkillPanel.SkillPanel]);
        }
        // 스킬 패널 처음에 비활성화
        _inGamePanels[(int)InGamePanel.SkillPanel].gameObject.SetActive(false);
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

    // 스킬 관련된 모든 UI 요소
    public Transform[] GetSkillButtonUI()
    {
        return _skillUIs[(int)SkillPanel.SkillPanel].GetComponentsInChildren<Transform>();
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
}