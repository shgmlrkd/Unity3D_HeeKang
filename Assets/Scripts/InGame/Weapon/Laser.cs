using UnityEngine;

public class Laser : Weapon
{
    private ParticleSystem[] _laserParticles;

    private bool _isPlayParticle = false;

    private void OnEnable()
    {
        base.OnEnable();
        _laserParticles = GetComponentsInChildren<ParticleSystem>();
        _isPlayParticle = false;
    }

    private void Update()
    {
        // 파티클을 플레이 했으면 _weaponLifeTimer 후 오브젝트 비활성화
        if(_isPlayParticle)
        {
            _timer += Time.deltaTime;

            if(_timer >= _weaponLifeTimer)
            {
                gameObject.SetActive(false);
            }
        }
    }

    public void Fire(Vector3 pos, WeaponData data)
    {
        gameObject.SetActive(true);

        // 파티클 플레이
        foreach (ParticleSystem laserParticle in _laserParticles)
        {
            laserParticle.Play();
        }

        _isPlayParticle = true;

        // 위치, 데이터 세팅
        transform.position = pos;
        _weaponLifeTimer = data.LifeTime;
        _weaponAttackPower = data.AttackPower;
    }
}