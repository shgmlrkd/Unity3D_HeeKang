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
            
            // �ΰ��� �г� ����
            SetInGamePanel();
            // �÷��̾� UI �г� ����
            SetPlayerPanel();
            // ���� ų �� �г� ����
            SetKillCountPanel();
            // ���� ų �� �ʱ�ȭ
            SetKillCountText();
            // ��� ���� �г� ����
            SetGoldCountPanel();
            // ��� ���� �ʱ�ȭ
            SetGoldCountText();
            // ���� Ŭ���� UI�� �ʱ�ȭ
            SetGameClearPanel();

            _inGamePanels[(int)InGamePanel.SkillPanel].gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        _registerRankingButton = _gameClearUIs[(int)GameClearUI.RankingNameSubmitButton].GetComponent<Button>();
        // ��ŷ ��� Ȯ�� ��ư Ŭ�� �̺�Ʈ
        _registerRankingButton.onClick.AddListener(() => OnClickRegisterRanking());
    }

    // InGameCanvas ������ �гε��� �迭�� �����ϴ� �Լ�
    private void SetInGamePanel()
    {
        // InGameCanvas�� �ڽ� ���� ����
        _panelChildrenCount = _inGameCanvas.childCount;
        // �ʱ�ȭ
        _inGamePanels = new Transform[_panelChildrenCount];
        // �ڽ�(�г�)�� �迭�� ����
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

    // PlayerPanel�� �ڽ� UI���� �����ϴ� �Լ�
    private void SetPlayerPanel()
    {
        // PlayerPanel�� �ڽ� ���� ����
        _playerPanelChildrenCount = _inGamePanels[(int)InGamePanel.PlayerPanel].childCount;
        // �ʱ�ȭ
        _playerUIs = new Transform[_playerPanelChildrenCount];
        // �� UI�� �迭�� ����
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

    // �÷��̾� ü�¹ٿ� ���õ� ��� UI ���
    public Transform[] GetPlayerHpBarUI()
    {
        return _playerUIs[(int)PlayerPanel.HpBar].GetComponentsInChildren<Transform>();
    }

    // �÷��̾� ����ġ�ٿ� ���õ� ��� UI ���
    public Transform[] GetPlayerExpBarUI()
    {
        return _playerUIs[(int)PlayerPanel.ExpBar].GetComponentsInChildren<Transform>();
    }

    // �ΰ��� �ð� ���� ��������
    public float GetInGameTimer()
    {
        return _playerUIs[(int)PlayerPanel.InGameTimer].GetComponent<Transform>().GetComponent<InGameTime>().InGameTimer;   
    }

    // ���� ���� �ð��� �� true�� ���� ��������
    public bool IsBossSpawnTime()
    {
        return _playerUIs[(int)PlayerPanel.InGameTimer].GetComponent<Transform>().GetComponent<InGameTime>().IsBossSpawnTime;
    }

    // ��ų �г� Ȱ��ȭ
    public void SkillPanelOn()
    {
        _inGamePanels[(int)InGamePanel.SkillPanel].gameObject.SetActive(true);
    }

    // ��ų �г� ��Ȱ��ȭ
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

        clearTimeText.text = $"Ŭ���� �ð�   {mintue.ToString("00") + " : " + second.ToString("00")}";
    }

    private void OnClickRegisterRanking()
    {
        InputField playerNameInput = _gameClearUIs[(int)GameClearUI.RankName].GetComponent<InputField>();
        string playerName = playerNameInput.text;

        // Ŭ���� ����� ������ Ŭ���̾�Ʈ�� �����Ͽ� ��ŷ ���� ��û
        GameManager.Instance.PhotonView.RPC("RankingSet", RpcTarget.MasterClient, playerName, _clearTime);
        
        Time.timeScale = 1.0f;
        SoundManager.Instance.PlayFX(SoundKey.ButtonClickSound, 0.04f);
        GameManager.Instance.DestroyGameManager();
        SceneManager.LoadScene("TitleScene");
    }
}