using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : Singleton<WeaponManager>
{
    public void CreateWeapons(int poolSize, string key)
    {
        GameObject weaponPrefab = Resources.Load<GameObject>("Prefabs/Weapons/" + key);
        PoolingManager.Instance.Add(key, poolSize, weaponPrefab, transform);
    }

    public List<GameObject> GetObjects(string key)
    {
        return PoolingManager.Instance.GetObjects(key);
    }

    // 플레이어 무기
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

    public void LaserFire(Vector3 pos, WeaponData data)
    {
        GameObject laser = PoolingManager.Instance.Pop("Laser");
        laser.GetComponent<Laser>().Fire(pos, data);
    }

    public void ShootFireBall(Vector3 pos, Vector3 dir, WeaponData data)
    {
        GameObject fireBall = PoolingManager.Instance.Pop("FireBall");
        fireBall.GetComponent<FireBall>().Fire(pos, dir, data);
    }

    public void StartAxeSpin(Transform playerTransform, Vector3 pos, WeaponData data)
    {
        GameObject axe = PoolingManager.Instance.Pop("Axe");
        axe.GetComponent<Axe>().SpinAround(playerTransform, pos, data);
    }

    public void ThrowSpinningSword(Vector3 pos, Vector3 dir, WeaponData data)
    {
        GameObject sword = PoolingManager.Instance.Pop("Sword");
        sword.GetComponent<Sword>().Fire(pos, dir, data);
    }

    // 몬스터 무기
    public void ShootLichFireBall(Vector3 pos, Vector3 dir, MonsterWeaponData data)
    {
        GameObject lichFireBall = PoolingManager.Instance.Pop("LichFireBall");
        lichFireBall.GetComponent<MonsterFireBall>().Fire(pos, dir, data);
    }
}