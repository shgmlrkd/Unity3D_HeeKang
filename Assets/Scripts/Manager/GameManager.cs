using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get { return _instance; }
    }

    private GameObject _player;
    public GameObject Player
    { get { return _player; } }

    [SerializeField]
    private int _skeletonPoolSize; 
    [SerializeField]
    private int _slimePoolSize;
    [SerializeField]
    private int _turtlePoolSize;
    [SerializeField]
    private int _expPoolSize;
    [SerializeField]
    private int _bulletPoolSize;
    [SerializeField]
    private int _kunaiPoolSize;
    [SerializeField]
    private int _laserPoolSize; 
    [SerializeField]
    private int _fireBallPoolSize;

    void Awake()
    {
        _instance = this;
        SpawnPlayer();
    }

    private void Start()
    {
        WeaponManager.Instance.CreateWeapons(_bulletPoolSize, "Bullet");
        WeaponManager.Instance.CreateWeapons(_kunaiPoolSize, "Kunai");
        WeaponManager.Instance.CreateWeapons(_laserPoolSize, "Laser");
        WeaponManager.Instance.CreateWeapons(_fireBallPoolSize, "FireBall");
        MonsterManager.Instance.CreateMonsters(_skeletonPoolSize, "Skeleton");
        MonsterManager.Instance.CreateMonsters(_slimePoolSize, "Slime");
        MonsterManager.Instance.CreateMonsters(_turtlePoolSize, "TurtleShell");
        ItemManager.Instance.CreateItems(_expPoolSize, "Exp");
    }

    private void SpawnPlayer()
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/Player");

        _player = Instantiate(prefab);
    }
}