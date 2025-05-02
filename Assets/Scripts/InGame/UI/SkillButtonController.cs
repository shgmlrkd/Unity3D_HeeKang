using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillButtonController : MonoBehaviour
{
    private Transform[] _skillBtns;
    private List<int> _skillsLevel;
    private List<int> _skillKeysCopy; // ��ų Ű �ε��� ������ ����
    private Dictionary<int, int> _skillLevelDict; // Ű���� ���̺� Ű��, �μ��� ����

    private readonly int _skillStartKey = 300;

    private int _skillCount; // ��ų �� ����
    private int _skillMaxLevel; // ��ų ���� = 5
    private int _skillBtnCount; // ��ų ��ư ���� = 3

    private void Awake()
    {
        // �ߺ� ���� ���� Ű�� ������ List
        _skillKeysCopy = new List<int>();
        // �� ��ų�� Ű�� ���� �뵵
        _skillLevelDict = new Dictionary<int, int>();
    }

    private void OnEnable()
    {
        // Start���� ��ư ���� ���ϸ� ����
        if (_skillBtnCount > 0)
        {
            SkillRandomKeys();
            SetSkillUI();
        }
    }

    private void Start()
    {
        // ��ų ���� ��������
        _skillCount = WeaponDataManager.Instance.WeaponCount;
        _skillMaxLevel = WeaponDataManager.Instance.WeaponMaxLevel;
        _skillsLevel = InGameManager.Instance.Player.GetComponent<PlayerSkill>().SkillsLevel;

        for (int i = 0; i < _skillCount; i++)
        {
            int skillKeyIndex = _skillStartKey + (i * _skillMaxLevel);
            _skillLevelDict.Add(i, skillKeyIndex + _skillsLevel[i]);
        }

        // ��ų ��ư ���� ��������
        _skillBtnCount = GetComponent<Transform>().childCount;
        // �ʱ�ȭ
        _skillBtns = new Transform[_skillBtnCount];

        // ��ų ��ư 3�� ����
        for (int i = 0; i < _skillBtnCount; i++)
        {
            _skillBtns[i] = transform.GetChild(i);
            int index = i;
            _skillBtns[i].GetComponent<Button>().onClick.AddListener(() => OnButtonClick(index));
        }
    }

    private void OnButtonClick(int index)
    {
        // Ŭ���� ��ų ������
        SkillLevelUp(_skillKeysCopy[index]);
        // ��ų ���� â �ݱ� �� ���� �簳
        InGameUIManager.Instance.SkillPanelOff();
        Time.timeScale = 1;
    }

    private void SkillRandomKeys()
    {
        // �ʱ�ȭ
        _skillKeysCopy.Clear();

        // ����Ʈ ��ųʸ��� �ִ� Ű���� ����Ʈ�� ���ҷ� �ʱ�ȭ
        List<int> availableSkillKeys = new List<int>(_skillLevelDict.Keys);
        HashSet<int> selectedKeys = new HashSet<int>();

        // �ߺ� ���� ���� Ű�� �̱�
        while (selectedKeys.Count < _skillBtnCount)
        {
            // ��ų ���� �� �������� �ε��� ����
            int randIndex = Random.Range(0, availableSkillKeys.Count);
            // ��ų Ű��
            int key = availableSkillKeys[randIndex];

            if (IsSkillMaxLevel(key))
            {
                continue; // ������ ��ų�� �н�
            }

            if (selectedKeys.Add(key))
            {
                _skillKeysCopy.Add(key);
            }
        }
    }

    private bool IsSkillMaxLevel(int key)
    {
        int curLevel = _skillLevelDict[key];

        // �� ��ų�� ���� �׷��� �ִ� Ű ���
        int skillMaxKey = _skillStartKey + (key + 1) * _skillMaxLevel - 1;

        // ���� ���� �Ǵ�
        return curLevel > skillMaxKey;
    }

    private void SetSkillUI()
    {
        _skillsLevel = InGameManager.Instance.Player.GetComponent<PlayerSkill>().SkillsLevel;

        // ��ư �̹���, �ؽ�Ʈ �ٲٴ� �۾�
        for (int i = 0; i < _skillBtnCount; i++)
        {
            int key = _skillLevelDict[_skillKeysCopy[i]];
            int levelKey = _skillsLevel[_skillKeysCopy[i]] + 1;
            _skillBtns[i].GetComponent<SkillButton>().SetSkillUI(key, levelKey);
        }
    }

    private void SkillLevelUp(int key)
    {
        int skillKey = _skillLevelDict[key];
        InGameManager.Instance.Player.GetComponent<PlayerSkill>().PlayerSkillUnlockOrLevelUp(skillKey);
        _skillLevelDict[key]++;
    }
}