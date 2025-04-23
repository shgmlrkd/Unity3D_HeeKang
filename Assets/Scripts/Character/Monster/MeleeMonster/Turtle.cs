using System.Linq;
using UnityEngine;

public class Turtle : MeleeMonster
{
    private Renderer[] _turleMaterial; 
    private ParticleSystem[] _shieldParticles;

    private Vector3 _playerPosAtHit; // �¾��� �� �÷��̾� ��ġ ����
    private Vector3 _directionToPlayer; // �÷��̾�� ���ϴ� ���⺤�� ����
    private Vector3 _dashTargetPos; // ���� ��ǥ ��ġ

    private Color _startColor = Color.white; // ���
    private Color _targetColor = Color.red; // ������

    private readonly float _rageEndDistance = 1.0f;
    private readonly float _rageSpeedRate = 5.0f;

    private float _targetDistance;
    private float _particalLifeTime;
    private float _colorLerpTime = 1.0f;
    private float _colorLerpTimer = 0.0f;
    private float _distanceOffset = 4.0f;

    private bool _isRagePrepared = false; // �г��ߴ��� üũ�ϴ� ����
    private bool _getDamaged = false; // ������ �Ծ����� üũ�ϴ� ����
    private bool _isNoDamage = false; // �������� üũ�ϴ� ����
    private bool _isRedColor = false; // ���������� üũ�ϴ� ����
    private bool _isReached = false; // �÷��̾ �־��� ��ġ���� �����ߴ��� üũ�ϴ� ����

    private int _turtleKey = 103;

    private void OnEnable()
    {
        base.OnEnable();

        _colorLerpTimer = 0.0f;

        // bool ���� �ʱ�ȭ
        _isRagePrepared = false;
        _getDamaged = false;
        _isNoDamage = false;
        _isRedColor = false;
        _isReached = false;

        // Ȥ�� �𸣴� ���� ���� ����
        if (_turleMaterial != null)
        {
            foreach (Renderer turleMaterial in _turleMaterial)
            {
                MaterialPropertyBlock block = new MaterialPropertyBlock();
                turleMaterial.GetPropertyBlock(block);
                block.SetColor("_BaseColor", _startColor);
                turleMaterial.SetPropertyBlock(block);
            }
        }
    }

    private void Start()
    {
        base.Start();

        _turleMaterial = GetComponentsInChildren<Renderer>();

        _shieldParticles = GetComponentsInChildren<ParticleSystem>();

        // Ű���� ���� ���� ������ ����
        SetMonsterData(MonsterDataManager.Instance.GetMonsterData(_turtleKey));
    }

    private void Update()
    {
        base.Update();

        // �ٽ� ���� ���������� �ٲ�
        ResetOriginalColor();

        if (_curHp <= 0) return;

        StopAttack();

        // ������ ������ �� ���������� �ٲ�
        DamageColorEffect();
        // ������ ���� �� �Լ��� ������
        ImmortalMove();

        Attack();

        if (CanMove() && !_isRedColor)
        {
            Move();
        }
    }

    private void ImmortalMove()
    {
        // �������̰� �÷��̾ �־��� ��ġ��
        // �������� ������ ��� �� �������� ������ ������
        if (_isRedColor && !_isReached)
        {
            transform.Translate(_directionToPlayer * _speed * _rageSpeedRate * Time.deltaTime, Space.World);

            // ���� �Ÿ� �Ǹ� ���� ����, �г� ����
            if (Vector3.Distance(transform.position, _dashTargetPos) <= _rageEndDistance)
            {
                _isNoDamage = false;
                _isRedColor = false;
                _isRagePrepared = false;
                _isReached = true;
                print("���� ��, ����");
            }
        }
    }

    private void ResetOriginalColor()
    {
        // �÷��̾ �־��� ��ġ�� �����ߴٸ�
        if (_isReached)
        {
            _colorLerpTimer += Time.deltaTime;
            // Ŭ����01�� 0���� 1���̷� ���� ������
            float colorLerpTimer = Mathf.Clamp01(_colorLerpTimer / _colorLerpTime);
            // �ź��� ���� �� ���������� ����
            foreach (Renderer turleMaterial in _turleMaterial)
            {
                MaterialPropertyBlock block = new MaterialPropertyBlock();
                turleMaterial.GetPropertyBlock(block);
                Color currentColor = Color.Lerp(_targetColor, _startColor, colorLerpTimer);
                block.SetColor("_BaseColor", currentColor);
                turleMaterial.SetPropertyBlock(block);
            }

            // ���������� �Ǹ� �ʱ�ȭ
            if (colorLerpTimer >= _colorLerpTime)
            {
                _isRedColor = false;
                _isReached = false;
                _colorLerpTimer = 0.0f;
                print("isreached ����, ���� �������� ���ƿ�");
            }
        }
    }

