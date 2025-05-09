using System.Collections;
using UnityEngine;

public class Sword : ThrowWeapon
{
    private Material swordMaterial;
    private Renderer swordRenderer;
    private Transform _targetTransform;

    private readonly float _triggerStayAttackInterval = 1.0f;
    private float _triggerStayTimer = 0.0f;

    private int _overlapCount = 0;

    private bool _isCollision = false;

    private void Awake()
    {
        _fadeLerpTime = 0.5f;
    }

    private void OnEnable()
    {
        base.OnEnable();
        _isCollision = false;
        _targetTransform = null;
        _triggerStayTimer = 0.0f;
        transform.localScale = Vector3.one;

        // ���İ� �ʱ�ȭ
        if(swordMaterial != null)
        {
            Color finalColor = swordMaterial.color;
            finalColor.a = _maxAlphaValue;
            swordMaterial.color = finalColor;
        }
    }

    private void Start()
    {
        // �� ���� ������Ʈ �ȿ� �ڽ����� ���� ����
        swordRenderer = transform.GetChild(0).GetComponent<Renderer>();
        swordMaterial = swordRenderer.material;
    }

    private void Update()
    {
        base.Update();

        transform.Rotate(Vector3.up * _weaponRotSpeed * Time.deltaTime);

        // ó�� ���� Ÿ���� ������ Ÿ�� ����
        if (_targetTransform != null && _targetTransform.gameObject.GetComponent<Monster>().MonsterCurHp <= 0)
        {
            _targetTransform = null;
        }
    }

    protected override void Move()
    {
        // ���� �浹���� �ʾҴٸ� _direction ������ ��� �̵�
        if(!_isCollision)
        {
            transform.position += _direction * _weaponSpeed * Time.deltaTime;
        }
        else
        {
            // �浹 �� Ÿ�� Transform�� ã���� �� ��ġ�� ���� �̵�
            if(_targetTransform != null)
            {
                transform.position = Vector3.Lerp(transform.position, _targetTransform.position, Time.deltaTime);
            }
        }
    }

    // ���� �ð� ������ ���̵� �ƿ� ����
    protected override void LifeTimer()
    {
        if (gameObject.activeSelf)
        {
            _timer += Time.deltaTime;

            if (_timer >= _weaponLifeTimer)
            {
                _timer -= _weaponLifeTimer;
                StartCoroutine(FadeSword());
            }
        }
    }

    private IEnumerator FadeSword()
    {
        if (swordRenderer != null)
        {
            // ���̵� �ƿ�
            float elapsed = 0f;
            while (elapsed < _fadeLerpTime)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / _fadeLerpTime;

                // ���� �� ����
                Color color = swordMaterial.color;
                color.a = Mathf.Lerp(color.a, _minAlphaValue, t);
                swordMaterial.color = color;

                yield return null;
            }

            // ���� ���İ� ����
            Color finalColor = swordMaterial.color;
            finalColor.a = _minAlphaValue;
            swordMaterial.color = finalColor;

            gameObject.SetActive(false);
        }
    }

    public void Fire(Vector3 pos, Vector3 dir, WeaponData data)
    {
        gameObject.SetActive(true);

        pos += _spawnPosYOffset;
        transform.position = pos;
        _direction = dir;
        _weaponRange = data.AttackRange;
        _weaponSpeed = data.AttackSpeed;
        _weaponRotSpeed = data.RotSpeed;
        _weaponLifeTimer = data.LifeTime;
        _weaponAttackPower = data.AttackPower;
        _direction.y = 0.0f;
    }

    private void StartSwordGrow()
    {
        StartCoroutine(GrowSword());
    }

    private IEnumerator GrowSword()
    {
        float timer = 0.0f;
        float growDuration = _weaponLifeTimer * 0.5f; // ���� Ŀ���� �� �ɸ��� �ð�

        Vector3 startScale = transform.localScale;

        while (timer < growDuration)
        {
            timer += Time.deltaTime;
            float t = timer / growDuration; // 0 ~ 1�� ����
            transform.localScale = Vector3.Lerp(startScale, Vector3.one * _weaponRange, t);
            yield return null;
        }

        transform.localScale = Vector3.one * _weaponRange;
    }

    private void OnTriggerEnter(Collider other)
    {
        // �ڷ�ƾ�� �ѹ��� �����ϱ� ���ؼ�
        if (other.CompareTag("Monster") || other.CompareTag("Boss"))
        {
            Collider[] _targetColliders = Physics.OverlapSphere(transform.position, transform.localScale.x, LayerMask.GetMask("Monster"));
            _overlapCount = _targetColliders.Length;

            foreach (Collider targetCollider in _targetColliders)
            {
                targetCollider.gameObject.GetComponent<Monster>().MonsterGetDamage(_weaponAttackPower);
                SoundManager.Instance.PlayFX(SoundKey.NormalWeaponHitSound, 0.04f / _overlapCount);
                DamageTextManager.Instance.ShowDamageText(targetCollider.transform, _weaponAttackPower, _color);
            }

            if (!_isCollision)
            {
                // �浹 �ϸ� LifeTime �ʱ�ȭ �ϰ� Ÿ�� ��ġ�� LifeTime ���� ����
                _timer = 0.0f;
                _isCollision = true;
                _targetTransform = other.transform;

                // �ڷ�ƾ�� �̿��� ���� ���� Ŀ��
                StartSwordGrow();
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // �浹 ���̶�� ���� �ð����� ������ �ֱ�
        if (other.CompareTag("Monster") || other.CompareTag("Boss"))
        {
            Monster target = other.gameObject.GetComponent<Monster>();

            _triggerStayTimer += Time.deltaTime;

            if (_triggerStayTimer >= _triggerStayAttackInterval)
            {
                _triggerStayTimer -= _triggerStayAttackInterval;
                target.MonsterGetDamage(_weaponAttackPower);
                SoundManager.Instance.PlayFX(SoundKey.NormalWeaponHitSound, 0.04f / _overlapCount);
                DamageTextManager.Instance.ShowDamageText(target.transform, _weaponAttackPower, _color);
            }
        }
    }
}