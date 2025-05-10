using Photon.Pun;
using TMPro;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameUIManager : Singleton<InGameUIManager>
{
    private enum InGamePanel
    {
        RadialBlurImage, PlayerPanel = 3, SkillPanel, KillCntPanel, GoldCntPanel, GameClearPanel
    }

    private enum PlayerPanel
    {
        HpBar, ExpBar, InGameTimer
    }

    private enum KillCountPanel
    {
        KillCount = 1
    }
    private enum GoldCountPanel
    {
        GoldCount = 1
    }

    private enum GameClearUI
    {
        GameClearPanel, ClearTimeRecord = 2, RankName, RankingNameSubmitButton = 6
    }

    private Transform[] _inGamePanels;
    private Transform[] _playerUIs;
    private Transform[] _killCountUIs;
    private Transform[] _goldCountUIs;
    private Transform[] _gameClearUIs;
    private Transform _inGameCanvas;
    private Transform _radialBlurTransform;

    private Button _registerRankingButton;

    private Material _radialBlurMat;

    private Vector3 _radialBlurScale = new Vector3(7.0f, 2.0f, 1.0f);

    private readonly float _oneMinute = 60.0f;
    private float _clearTime = 0.0f;

    private int _panelChildrenCount;
    private int _playerPanelChildrenCount;
    private int _killCountPanelChildrenCount;
    private int _goldCountPanelChildrenCount;
    private int _killCount = 0;
    private int _goldCount = 0;

    private bool _isStart = false;

    private void OnEnable()
    {
        if(!_isStart)
        {
            _isStart = true;
            _inGameCanvas = GameObject.Find("InGameCanvas").GetComponent<Transform>();
            _radialBlurMat = Resources.Load<Material>("Materials/RadialBlurMaterial");
            
            // 인게임 패널 세팅
            SetInGamePanel();
            // 플레이어 UI 패널 세팅
            SetPlayerPanel();
            // 몬스터 킬 수 패널 세팅
            SetKillCountPanel();
            // 몬스터 킬 수 초기화
            SetKillCountText();
            // 골드 개수 패널 세팅
            SetGoldCountPanel();
            // 골드 개수 초기화
            SetGoldCountText();
            // 게임 클리어 UI들 초기화
            SetGameClearPanel();

            _inGamePanels[(int)InGamePanel.SkillPanel].gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        _registerRankingButton = _gameClearUIs[(int)GameClearUI.RankingNameSubmitButton].GetComponent<Button>();
        // 랭킹 등록 확인 버튼 클릭 이벤트
        _registerRankingButton.onClick.AddListener(() => OnClickRegisterRanking());
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

    public void SetRadialBlurImage(Vector3 pos)
    {
        _radialBlurTransform = _inGamePanels[(int)InGamePanel.RadialBlurImage];
        _radialBlurTransform.gameObject.GetComponent<RawImage>().material = _radialBlurMat;
        Vector3 position = new Vector3(pos.x / Screen.width, pos.y / Screen.height, 0.0f);
        _radialBlurMat.SetVector("_Scale", _radialBlurScale);
        _radialBlurMat.SetVector("_Position", position);
    }

    public void ClearRadialBlur()
    {
        _radialBlurTransform.gameObject.GetComponent<RawImage>().material = null;
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

    private void SetGameClearPanel()
    {
        _gameClearUIs = _inGamePanels[(int)InGamePanel.GameClearPanel].GetComponentsInChildren<Transform>();
        _gameClearUIs[(int)GameClearUI.GameClearPanel].gameObject.SetActive(false);
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

    private void SetGoldCountPanel()
    {
        _goldCountPanelChildrenCount = _inGamePanels[(int)InGamePanel.GoldCntPanel].childCount;
        _goldCountUIs = new Transform[_goldCountPanelChildrenCount];
        for (int i = 0; i < _goldCountPanelChildrenCount; i++)
        {
            _goldCountUIs[i] = _inGamePanels[(int)InGamePanel.GoldCntPanel].GetChild(i);
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

    // 보스 스폰 시간일 때 true인 변수 가져오기
    public bool IsBossSpawnTime()
    {
        return _playerUIs[(int)PlayerPanel.InGameTimer].GetComponent<Transform>().GetComponent<InGameTime>().IsBossSpawnTime;
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

    public void SetGoldCountText()
    {
        _goldCountUIs[(int)GoldCountPanel.GoldCount].GetComponent<TextMeshProUGUI>().text = "X " + _goldCount++.ToString();
    }

    public void OnGameClearPanel()
    {
        Time.timeScale = 0.0f;
        _gameClearUIs[(int)GameClearUI.GameClearPanel].gameObject.SetActive(true);
    }

    public void RecordClearTime(float timer)
    {
        TextMeshProUGUI clearTimeText = _gameClearUIs[(int)GameClearUI.ClearTimeRecord].GetComponent<TextMeshProUGUI>();
        _clearTime = timer;
        float mintue = Mathf.FloorToInt(timer / _oneMinute);
        float second = Mathf.FloorToInt(timer % _oneMinute);

        clearTimeText.text = $"클리어 시간   {mintue.ToString("00") + " : " + second.ToString("00")}";
    }

    private void OnClickRegisterRanking()
    {
        InputField playerNameInput = _gameClearUIs[(int)GameClearUI.RankName].GetComponent<InputField>();
        string playerName = playerNameInput.text;

        // 클리어 기록을 마스터 클라이언트에 전달하여 랭킹 갱신 요청
        GameManager.Instance.PhotonView.RPC("RankingSet", RpcTarget.MasterClient, playerName, _clearTime);
        
        Time.timeScale = 1.0f;
        SoundManager.Instance.PlayFX(SoundKey.ButtonClickSound, 0.04f);
        GameManager.Instance.DestroyGameManager();
        SceneManager.LoadScene("TitleScene");
    }
}