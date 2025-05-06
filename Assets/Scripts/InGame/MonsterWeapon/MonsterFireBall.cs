using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class MonsterFireBall : FireBall
{
    public void Fire(Vector3 pos, Vector3 dir, MonsterWeaponData data)
    {
        gameObject.SetActive(true);

        pos += _spawnPosYOffset;
        transform.position = pos;
        _direction = dir.normalized;
        _weaponSpeed = data.AttackSpeed;
        _weaponAttackPower = data.AttackPower;
        _weaponLifeTimer = data.LifeTime;
        _isExplosionParticleTimer = _weaponLifeTimer * 0.1f; // 라이프 타임의 1 / 10
        _direction.y = 0.0f;

        transform.rotation = Quaternion.LookRotation(_direction);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !_isExplosionParticlePlay)
        {
            ExplosionParticleStart();

            other.gameObject.GetComponent<PlayerGetDamage>().GetDamage(_weaponAttackPower);
            DamageTextManager.Instance.ShowDamageText(other.transform, _weaponAttackPower, Color.red);
        }
    }

    protected new void OnTriggerStay(Collider other)
    {
    }
}