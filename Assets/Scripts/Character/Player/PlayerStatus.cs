using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    private Status _playerStatus;

    private PlayerData _playerData;

    private float _maxExp;
    public float MaxExp
    {
        get { return _maxExp; }
    }
    private float _maxHp;
    public float MaxHp
    {
        get { return _maxHp; }
    }
    private float _speed;
    public float Speed
    {
        get { return _speed; }
    }
    private float _attackPowerRate;
    public float AttackPowerRate
    {
        get { return _attackPowerRate; }
    }
    private float _attackSpeedRate;
    public float AttackSpeedRate
    {
        get { return _attackSpeedRate; }
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

    private int _hpLevel = 1;
    private int _attackPowerLevel = 1;
    private int _attackSpeedLevel = 1;
    private int _speedLevel = 1;

    private void Start()
    {
        _playerData = PlayerDataManager.Instance.GetPlayerDataByStatLevel(_hpLevel, _expLevel, _attackPowerLevel, _attackSpeedLevel, _speedLevel);
        _playerStatus = new Status(_playerData);
        _maxExp = _playerStatus.Exp;
        SetPlayerData();
    }

    private void SetPlayerData()
    {
        // 플레이어 스탯
        _maxHp = _playerData.Hp;
        _speed = _playerData.Speed;
        _attackPowerRate = _playerData.AttackPowerRate;
        _attackSpeedRate = _playerData.AttackSpeedRate;

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
    }
}
