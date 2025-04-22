using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Monster : MonoBehaviour
{
    protected Transform _player;
    protected GameObject _ground;
    protected GameObject _monsterHpBarpanel;
    protected GameObject _monsterHpBarPrefab;
    protected Slider _monsterHpBarSlider;
    protected Animator _monsterAnimator;
    protected Collider _monsterCollider;

    private GameObject _inGameTimer;
    private Transform _groundParent;
    private Image[] _monsterHpBarImages;

    protected Vector3 _moveOffset;
    protected MonsterData _monsterData;
    protected AnimatorStateInfo _monsterAnimStateInfo;
    
    private Vector3 _monsterHpBarOffset;

    protected float _maxHp;
    protected float _curHp;
    protected float _speed;
    protected float _rotSpeed;
    protected float _attackPower;
    protected float _attackInterval;
    protected float _attackDistance;
    protected float _lifeTime;
    protected float _spawnInterval = float.MinValue;
    protected float _spawnStartTime = float.MinValue;
    protected float _spawnEndTime = float.MinValue;
    protected float _stateScaleFactor;

    protected float _attackTimer = 0.0f;
    protected float _distanceThreshold;

    private readonly float _halfRatio = 0.5f;
    private readonly float _fadeSpeed = 3.0f;
    private readonly float _hpBarVisibleDuration = 2.0f;
    private readonly float _minAlphaValue = 0.05f;

    private float _inGameTime;
    private float _baseHp;
    private float _baseAtk;
    private float _hpBarAlphaValue = 1.0f;
    private float _hpBarVisibleTimer = 0.0f;

    protected int _exp;
    protected int _type;

    private readonly int _one = 1;

    protected bool _isAttackAble = false;
    protected bool _isHit = false;

    private bool _isFadeOut = false;
    private bool _isHpBarVisible = false;
    private bool _isStatSettingEnd = false;

    protected void Awake()
    {
        _monsterHpBarPrefab = Resources.Load<GameObject>("Prefabs/MonsterUI/MonsterHpBar");
        _monsterHpBarOffset = new Vector3(0.0f, 1.5f, 0.0f);
    }

    protected void Start()
    {
        _player = GameManager.Instance.Player.transform;

        _monsterCollider = GetComponent<Collider>();
        _monsterAnimator = GetComponent<Animator>();

        // ü�¹� ����
        GameObject _monsterHpBarPanel = GameObject.Find("HpBarPanel");
        GameObject _monsterHpBar = Instantiate(_monsterHpBarPrefab, _monsterHpBarPanel.transform);
        _monsterHpBarSlider = _monsterHpBar.GetComponent<Slider>();
        _monsterHpBarSlider.gameObject.SetActive(false);

        // ü�¹� �̹����� �޾ƿ���
        _monsterHpBarImages = _monsterHpBar.GetComponentsInChildren<Image>();
        SetMonsterHpBarAlpha(_one);

        // �ΰ��� �ð��� �������� ���� InGamePlayTimer ������Ʈ�� ã��
        _inGameTimer = GameObject.Find("InGamePlayTimer");

        // �� �Ѱ��� ã���� �Ǳ� ������ �� ������Ʈ�� GroundObjects�� ã�� (�θ�)
        _groundParent = GameObject.Find("GroundObjects").transform;
        _ground = _groundParent.GetChild(0).gameObject;
        // ���� ����� ������
        _moveOffset = _ground.GetComponent<Ground>().GroundSize;
        // �÷��̾�� ���� ���� �Ÿ��� ���ذ��� ����
        _distanceThreshold = _moveOffset.x * _halfRatio;
    }

    protected void OnEnable()
    {
        // ������ ������ �Ϸ�Ǿ��� ���, HP�� ATK ����
        if (_isStatSettingEnd)
        {
            // �ΰ��� �ð� �޾ƿ���
            _inGameTime = _inGameTimer.GetComponent<InGameTime>().InGameTimer;
            // HP = ���� HP �� (1 + (���� ��� �ð� / ������ ����))
            // ATKPW = ���� ���ݷ� �� (1 + (���� ��� �ð� / ������ ����))
            _maxHp = _baseHp * (_one + (_inGameTime / _stateScaleFactor));
            _curHp = _maxHp;
            _attackPower = _baseAtk * (_one + (_inGameTime / _stateScaleFactor));
        }

        // �ʱ�ȭ
        _curHp = _maxHp;
        _hpBarVisibleTimer = 0.0f;

        _isFadeOut = false;
        _isHpBarVisible = false;
        _hpBarAlphaValue = _one;

        // null �ƴϸ� �浹ü Ŵ
        if (_monsterCollider != null)
        {
            _monsterCollider.enabled = true;
        }
        // null �ƴϸ� ü�¹� ��Ȱ��ȭ
        if(_monsterHpBarSlider != null)
        {
            SetMonsterHpBarAlpha(_one);
            _monsterHpBarSlider.gameObject.SetActive(false);
        }
    }

    protected void Update()
    {
        // ü�¹� ��ġ�� �׻� ����
        ShowHpBar();
        // ü�¹� ���̵� �ƿ� ����
        FadeOutMonsterHpBar();
        // �÷��̾�� �Ÿ��� �ʹ� �ָ� �ݴ������� ������
        Reposition();
    }

    // ���� ������ ����
    protected void SetMonsterData(MonsterData monsterData)
    {
        _maxHp = monsterData.Hp;
        _baseHp = _maxHp;
        _curHp = _maxHp;
        _exp = monsterData.Exp;
        _type = monsterData.Type;
        _speed = monsterData.MoveSpeed; 
        _rotSpeed = monsterData.RotateSpeed;

        _attackPower = monsterData.AttackPower;
        _baseAtk = _attackPower;
        _attackInterval = monsterData.AttackInterval;
        _attackDistance = monsterData.AttackDistance;

        _lifeTime = monsterData.LifeTime;

        _stateScaleFactor = monsterData.StatScaleFactor;

        _isStatSettingEnd = true;
    }

    protected void Reposition()
    {
        Vector3 playerPos = _player.transform.position;
        Vector3 skeletonPos = transform.position;
        // �÷��̾���� �Ÿ�
        float distance = Vector3.Distance(playerPos, skeletonPos);
        // �÷��̾�� ���ϴ� ����
        Vector3 dir = (_player.transform.position - transform.position).normalized;
        dir.y = 0.0f;

        // �Ÿ��� ����ġ �̻� ���̳���
        if (distance >= _distanceThreshold)
        {
            // �ݴ� �������� �̵�
            transform.position += Vector3.Scale(dir, _moveOffset);
        }
    }
    
    // �ݶ��̴��� ������ ���� �Ұ���
    protected void StopAttack()
    {
        if (!_monsterCollider.enabled)
        {
            _isAttackAble = false;
            _attackTimer = 0.0f;
        }
    }

    // ü�� ���� �����ؼ� �����ֱ�
    protected void ShowHpBar()
    {
        Vector3 worldPos = transform.position + _monsterHpBarOffset;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        if (screenPos.z > 0)
        {
            _monsterHpBarSlider.transform.position = screenPos;
            _monsterHpBarSlider.value = _curHp / _maxHp;
        }
    }

    protected void Move()
    {
        // �÷��̾� �������� �̵�, ȸ��
        Vector3 direction = (_player.position - transform.position).normalized;
        direction.y = 0;

        if (direction.sqrMagnitude > 0)
        {
            transform.Translate(direction * _speed * Time.deltaTime, Space.World);

            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * _rotSpeed);
        }
    }

    private bool HasAnimParameter(Animator animator, string paramName)
    {
        // �Ķ���Ϳ� �Ű������� ���� paramName�� �ִ��� Ȯ��
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == paramName)
                return true;
        }
        return false;
    }

    protected virtual void MonsterGetDamage(float damage)
    {
        // �ǰ�
        _curHp -= damage;

        // ������ �״� �ִϸ��̼�, �浹ü ���� ü�¹� ����
        if (_curHp <= 0)
        {
            _curHp = 0;
            _monsterCollider.enabled = false;
            GameObject exp = PoolingManager.Instance.Pop("Exp");
            exp.GetComponent<Exp>().SetExp(_exp, transform.position);
            _monsterAnimator.SetTrigger("Dead");
        }
        else
        {
            // Hit�̶�� �Ķ���Ͱ� �ִϸ��̼ǿ� �ִ��� üũ
            if (HasAnimParameter(_monsterAnimator, "Hit"))
            {
                // �´� �ִϸ��̼�
                _monsterAnimator.SetTrigger("Hit");
            }
        }
    }

    // ü�¹� ���İ� ����
    private void SetMonsterHpBarAlpha(float alphaValue)
    {
        foreach (Image monsterHpBarImage in _monsterHpBarImages)
        {
            Color monsterHpBarColor = monsterHpBarImage.color;
            monsterHpBarColor.a = alphaValue;
            monsterHpBarImage.color = monsterHpBarColor;
        }
    }

    private void FadeOutMonsterHpBar()
    {
        if (_isHpBarVisible)
        {
            _hpBarVisibleTimer += Time.deltaTime;

            if (_hpBarVisibleTimer >= _hpBarVisibleDuration && !_isFadeOut)
            {
                _isFadeOut = true;
            }
        }

        // ü�¹ٰ� ���̰� fade �ƿ��� �����ߴٸ� ���� ���� ������ ����
        if (_isFadeOut && _hpBarAlphaValue > _minAlphaValue)
        {
            _hpBarAlphaValue -= Time.deltaTime * _fadeSpeed;
            SetMonsterHpBarAlpha(_hpBarAlphaValue);

            if (_hpBarAlphaValue <= _minAlphaValue)
            {
                _monsterHpBarSlider.gameObject.SetActive(false);
                _isHpBarVisible = false;
            }
        }
    }

    // ���� ü�¹� ���� (�ִϸ��̼� Ű)
    public void OnMonsterHpBarOff()
    {
        _monsterHpBarSlider.gameObject.SetActive(false);
    }

    // ���� ���� (�ִϸ��̼� Ű)
    public void OnInActive()
    {
        gameObject.SetActive(false);
    }

    protected void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Weapon"))
        {
            // ü�¹� Ÿ�̸� �ʱ�ȭ �� ǥ�� ����
            _isFadeOut = false;
            _isHpBarVisible = true;
            _hpBarVisibleTimer = 0.0f;
            _hpBarAlphaValue = _one;

            // ���� ü�¹� ���İ� 1�� �ϰ�
            SetMonsterHpBarAlpha(_one);
            // ���� ü�¹� �����ֱ�
            _monsterHpBarSlider.gameObject.SetActive(true);
            // ���� ���ݷ� ��ŭ �������ֱ�
            float damage = other.GetComponent<Weapon>().WeaponAttackPower;
            MonsterGetDamage(damage);
        }
    }
}