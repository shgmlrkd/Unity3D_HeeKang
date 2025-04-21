using UnityEngine;

public class ItemManager : Singleton<ItemManager>
{
    public void CreateItems(int poolSize, string key)
    {
        GameObject weaponPrefab = Resources.Load<GameObject>("Prefabs/Items/" + key);
        PoolingManager.Instance.Add(key, poolSize, weaponPrefab, transform);
    }
}