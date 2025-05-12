using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillButtonController : MonoBehaviour
{
    private Transform[] _skillBtns;
    private List<int> _skillsLevel;
    private List<int> _skillKeysCopy; // 스킬 키 인덱스 복사할 공간
    private Dictionary<int, int> _skillLevelDict; // 키값은 테이블 키값, 인수는 레벨

    private readonly int _skillStartKey = 300;

    private int _skillCount; // 스킬 총 개수
    private int _skillMaxLevel; // 스킬 만렙 = 5
    private int _skillBtnCount; // 스킬 버튼 개수 = 3

    private void Awake()
    {
        // 중복 없는 랜덤 키값 복사할 List
        _skillKeysCopy = new List<int>();
        // 각 스킬의 키값 저장 용도
        _skillLevelDict = new Dictionary<int, int>();
    }

    private void OnEnable()
    {
        // Start에서 버튼 개수 구하면 실행
        if (_skillBtnCount > 0)
        {
            SkillRandomKeys();
            SetSkillUI();
        }
    }

    private void Start()
    {
        // 스킬 개수 가져오기
        _skillCount = WeaponDataManager.Instance.WeaponCount;
        _skillMaxLevel = WeaponDataManager.Instance.WeaponMaxLevel;
        _skillsLevel = InGameManager.Instance.Player.GetComponent<PlayerSkill>().SkillsLevel;

        for (int i = 0; i < _skillCount; i++)
        {
            int skillKeyIndex = _skillStartKey + (i * _skillMaxLevel);
            _skillLevelDict.Add(i, skillKeyIndex + _skillsLevel[i]);
        }

        // 스킬 버튼 개수 가져오기
        _skillBtnCount = GetComponent<Transform>().childCount;
        // 초기화
        _skillBtns = new Transform[_skillBtnCount];

        // 스킬 버튼 3개 초기화 및 클릭 이벤트 등록
        for (int i = 0; i < _skillBtnCount; i++)
        {
            _skillBtns[i] = transform.GetChild(i);
            int index = i;
            _skillBtns[i].GetComponent<Button>().onClick.AddListener(() => OnButtonClick(index));
        }
    }

    private void OnButtonClick(int index)
    {
        // 클릭한 스킬 레벨업
        SkillLevelUp(_skillKeysCopy[index]);
        // 스킬 선택 창 닫기 및 게임 재개
        InGameUIManager.Instance.SkillPanelOff();
        Time.timeScale = 1;
    }

    private void SkillRandomKeys()
    {
        // 초기화
        _skillKeysCopy.Clear();

        // 리스트 딕셔너리에 있는 키값을 리스트의 원소로 초기화
        List<int> availableSkillKeys = new List<int>(_skillLevelDict.Keys);
        // 중복 방지를 위해 HashSet 사용
        HashSet<int> selectedKeys = new HashSet<int>();
        // 중복 없는 랜덤 키값 뽑기 (_skillBtnCount는 버튼 수 버튼은 3개)
        while (selectedKeys.Count < _skillBtnCount)
        {
            // 랜덤 인덱스로 스킬 키 선택
            int randIndex = Random.Range(0, availableSkillKeys.Count);
            int key = availableSkillKeys[randIndex];
            // 최대 레벨인 스킬은 제외
            if (IsSkillMaxLevel(key))
            {
                continue; 
            }
            // 중복 없이 선택된 키만 추가
            if (selectedKeys.Add(key))
            {
                _skillKeysCopy.Add(key);
            }
        }
    }

    private bool IsSkillMaxLevel(int key)
    {
        int curLevel = _skillLevelDict[key];

        // 이 스킬이 속한 그룹의 최대 키 계산
        int skillMaxKey = _skillStartKey + (key + 1) * _skillMaxLevel - 1;

        // 만렙 여부 판단
        return curLevel > skillMaxKey;
    }

    private void SetSkillUI()
    {
        _skillsLevel = InGameManager.Instance.Player.GetComponent<PlayerSkill>().SkillsLevel;

        // 버튼 이미지, 텍스트 바꾸는 작업
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