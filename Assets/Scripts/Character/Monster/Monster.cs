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

        // 체력바 생성
        GameObject _monsterHpBarPanel = GameObject.Find("HpBarPanel");
        GameObject _monsterHpBar = Instantiate(_monsterHpBarPrefab, _monsterHpBarPanel.transform);
        _monsterHpBarSlider = _monsterHpBar.GetComponent<Slider>();
        _monsterHpBarSlider.gameObject.SetActive(false);

        // 체력바 이미지들 받아오기
        _monsterHpBarImages = _monsterHpBar.GetComponentsInChildren<Image>();
        SetMonsterHpBarAlpha(_one);

        // 인게임 시간을 가져오기 위해 InGamePlayTimer 오브젝트를 찾음
        _inGameTimer = GameObject.Find("InGamePlayTimer");

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
        // 몬스터의 세팅이 완료되었을 경우, HP와 ATK 갱신
        if (_isStatSettingEnd)
        {
            // 인게임 시간 받아오기
            _inGameTime = _inGameTimer.GetComponent<InGameTime>().InGameTimer;
            // HP = 현재 HP × (1 + (게임 경과 시간 / 스케일 배율))
            // ATKPW = 현재 공격력 × (1 + (게임 경과 시간 / 스케일 배율))
            _maxHp = _baseHp * (_one + (_inGameTime / _stateScaleFactor));
            _curHp = _maxHp;
            _attackPower = _baseAtk * (_one + (_inGameTime / _stateScaleFactor));
        }

        // 초기화
        _curHp = _maxHp;
        _hpBarVisibleTimer = 0.0f;

        _isFadeOut = false;
        _isHpBarVisible = false;
        _hpBarAlphaValue = _one;

        // null 아니면 충돌체 킴
        if (_monsterCollider != null)
        {
            _monsterCollider.enabled = true;
        }
        // null 아니면 체력바 비활성화
        if(_monsterHpBarSlider != null)
        {
            SetMonsterHpBarAlpha(_one);
            _monsterHpBarSlider.gameObject.SetActive(false);
        }
    }

    protected void Update()
    {
        // 체력바 위치는 항상 갱신
        ShowHpBar();
        // 체력바 페이드 아웃 연출
        FadeOutMonsterHpBar();
        // 플레이어와 거리가 너무 멀면 반대편으로 보내기
        Reposition();
    }

    // 몬스터 데이터 세팅
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
        // 플레이어와의 거리
        float distance = Vector3.Distance(playerPos, skeletonPos);
        // 플레이어로 향하는 방향
        Vector3 dir = (_player.transform.position - transform.position).normalized;
        dir.y = 0.0f;

        // 거리가 기준치 이상 차이나면
        if (distance >= _distanceThreshold)
        {
            // 반대 방향으로 이동
            transform.position += Vector3.Scale(dir, _moveOffset);
        }
    }
    
    // 콜라이더가 꺼지면 공격 불가능
    protected void StopAttack()
    {
        if (!_monsterCollider.enabled)
        {
            _isAttackAble = false;
            _attackTimer = 0.0f;
        }
    }

    // 체력 상태 갱신해서 보여주기
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
        // 플레이어 방향으로 이동, 회전
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
        // 파라미터에 매개변수로 들어온 paramName이 있는지 확인
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == paramName)
                return true;
        }
        return false;
    }

    protected virtual void MonsterGetDamage(float damage)
    {
        // 피격
        _curHp -= damage;

        // 죽으면 죽는 애니메이션, 충돌체 끄고 체력바 끄기
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
            // Hit이라는 파라미터가 애니메이션에 있는지 체크
            if (HasAnimParameter(_monsterAnimator, "Hit"))
            {
                // 맞는 애니메이션
                _monsterAnimator.SetTrigger("Hit");
            }
        }
    }

    // 체력바 알파값 조절
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

        // 체력바가 보이고 fade 아웃을 시작했다면 알파 값을 서서히 줄임
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

    // 몬스터 체력바 끄기 (애니메이션 키)
    public void OnMonsterHpBarOff()
    {
        _monsterHpBarSlider.gameObject.SetActive(false);
    }

    // 몬스터 끄기 (애니메이션 키)
    public void OnInActive()
    {
        gameObject.SetActive(false);
    }

    protected void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Weapon"))
        {
            // 체력바 타이머 초기화 및 표시 설정
            _isFadeOut = false;
            _isHpBarVisible = true;
            _hpBarVisibleTimer = 0.0f;
            _hpBarAlphaValue = _one;

            // 몬스터 체력바 알파값 1로 하고
            SetMonsterHpBarAlpha(_one);
            // 몬스터 체력바 보여주기
            _monsterHpBarSlider.gameObject.SetActive(true);
            // 무기 공격력 만큼 데미지주기
            float damage = other.GetComponent<Weapon>().WeaponAttackPower;
            MonsterGetDamage(damage);
        }
    }
}