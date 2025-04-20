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

        // 체력바 생성
        GameObject _monsterHpBarPanel = GameObject.Find("HpBarPanel");
        GameObject _monsterHpBar = Instantiate(_monsterHpBarPrefab, _monsterHpBarPanel.transform);
        _monsterHpBarSlider = _monsterHpBar.GetComponent<Slider>();
        _monsterHpBarSlider.gameObject.SetActive(false);

        _monsterHpBarImages = _monsterHpBar.GetComponentsInChildren<Image>();
        SetMonsterHpBarAlphaToOpaque();

        // 땅 한개만 찾으면 되기 때문에 빈 오브젝트인 GroundObjects를 찾음 (부모)
        _groundParent = GameObject.Find("GroundObjects").transform;
        _ground = _groundParent.GetChild(0).gameObject;
        // 땅의 사이즈를 가져옴
        _moveOffset = _ground.GetComponent<Ground>().GroundSize;
        // 플레이어와 몬스터 사이 거리의 기준값을 설정
        _distanceThreshold = _moveOffset.x * _halfRatio;
    }

    protected void OnEnable()
    {
        // 체력 초기화
        _curHp = _maxHp;
        
        // null 아니면 충돌체 킴
        if(_monsterCollider != null)
        {
            _monsterCollider.enabled = true;
        }
        // null 아니면 체력바 비활성화
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

    // 몬스터 데이터 세팅
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
        // 피격
        _curHp -= damage;

        // 죽으면 죽는 애니메이션, 충돌체 끄고 체력바 끄기
        if (_curHp <= 0)
        {
            _curHp = 0;
            _monsterCollider.enabled = false;
            _monsterAnimator.SetTrigger("Dead");
        }
        else
        {   
            // 맞는 애니메이션
            _monsterAnimator.SetTrigger("Hit");
        }
    }

    private void SetMonsterHpBarAlphaToOpaque()
    {
        // 체력바 알파값 1로 만들기
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

        // 알파값이 거의 다 줄으면
        while (alphaValue > _minAlphaValue)
        {
            alphaValue = SetMonsterHpBarAlphaFadeOut(_fadeSpeed);
            yield return null;
        }
        
        // 체력바 비활성화
        _monsterHpBarSlider.gameObject.SetActive(false);
        _isFadeOut = false;
        _isHpBarVisible = false;
    }

    protected void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Weapon"))
        {
            // 체력바 보이게함
            _isHpBarVisible = true;

            // 몬스터 체력바 알파값 1로 하고
            SetMonsterHpBarAlphaToOpaque();
            // 몬스터 체력바 보여주기
            _monsterHpBarSlider.gameObject.SetActive(true);
            // 무기 공격력 만큼 데미지주기
            float damage = other.GetComponent<Weapon>().WeaponAttackPower;
            GetDamage(damage);
        }
    }
}