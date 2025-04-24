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

    // ��ȭ ������ ���� ������ ������ (enchantLevel)
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
        // ��ų �г� �ݱ� (�ӽ� �ƹ��͵� �ƴ�)
        if (Input.GetMouseButtonDown(0))
        {
            InGameUIManager.Instance.SkillPanelOff();
            Time.timeScale = 1;
        }
    }

    private void SetPlayerGoldData()
    {
        // ��ȭ �� �� �ʿ��� ���
        _hpGold = _playerData.HpGold;
        _speedGold = _playerData.SpeedGold;
        _attackPowerGold = _playerData.AttackPowerGold;
        _attackSpeedGold = _playerData.AttackSpeedGold;
    }

    public void LevelUp()
    {
        _expLevel++;
        _maxExp = PlayerDataManager.Instance.GetPlayerTotalExpToLevel(_expLevel);

        // ������ ������ ��ų 3�� �� 1�� �����ϴ°� ���� �ؾ���


        // ��ų �г� ����
        InGameUIManager.Instance.SkillPanelOn();
        // �ð� ����
        Time.timeScale = 0;
    }
}
