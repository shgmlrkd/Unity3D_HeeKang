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

        // 알파값 초기화
        if(swordMaterial != null)
        {
            Color finalColor = swordMaterial.color;
            finalColor.a = _maxAlphaValue;
            swordMaterial.color = finalColor;
        }
    }

    private void Start()
    {
        // 빈 게임 오브젝트 안에 자식으로 모델이 있음
        swordRenderer = transform.GetChild(0).GetComponent<Renderer>();
        swordMaterial = swordRenderer.material;
    }

    private void Update()
    {
        base.Update();

        transform.Rotate(Vector3.up * _weaponRotSpeed * Time.deltaTime);

        // 처음 잡은 타겟이 죽으면 타겟 해제
        if (_targetTransform != null && _targetTransform.gameObject.GetComponent<Monster>().MonsterCurHp <= 0)
        {
            _targetTransform = null;
        }
    }

    protected override void Move()
    {
        // 아직 충돌하지 않았다면 _direction 쪽으로 계속 이동
        if(!_isCollision)
        {
            transform.position += _direction * _weaponSpeed * Time.deltaTime;
        }
        else
        {
            // 충돌 후 타겟 Transform을 찾으면 그 위치로 보간 이동
            if(_targetTransform != null)
            {
                transform.position = Vector3.Lerp(transform.position, _targetTransform.position, Time.deltaTime);
            }
        }
    }

    // 일정 시간 지나면 페이드 아웃 시작
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
            // 페이드 아웃
            float elapsed = 0f;
            while (elapsed < _fadeLerpTime)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / _fadeLerpTime;

                // 알파 값 조절
                Color color = swordMaterial.color;
                color.a = Mathf.Lerp(color.a, _minAlphaValue, t);
                swordMaterial.color = color;

                yield return null;
            }

            // 최종 알파값 설정
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
        float growDuration = _weaponLifeTimer * 0.5f; // 검이 커지는 데 걸리는 시간

        Vector3 startScale = transform.localScale;

        while (timer < growDuration)
        {
            timer += Time.deltaTime;
            float t = timer / growDuration; // 0 ~ 1로 변함
            transform.localScale = Vector3.Lerp(startScale, Vector3.one * _weaponRange, t);
            yield return null;
        }

        transform.localScale = Vector3.one * _weaponRange;
    }

    private void OnTriggerEnter(Collider other)
    {
        // 코루틴을 한번만 실행하기 위해서
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
                // 충돌 하면 LifeTime 초기화 하고 타겟 위치로 LifeTime 동안 추적
                _timer = 0.0f;
                _isCollision = true;
                _targetTransform = other.transform;

                // 코루틴을 이용해 검이 점점 커짐
                StartSwordGrow();
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // 충돌 중이라면 일정 시간마다 데미지 주기
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