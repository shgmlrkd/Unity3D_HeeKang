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
    private float _speed;
    public float Speed
    { 
        get { return _speed; } 
    }
    private float _maxHp;
    public float MaxHp
    {
        get { return _maxHp; }
    }

   /* private int _hpGold;
    private int _speedGold;
    private int _attackPowerGold;
    private int _attackSpeedGold;*/

    private int _expLevel = 0;
    public int ExpLevel
    {
        get { return _expLevel; }
    }   

    /*// 강화 레벨에 따라서 스텟이 정해짐 (enchantLevel)
    private int _hpLevel = 1;
    private int _attackPowerLevel = 1;
    private int _attackSpeedLevel = 1;
    private int _speedLevel = 1;*/

    private void Start()
    {
        //_playerData = PlayerDataManager.Instance.GetPlayerDataByStatLevel(_hpLevel, _expLevel, _attackPowerLevel, _attackSpeedLevel, _speedLevel);
        _playerData = PlayerDataManager.Instance.GetPlayerData(GameManager.Instance.PlayerKey);
        _playerStatus = new Status(_playerData);
        _maxExp = _playerStatus.Exp;
    }

   /* private void SetPlayerGoldData()
    {
        // 강화 할 때 필요한 골드
        _hpGold = _playerData.HpGold;
        _speedGold = _playerData.SpeedGold;
        _attackPowerGold = _playerData.AttackPowerGold;
        _attackSpeedGold = _playerData.AttackSpeedGold;
    }*/

    public void LevelUp()
    {
        _expLevel++;

        _playerData = PlayerDataManager.Instance.GetPlayerData(GameManager.Instance.PlayerKey + _expLevel);

        _playerStatus.MaxHp = _playerData.Hp;
        _maxExp = _playerData.Exp;
        _playerStatus.Speed = _playerData.Speed;


        // 스킬 패널 열기
        InGameUIManager.Instance.SkillPanelOn();
        // 시간 멈춤
        Time.timeScale = 0;
    }
}