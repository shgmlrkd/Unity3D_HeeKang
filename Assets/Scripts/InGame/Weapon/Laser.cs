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
        // ��ƼŬ�� �÷��� ������ _weaponLifeTimer �� ������Ʈ ��Ȱ��ȭ
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

        // ��ƼŬ �÷���
        foreach (ParticleSystem laserParticle in _laserParticles)
        {
            laserParticle.Play();
        }

        _isPlayParticle = true;

        // ��ġ, ������ ����
        transform.position = pos;
        _weaponLifeTimer = data.LifeTime;
        _weaponAttackPower = data.AttackPower;
    }
}