using UnityEngine;

[System.Serializable]
public struct PoolData
{
    public string name;
    public int size;
}

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
    private PoolData[] _weaponPools;
    [SerializeField] 
    private PoolData[] _monsterPools;
    [SerializeField] 
    private PoolData[] _itemPools;

    void Awake()
    {
        _instance = this;
        SpawnPlayer();
    }

    private void Start()
    {
        // ����(��ų)��
        foreach (PoolData  pool in _weaponPools)
            WeaponManager.Instance.CreateWeapons(pool.size, pool.name);

        // ���͵�
        foreach (PoolData pool in _monsterPools)
            MonsterManager.Instance.CreateMonsters(pool.size, pool.name);

        // �����۵�
        foreach (PoolData pool in _itemPools)
            ItemManager.Instance.CreateItems(pool.size, pool.name);
    }

    private void SpawnPlayer()
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/Player");

        _player = Instantiate(prefab);
    }
}