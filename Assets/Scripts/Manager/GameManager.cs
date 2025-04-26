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
    private PoolData[] weaponPools;
    [SerializeField] 
    private PoolData[] monsterPools;
    [SerializeField] 
    private PoolData[] itemPools;

    void Awake()
    {
        _instance = this;
        SpawnPlayer();
    }

    private void Start()
    {
        // 무기(스킬)들
        foreach (PoolData  pool in weaponPools)
            WeaponManager.Instance.CreateWeapons(pool.size, pool.name);

        // 몬스터들
        foreach (PoolData pool in monsterPools)
            MonsterManager.Instance.CreateMonsters(pool.size, pool.name);

        // 아이템들
        foreach (PoolData pool in itemPools)
            ItemManager.Instance.CreateItems(pool.size, pool.name);
    }

    private void SpawnPlayer()
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/Player");

        _player = Instantiate(prefab);
    }
}