using UnityEngine;

public class Weapon : MonoBehaviour
{
    protected Vector3 _direction;

    protected float _weaponAttackPower = 0.0f;
    public float WeaponAttackPower
    {
        get { return _weaponAttackPower; }
    }
    protected float _weaponSpeed = 0.0f;
    protected float _weaponLifeTimer = 0.0f;
    protected float _timer = 0.0f;
    protected float _weaponPierce = 0.0f;
    protected float _weaponProjectileCount = 0.0f;
    protected float _weaponColliderRadius;

    /*// 가장 가까운 몬스터 충돌체 가져오기
    protected Collider ClosestMonsterCollider(float range)
    {
        Collider[] monsterColliders = Physics.OverlapSphere(transform.position, range, LayerMask.GetMask("Monster"));

        Collider closestMonsterCollider = null;
        float minDist = float.MaxValue;

        foreach (Collider collider in monsterColliders)
        {
            float dist = Vector3.Distance(transform.position, collider.transform.position);

            if (dist <= minDist)
            {
                minDist = dist;
                closestMonsterCollider = collider;
            }
        }

        return closestMonsterCollider;
    }*/

    protected void LifeTimer()
    {
        if (gameObject.activeSelf)
        {
            _timer += Time.deltaTime;

            if (_timer >= _weaponLifeTimer)
            {
                _timer -= _weaponLifeTimer;
                gameObject.SetActive(false);
            }
        }
    }
}
