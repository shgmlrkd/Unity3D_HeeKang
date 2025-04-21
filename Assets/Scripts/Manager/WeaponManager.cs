using UnityEngine;

public class WeaponManager : Singleton<WeaponManager>
{
    public void CreateWeapons(int poolSize, string key)
    {
        GameObject weaponPrefab = Resources.Load<GameObject>("Prefabs/Weapons/" + key);
        PoolingManager.Instance.Add(key, poolSize, weaponPrefab, transform);
    }

    public void BulletFire(Vector3 pos, Vector3 dir, WeaponData data)
    {
        GameObject bullet = PoolingManager.Instance.Pop("Bullet");
        bullet.GetComponent<Bullet>().Fire(pos, dir, data);
    }

    public void KunaiFire(Vector3 pos, Vector3 dir, WeaponData data)
    {
        GameObject kunai = PoolingManager.Instance.Pop("Kunai");
        kunai.GetComponent<Kunai>().Fire(pos, dir, data);
    }
}
