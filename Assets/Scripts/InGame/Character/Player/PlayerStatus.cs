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
    private int _expLevel = 0;
    public int ExpLevel
    {
        get { return _expLevel; }
    }   

    private void Start()
    {
        _playerData = PlayerDataManager.Instance.GetPlayerData(GameManager.Instance.PlayerKey);
        _playerStatus = new Status(_playerData);
        _maxExp = _playerStatus.Exp;
    }

    public void LevelUp()
    {
        _expLevel++;

        _playerData = PlayerDataManager.Instance.GetPlayerData(GameManager.Instance.PlayerKey + _expLevel);

        _playerStatus.MaxHp = _playerData.Hp;
        _maxExp = _playerData.Exp;
        _playerStatus.Speed = _playerData.Speed;


        // Ω∫≈≥ ∆–≥Œ ø≠±‚
        InGameUIManager.Instance.SkillPanelOn();
        // Ω√∞£ ∏ÿ√„
        Time.timeScale = 0;
    }
}