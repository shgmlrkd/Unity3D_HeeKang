using UnityEngine;

public class Status
{
    // 캐릭터 공용
    // 체력
    private float _maxHp;
    public float MaxHp
    {
        get { return _maxHp; }
        set { _maxHp = value; }
    }
    // 스피드
    private float _speed;
    public float Speed
    {
        get { return _speed; }
        set { _speed = value; }
    }
    // 경험치
    private float _exp;
    public float Exp
    {
        get { return _exp; }
        set { _exp = value; }
    }

    // 몬스터
    // 회전 속도 (플레이어 바라보는 속도)
    private float _rotSpeed;
    public float RotSpeed
    {
        get { return _rotSpeed; }
        set { _rotSpeed = value; }
    }
    // 공격력
    private float _attackPower;
    public float AttackPower
    {
        get { return _attackPower; }
        set { _attackPower = value; }
    }
    // 공격 간격
    private float _attackInterval;
    public float AttackInterval
    {
        get { return _attackInterval; }
        set { _attackInterval = value; }
    }
    // 공격 가능한 거리
    private float _attackDistance;
    public float AttackDistance
    {
        get { return _attackDistance; }
        set { _attackDistance = value; }
    }
    // 원거리 공격 시 투사체 생존 시간
    private float _lifeTime;
    public float LifeTime
    {
        get { return _lifeTime; }
        set { _lifeTime = value; }
    }
    // 몬스터 시간 지날 때 이거 기준으로 HP, ATK 증가
    private float _stateScaleFactor;
    public float StateScaleFactor
    {
        get { return _stateScaleFactor; }
        set { _stateScaleFactor = value; }
    }
    // 몬스터 시간 지날 때 이거 기준으로 갱신 가능
    private float _statUpdateInterval;
    public float StatUpdateInterval
    {
        get { return _statUpdateInterval; }
        set { _statUpdateInterval = value; }
    }
    private float _range;
    public float Range
    {
        get { return _range; }
    }

    // 몬스터 처음 스폰 시간
    private float _spawnStartTime;
    public float SpawnStartTime
    {
        get { return _spawnStartTime; }
    }
    // 타입 (근거리, 원거리)
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
    }

    // 몬스터 Status 초기화
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
        _range = mosnterSpawnData.SpawnRange;
        _spawnStartTime = mosnterSpawnData.SpawnStartTime;
    }
}
