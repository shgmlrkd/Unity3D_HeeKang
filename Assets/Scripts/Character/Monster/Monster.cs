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

    private Transform _groundParent;
    private Image[] _monsterHpBarImages;

    protected Vector3 _moveOffset;
    protected Vector3 _monsterHpBarOffset;
    protected MonsterData _monsterData;

    protected float _maxHp;
    protected float _curHp;
    protected float _speed;
    protected float _rotSpeed;
    protected float _attackPower;
    protected float _attackInterval;
    protected float _attackDistance;
    protected float _lifeTime;
    protected float _spawnInterval;
    protected float _spawnStartTime;
    protected float _spawnEndTime;
    protected float _stateScaleFactor;

    protected float _attackTimer = 0.0f;
    protected float _distanceThreshold;

    private readonly float _halfRatio = 0.5f;
    private readonly float _fadeSpeed = 3.0f;
    private readonly float _hpBarVisibleDuration = 2.0f;
    private readonly float _minAlphaValue = 0.05f;

    private float _hpBarVisibleTimer = 0.0f;

    protected int _exp;
    protected int _type;

    protected bool _isAttackAble = false;

    private bool _isHpBarVisible = false;
    private bool _isFadeOut = false;

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

        _monsterHpBarImages = _monsterHpBar.GetComponentsInChildren<Image>();
        SetMonsterHpBarAlphaToOpaque();

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
        // ü�� �ʱ�ȭ
        _curHp = _maxHp;
        
        // null �ƴϸ� �浹ü Ŵ
        if(_monsterCollider != null)
        {
            _monsterCollider.enabled = true;
        }
        // null �ƴϸ� ü�¹� ��Ȱ��ȭ
        if(_monsterHpBarSlider != null)
        {
            SetMonsterHpBarAlphaToOpaque();
            _monsterHpBarSlider.gameObject.SetActive(false);
        }
    }

    protected void Update()
    {
        if (_isHpBarVisible)
        {
            _hpBarVisibleTimer += Time.deltaTime;

            if (_hpBarVisibleTimer >= _hpBarVisibleDuration && !_isFadeOut)
            {
                _hpBarVisibleTimer -= _hpBarVisibleDuration;
                _isFadeOut = true;
                StartCoroutine(FadeOutHpBar());
            }
        }
    }

    // ���� ������ ����
    protected void SetMonsterData(MonsterData monsterData)
    {
        _maxHp = monsterData.Hp;
        _curHp = _maxHp;
        _exp = monsterData.Exp;
        _type = monsterData.Type;
        _speed = monsterData.MoveSpeed; 
        _rotSpeed = monsterData.RotateSpeed;

        _attackPower = monsterData.AttackPower;
        _attackInterval = monsterData.AttackInterval;
        _attackDistance = monsterData.AttackDistance;

        _lifeTime = monsterData.LifeTime;

        _spawnInterval = monsterData.SpawnInterval;
        _spawnStartTime = monsterData.SpawnStartTime;
        _spawnEndTime = monsterData.SpawnEndTime;

        _stateScaleFactor = monsterData.StatScaleFactor;
    }

    private void GetDamage(float damage)
    {
        // �ǰ�
        _curHp -= damage;

        // ������ �״� �ִϸ��̼�, �浹ü ���� ü�¹� ����
        if (_curHp <= 0)
        {
            _curHp = 0;
            _monsterCollider.enabled = false;
            _monsterAnimator.SetTrigger("Dead");
        }
        else
        {   
            // �´� �ִϸ��̼�
            _monsterAnimator.SetTrigger("Hit");
        }
    }

    private void SetMonsterHpBarAlphaToOpaque()
    {
        // ü�¹� ���İ� 1�� �����
        foreach(Image monsterHpBarImage in _monsterHpBarImages)
        {
            Color monsterHpBarColor = monsterHpBarImage.color;
            monsterHpBarColor.a = 1.0f;
            monsterHpBarImage.color = monsterHpBarColor;
        }
    }

    private float SetMonsterHpBarAlphaFadeOut(float fadeSpeed)
    {
        float alphaValue = 0.0f;
        foreach (Image monsterHpBarImage in _monsterHpBarImages)
        {
            Color monsterHpBarColor = monsterHpBarImage.color;
            monsterHpBarColor.a = Mathf.Lerp(monsterHpBarColor.a, 0, Time.deltaTime * fadeSpeed);
            monsterHpBarImage.color = monsterHpBarColor;
            alphaValue = monsterHpBarColor.a;
        }

        return alphaValue;
    }

    public void MonsterHpBarOff()
    {
        _monsterHpBarSlider.gameObject.SetActive(false);
    }

    public void ActiveOff()
    {
        gameObject.SetActive(false);
    }

    private IEnumerator FadeOutHpBar()
    {
        float alphaValue = float.MaxValue;

        // ���İ��� ���� �� ������
        while (alphaValue > _minAlphaValue)
        {
            alphaValue = SetMonsterHpBarAlphaFadeOut(_fadeSpeed);
            yield return null;
        }
        
        // ü�¹� ��Ȱ��ȭ
        _monsterHpBarSlider.gameObject.SetActive(false);
        _isFadeOut = false;
        _isHpBarVisible = false;
    }

    protected void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Weapon"))
        {
            // ü�¹� ���̰���
            _isHpBarVisible = true;

            // ���� ü�¹� ���İ� 1�� �ϰ�
            SetMonsterHpBarAlphaToOpaque();
            // ���� ü�¹� �����ֱ�
            _monsterHpBarSlider.gameObject.SetActive(true);
            // ���� ���ݷ� ��ŭ �������ֱ�
            float damage = other.GetComponent<Weapon>().WeaponAttackPower;
            GetDamage(damage);
        }
    }
}