    private void DamageColorEffect()
    {
        // �������� �Ծ����� �� �ڸ����� ���������� material �� ����
        if (_getDamaged)
        {
            _colorLerpTimer += Time.deltaTime;
            // Ŭ����01�� 0���� 1���̷� ���� ������
            float colorLerpTimer = Mathf.Clamp01(_colorLerpTimer / _colorLerpTime);
            // �ź��� �ǰ� ����� ���� ���������� ����
            foreach (Renderer turleMaterial in _turleMaterial)
            {
                MaterialPropertyBlock block = new MaterialPropertyBlock();
                turleMaterial.GetPropertyBlock(block);
                Color currentColor = Color.Lerp(_startColor, _targetColor, colorLerpTimer);
                block.SetColor("_BaseColor", currentColor);
                turleMaterial.SetPropertyBlock(block);
            }

            // �г� ���� �غ� �ȵǾ����� �� ���� ����
            if (!_isRagePrepared)
            {
                PrepareRageState();
                _isRagePrepared = true;  // PrepareRageState() ���� �Ŀ��� �ٽ� ������� �ʵ��� ����
            }

            // ���������� �Ǹ� �ʱ�ȭ
            if (colorLerpTimer >= _colorLerpTime)
            {
                _isRedColor = true;
                _getDamaged = false;
                _colorLerpTimer = 0.0f;
            }
        }
    }

    private void PrepareRageState()
    {
        _playerPosAtHit = _player.position;
        // �÷��̾ �־��� ��ġ�� ���� ���� ����
        _directionToPlayer = (_playerPosAtHit - transform.position).normalized;
        _directionToPlayer.y = 0.0f;
        // ���� �� ��ǥ ��ġ ����
        _dashTargetPos = _playerPosAtHit + _directionToPlayer * _distanceOffset;
        // �÷��̾ �־��� ��ġ ���� �Ÿ� ���
        _targetDistance = Vector3.Distance(transform.position, _player.position);
        // ��ƼŬ ������ Ÿ�� ���� (_colorLerpTime ���� �� �ڸ��� ������ �ֱ� ������ �� �ð��� ����)
        _particalLifeTime = _targetDistance / (_speed * _rageSpeedRate) + _colorLerpTime;

        // ü���� ���� �ִٸ�
        if (_curHp > 0)
        {
            // ���� ��ƼŬ �÷���
            foreach (ParticleSystem shieldparticle in _shieldParticles)
            {
                // ��ƼŬ ���� �ð��� ���� �� �÷���
                ParticleSystem.MainModule particleLifeTime = shieldparticle.main;
                particleLifeTime.startLifetime = new ParticleSystem.MinMaxCurve(_particalLifeTime);
                shieldparticle.Play();
            }
        }
    }

    // ���� �ִϸ��̼� ���·� ������ �� �ִ��� Ȯ��
    private bool CanMove()
    {
        // �ִϸ��̼ǿ� Base Layer�� �����°Ű� Base Layer�� �ε����� 0 �̾ �Ű������� 0��
        _monsterAnimStateInfo = _monsterAnimator.GetCurrentAnimatorStateInfo(0);
        bool isInDead = _monsterAnimStateInfo.IsName("Dead");
        bool isInHit = _monsterAnimStateInfo.IsName("Hit");

        // Hit�̳� Dead ���°� �ƴ϶�� true ��ȯ
        return !(isInHit || isInDead);
    }

    protected override void MonsterGetDamage(float damage)
    {
        if (!_isNoDamage)
        {
            base.MonsterGetDamage(damage);
            // ���� ���� On
            _isNoDamage = true;
        }

        // material�� �������� �ƴϸ� true
        if (!_isRedColor)
        {
            _getDamaged = true;
        }
    }
}