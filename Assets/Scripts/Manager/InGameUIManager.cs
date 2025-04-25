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

    private Transform[] _inGamePanels;
    private Transform[] _playerUIs;
    private Transform _inGameCanvas;

    private int _panelChildrenCount;
    private int _playerPanelChildrenCount;

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
}