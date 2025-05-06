using TMPro;
using UnityEngine;

public class InGameUIManager : Singleton<InGameUIManager>
{
    private enum InGamePanel
    {
        PlayerPanel = 2, SkillPanel, KillCntPanel, GoldCntPanel
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

    private Transform[] _inGamePanels;
    private Transform[] _playerUIs;
    private Transform[] _killCountUIs;
    private Transform[] _goldCountUIs;
    private Transform _inGameCanvas;

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
            _inGamePanels[(int)InGamePanel.SkillPanel].gameObject.SetActive(false);
        }
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
}