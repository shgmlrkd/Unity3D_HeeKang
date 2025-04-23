using UnityEngine;

public class Status
{
    private float _maxHp;
    public float MaxHp
    {
        get { return _maxHp; }
        set { _maxHp = value; }
    }
    private float _speed;
    public float Speed
    {
        get { return _speed; }
        set { _speed = value; }
    }
    private float _rotSpeed;
    public float RotSpeed
    {
        get { return _rotSpeed; }
        set { _rotSpeed = value; }
    }
    private float _attackPower;
    public float AttackPower
    {
        get { return _attackPower; }
        set { _attackPower = value; }
    }
    private float _attackInterval;
    public float AttackInterval
    {
        get { return _attackInterval; }
        set { _attackInterval = value; }
    }
    private float _attackSpeed;
    public float AttackSpeed
    {
        get { return _attackSpeed; }
        set { _attackSpeed = value; }
    }
    private float _attackDistance;
    public float AttackDistance
    {
        get { return _attackDistance; }
        set { _attackDistance = value; }
    }
    private float _lifeTime;
    public float LifeTime
    {
        get { return _lifeTime; }
        set { _lifeTime = value; }
    }
    private float _stateScaleFactor;
    public float StateScaleFactor
    {
        get { return _stateScaleFactor; }
        set { _stateScaleFactor = value; }
    }
    private float _exp;
    public float Exp
    {
        get { return _exp; }
        set { _exp = value; }
    }
    private int _type;
    public int Type
    {
        get { return _type; }
        set { _type = value; }
    }

    // 캐릭터 Status 초기화
    public Status(PlayerData data)
    {
        _maxHp = data.Hp;
        _exp = data.Exp;
        _speed = data.Speed;
        _attackPower = data.AttackPowerRate;
        _attackSpeed = data.AttackSpeedRate;
    }

    // 몬스터 Status 초기화
    public Status(MonsterData data)
    {
        _maxHp = data.Hp;
        _speed = data.MoveSpeed;
        _rotSpeed = data.RotateSpeed;
        _attackPower = data.AttackPower;
        _attackInterval = data.AttackInterval;
        _attackDistance = data.AttackDistance;
        _lifeTime = data.LifeTime;
        _stateScaleFactor = data.StatScaleFactor;
        _exp = data.Exp;
        _type = data.Type;
    }
}
