using UnityEngine;

[System.Serializable]
public struct PoolData
{
    public string name;
    public int size;
}

public class InGameManager : MonoBehaviour
{
    private static InGameManager _instance;
    public static InGameManager Instance
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
    [SerializeField]
    private PoolData _damageTextPool;

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

        // ������ UI
        DamageTextManager.Instance.CreateDamageTexts(_damageTextPool.size, _damageTextPool.name);
    }

    private void SpawnPlayer()
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/Player/" + GameManager.Instance.PlayerName);

        _player = Instantiate(prefab);
    }
}