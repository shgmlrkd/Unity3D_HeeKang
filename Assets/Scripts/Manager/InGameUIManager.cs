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

        // �ΰ��� �г� ����
        SetInGamePanel();
        // �÷��̾� UI �г� ����
        SetPlayerPanel();
        // ��ų UI �г� ����
        SetSkillPanel();
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

    // SkillPanel�� �ڽ� UI���� �����ϰ� ��ų ��ư�� �������� �����ϴ� �Լ�
    private void SetSkillPanel()
    {
        // SkillPanel�� �ڽ� ���� ����
        _skillPanelChildrenCount = _inGamePanels[(int)InGamePanel.SkillPanel].childCount;
        // �ʱ�ȭ
        _skillUIs = new Transform[_skillPanelChildrenCount];
        // �� UI�� �迭�� ����
        for (int i = 0; i < _skillPanelChildrenCount; i++)
        {
            _skillUIs[i] = _inGamePanels[(int)InGamePanel.SkillPanel].GetChild(i);
        }

        // ������ ����
        GameObject skillBtnPrefab = Resources.Load<GameObject>("Prefabs/UI/SkillUIButton");

        // ��ų ��ư 3�� �߰�
        for(int i = 0; i < _cardButtonCount; i++)
        {
            GameObject skillBtn = Instantiate(skillBtnPrefab, _skillUIs[(int)SkillPanel.SkillPanel]);
        }
        // ��ų �г� ó���� ��Ȱ��ȭ
        _inGamePanels[(int)InGamePanel.SkillPanel].gameObject.SetActive(false);
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

    // ��ų ���õ� ��� UI ���
    public Transform[] GetSkillButtonUI()
    {
        return _skillUIs[(int)SkillPanel.SkillPanel].GetComponentsInChildren<Transform>();
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