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
    private int _monsterPoolSize;
    [SerializeField]
    private int _bulletPoolSize; 
    /*[SerializeField]
    private int _kunaiPoolSize; 
    [SerializeField]
    private int _fireBallPoolSize;*/

    void Awake()
    {
        _instance = this;
        /*
        WeaponManager.Instance.CreateWeapons(_kunaiPoolSize, "Kunai");
        WeaponManager.Instance.CreateWeapons(_fireBallPoolSize, "FireBall");*/
        SpawnPlayer();
    }

    private void Start()
    {
        WeaponManager.Instance.CreateWeapons(_bulletPoolSize, "Bullet");
        MonsterManager.Instance.CreateMonsters(_monsterPoolSize, "Skeleton");
        ItemManager.Instance.CreateItems(_monsterPoolSize, "Exp");
    }

    private void SpawnPlayer()
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/Player");

        _player = Instantiate(prefab);
    }
}