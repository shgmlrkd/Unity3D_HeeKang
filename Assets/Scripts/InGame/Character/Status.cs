using UnityEngine;

public class Status
{
    // ĳ���� ����
    // ü��
    private float _maxHp;
    public float MaxHp
    {
        get { return _maxHp; }
        set { _maxHp = value; }
    }
    // ���ǵ�
    private float _speed;
    public float Speed
    {
        get { return _speed; }
        set { _speed = value; }
    }
    // ����ġ
    private float _exp;
    public float Exp
    {
        get { return _exp; }
        set { _exp = value; }
    }

    // ����
    // ȸ�� �ӵ� (�÷��̾� �ٶ󺸴� �ӵ�)
    private float _rotSpeed;
    public float RotSpeed
    {
        get { return _rotSpeed; }
        set { _rotSpeed = value; }
    }
    // ���ݷ�
    private float _attackPower;
    public float AttackPower
    {
        get { return _attackPower; }
        set { _attackPower = value; }
    }
    // ���� ����
    private float _attackInterval;
    public float AttackInterval
    {
        get { return _attackInterval; }
        set { _attackInterval = value; }
    }
    // ���� ������ �Ÿ�
    private float _attackDistance;
    public float AttackDistance
    {
        get { return _attackDistance; }
        set { _attackDistance = value; }
    }
    // ���Ÿ� ���� �� ����ü ���� �ð�
    private float _lifeTime;
    public float LifeTime
    {
        get { return _lifeTime; }
        set { _lifeTime = value; }
    }
    // ���� �ð� ���� �� �̰� �������� HP, ATK ����
    private float _stateScaleFactor;
    public float StateScaleFactor
    {
        get { return _stateScaleFactor; }
        set { _stateScaleFactor = value; }
    }
    // ���� �ð� ���� �� �̰� �������� ���� ����
    private float _statUpdateInterval;
    public float StatUpdateInterval
    {
        get { return _statUpdateInterval; }
        set { _statUpdateInterval = value; }
    }
    // ���� ó�� ���� �ð�
    private float _spawnStartTime;
    public float SpawnStartTime
    {
        get { return _spawnStartTime; }
    }
    // Ÿ�� (�ٰŸ�, ���Ÿ�)
    private int _type;
    public int Type
    {
        get { return _type; }
        set { _type = value; }
    }

    // ĳ���� Status �ʱ�ȭ
    public Status(PlayerData data)
    {
        _maxHp = data.Hp;
        _exp = data.Exp;
        _speed = data.Speed;
    }

    // ���� Status �ʱ�ȭ
    public Status(MonsterData monsterData, MonsterSpawnData mosnterSpawnData)
    {
        _maxHp = monsterData.Hp;
        _speed = monsterData.MoveSpeed;
        _rotSpeed = monsterData.RotateSpeed;
        _attackPower = monsterData.AttackPower;
        _attackInterval = monsterData.AttackInterval;
        _attackDistance = monsterData.AttackDistance;
        _lifeTime = monsterData.LifeTime;
        _stateScaleFactor = monsterData.StatScaleFactor;
        _statUpdateInterval = monsterData.StatUpdateInterval;
        _exp = monsterData.Exp;
        _spawnStartTime = mosnterSpawnData.SpawnStartTime;
    }
}
