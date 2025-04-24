using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    private Status _playerStatus;
    public Status Status 
    { 
        get { return _playerStatus; }
    }

    private PlayerData _playerData;

    private float _maxExp;
    public float MaxExp
    { 
        get { return _maxExp; }
    }

    private int _hpGold;
    private int _speedGold;
    private int _attackPowerGold;
    private int _attackSpeedGold;

    private int _expLevel = 1;
    public int ExpLevel
    {
        get { return _expLevel; }
    }   

    // 강화 레벨에 따라서 스텟이 정해짐 (enchantLevel)
    private int _hpLevel = 1;
    private int _attackPowerLevel = 1;
    private int _attackSpeedLevel = 1;
    private int _speedLevel = 1;

    private void Start()
    {
        _playerData = PlayerDataManager.Instance.GetPlayerDataByStatLevel(_hpLevel, _expLevel, _attackPowerLevel, _attackSpeedLevel, _speedLevel);
        _playerStatus = new Status(_playerData);
        _maxExp = _playerStatus.Exp;
        //SetPlayerData();
    }

    private void Update()
    {
        // 스킬 패널 닫기 (임시 아무것도 아님)
        if (Input.GetMouseButtonDown(0))
        {
            InGameUIManager.Instance.SkillPanelOff();
            Time.timeScale = 1;
        }
    }

    private void SetPlayerGoldData()
    {
        // 강화 할 때 필요한 골드
        _hpGold = _playerData.HpGold;
        _speedGold = _playerData.SpeedGold;
        _attackPowerGold = _playerData.AttackPowerGold;
        _attackSpeedGold = _playerData.AttackSpeedGold;
    }

    public void LevelUp()
    {
        _expLevel++;
        _maxExp = PlayerDataManager.Instance.GetPlayerTotalExpToLevel(_expLevel);

        // 레벨업 했으니 스킬 3개 중 1개 선택하는거 구현 해야함


        // 스킬 패널 열기
        InGameUIManager.Instance.SkillPanelOn();
        // 시간 멈춤
        Time.timeScale = 0;
    }
}
