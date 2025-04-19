using UnityEngine;

public class Monster : MonoBehaviour
{
    protected Transform _player;

    protected MonsterData _monsterData;

    protected float _maxHp;
    protected float _curHp;
    protected float _speed;
    protected float _rotSpeed;
    protected float _attackPower;
    protected float _attackInterval;
    protected float _attackDistance;
    protected float _lifeTime;
    protected float _spawnInterval;
    protected float _spawnStartTime;
    protected float _spawnEndTime;
    protected float _stateScaleFactor;

    protected int _exp;
    protected int _type;

    protected void Start()
    {
        _player = GameObject.FindWithTag("Player").GetComponent<Transform>();
    }

    // 몬스터 데이터 세팅
    protected void SetMonsterData(MonsterData monsterData)
    {
        _maxHp = monsterData.Hp;
        _curHp = _maxHp;
        _exp = monsterData.Exp;
        _type = monsterData.Type;
        _speed = monsterData.MoveSpeed; 
        _rotSpeed = monsterData.RotateSpeed;

        _attackPower = monsterData.AttackPower;
        _attackInterval = monsterData.AttackInterval;
        _attackDistance = monsterData.AttackDistance;

        _lifeTime = monsterData.LifeTime;

        _spawnInterval = monsterData.SpawnInterval;
        _spawnStartTime = monsterData.SpawnStartTime;
        _spawnEndTime = monsterData.SpawnEndTime;

        _stateScaleFactor = monsterData.StatScaleFactor;
    }
}