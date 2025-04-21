using UnityEngine;

public class Player : MonoBehaviour
{
    protected PlayerData _playerData;

    protected float _maxExp;
    protected float _curExp = 0.0f;
    protected float _maxHp;
    protected float _curHp;
    protected float _speed;
    protected float _attackPowerRate;
    protected float _attackSpeedRate;

    protected int _hpGold;
    protected int _speedGold;
    protected int _attackPowerGold;
    protected int _attackSpeedGold;

    protected int _expLevel = 1;
    protected int _hpLevel = 1;
    protected int _attackPowerLevel = 1;
    protected int _attackSpeedLevel = 1;
    protected int _speedLevel = 1;

    protected void Start()
    {
        _playerData = PlayerDataManager.Instance.GetPlayerDataByStatLevel(_hpLevel, _attackPowerLevel, _attackSpeedLevel, _speedLevel);
        _maxExp = PlayerDataManager.Instance.GetPlayerTotalExpToLevel(_expLevel);
        SetPlayerData();
    }

    protected void SetPlayerData()
    {
        // 플레이어 스탯
        _maxHp = _playerData.Hp;
        _curHp = _maxHp;
        _speed = _playerData.Speed;
        _attackPowerRate = _playerData.AttackPowerRate;
        _attackSpeedRate = _playerData.AttackSpeedRate;

        // 강화 할 때 필요한 골드
        _hpGold = _playerData.HpGold;
        _speedGold = _playerData.SpeedGold;
        _attackPowerGold = _playerData.AttackPowerGold;
        _attackSpeedGold = _playerData.AttackSpeedGold;
    }

    protected void LevelUp()
    {
        _expLevel++;
        _maxExp = PlayerDataManager.Instance.GetPlayerTotalExpToLevel(_expLevel);
        // 레벨업 했으니 스킬 3개 중 1개 선택하는거 구현 해야함
    }
}
