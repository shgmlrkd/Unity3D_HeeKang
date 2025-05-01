using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Monster : MonoBehaviour
{
    protected enum MonsterStatus
    {
        Run, Hit, Rush, Attack, Dead
    }

    protected Transform _player;
    protected GameObject _ground;
    protected GameObject _monsterHpBarpanel;
    protected GameObject _monsterHpBarPrefab;
    protected Slider _monsterHpBarSlider;
    protected Animator _monsterAnimator;
    protected Collider _monsterCollider;
    protected List<Material> _monsterMaterials;

    private Transform _groundParent;
    private Image[] _monsterHpBarImages;

    protected Status _monsterStatus;

    protected MonsterStatus _monsterCurrentState = MonsterStatus.Run;

    protected Vector3 _moveOffset;
    protected AnimatorStateInfo _monsterAnimStateInfo;

    private Vector3 _monsterHpBarOffset;

    protected float _maxHp;
    protected float _curHp;
    public float MonsterCurHp
    {
        get { return _curHp; }
    }
    protected float _attackPower;
    protected float _attackTimer = 0.0f;
    protected float _distanceThreshold;
    protected float _attackInterval = 0.0f;
    protected float _fadeLerpTimer = 10.0f;

    private readonly float _halfRatio = 0.5f;
    private readonly float _fadeSpeed = 3.0f;
    private readonly float _hpBarVisibleDuration = 2.0f;

    private readonly float _maxAlphaValue = 1.0f;
    private readonly float _minAlphaValue = 0.05f;

    private float _inGameTime;
    private float _baseHp;
    private float _baseAtk;
    private float _spawnStartTime;
    private float _hpBarAlphaValue = 1.0f;
    private float _hpBarVisibleTimer = 0.0f;

    private readonly int _one = 1;

    private int _prevUpgradeCount = 0;

    private bool _isSetting = false;
    private bool _isFadeOut = false;
    private bool _isHpBarVisible = false;
    private bool _isStatSettingEnd = false;

    protected void Awake()
    {
        _monsterHpBarPrefab = Resources.Load<GameObject>("Prefabs/UI/MonsterHpBar");
        _monsterHpBarOffset = new Vector3(0.0f, 1.5f, 0.0f);
    }

    protected void Start()
    {
        _player = InGameManager.Instance.Player.transform;

        _monsterMaterials = new List<Material>();

        _monsterCollider = GetComponent<Collider>();
        _monsterAnimator = GetComponent<Animator>();

        // 체력바 생성
        GameObject _monsterHpBarPanel = GameObject.Find("InGameCanvas/MonsterHpBarPanel");
        GameObject _monsterHpBar = Instantiate(_monsterHpBarPrefab, _monsterHpBarPanel.transform);
        _monsterHpBarSlider = _monsterHpBar.GetComponent<Slider>();
        _monsterHpBarSlider.gameObject.SetActive(false);

        _maxHp = _baseHp;
        _curHp = _maxHp;
        _attackPower = _baseAtk;

        // 체력바 이미지들 받아오기
        _monsterHpBarImages = _monsterHpBar.GetComponentsInChildren<Image>();
        SetMonsterHpBarAlpha(_one);

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
        // 몬스터의 세팅이 완료되었을 경우 
        if (_isStatSettingEnd)
        {
            // 인게임 시간 받아오고
            _inGameTime = InGameUIManager.Instance.GetInGameTimer();
            // 현재 강화 횟수 계산
            int upgradeCount = MonsterUpgradeManager.GetUpgradeCount(_inGameTime, _monsterStatus.SpawnStartTime, _monsterStatus.StatUpdateInterval);
            // 이전보다 강화 횟수가 증가했을 경우 능력치 갱신
            if (upgradeCount > _prevUpgradeCount)
            {
                float scale = MonsterUpgradeManager.GetScaleFactor(upgradeCount, _monsterStatus.StatUpdateInterval, _monsterStatus.StateScaleFactor);
                // HP 및 공격력 갱신
                _maxHp = _baseHp * scale;
                _curHp = _maxHp;
                _attackPower = _baseAtk * scale;
                // 현재 강화 횟수 저장
                _prevUpgradeCount = upgradeCount;
            }
        }

        // 초기화
        _curHp = _maxHp;

        _hpBarVisibleTimer = 0.0f;
        _monsterCurrentState = MonsterStatus.Run;

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
        // 알파값 되돌리기
        if (_monsterMaterials != null)
        {
            // 알파값 1로 돌려놓기
            foreach (Material monsterMaterial in _monsterMaterials)
            {
                Color finalColor = monsterMaterial.color;
                finalColor.a = _maxAlphaValue;
                monsterMaterial.color = finalColor;
            }
        }
    }

    protected void Update()
    {
        // 콜라이더 꺼지면 초기화
        StopAttackIfColliderOff();
        // 체력바 위치는 항상 갱신
        ShowHpBar();
        // 체력바 페이드 아웃 연출
        FadeOutMonsterHpBar();
        // 플레이어와 거리가 너무 멀면 반대편으로 보내기
        Reposition();
        // 몬스터 액션 FSM
        Action();
    }

    private void Action()
    {
        // 몬스터의 현재 상태에 따라 행동 처리
        switch (_monsterCurrentState)
        {
            // 몬스터가 Run 상태면 조건 확인 후 Move() 실행
            case MonsterStatus.Run:
                if (CanMove())
                { 
                    Move();
                }
                break;
            // 몬스터가 Hit 상태면 HandleHitState() 실행
            case MonsterStatus.Hit:
                HandleHitState();
                break;
            // 몬스터가 Rush 상태면 HandleRushState() 실행
            case MonsterStatus.Rush:
                HandleRushState();
                break;
            case MonsterStatus.Attack:
                HandleAttackState();
                break;
            case MonsterStatus.Dead:
                HandleDeadState();
                break;
        }
    }

    private void Reposition()
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
    private void StopAttackIfColliderOff()
    {
        if (!_monsterCollider.enabled)
        {
            _attackTimer = 0.0f;
        }
    }

    // 체력 상태 갱신해서 보여주기
    private void ShowHpBar()
    {
        Vector3 worldPos = transform.position + _monsterHpBarOffset;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        if (screenPos.z > 0)
        {
            _monsterHpBarSlider.transform.position = screenPos;
            _monsterHpBarSlider.value = _curHp / _maxHp;
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

    private void InitHpBarEffect()
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
    }

    // 체력바 알파값 조절
    public void SetMonsterHpBarAlpha(float alphaValue)
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

    protected virtual void Move()
    {
        _monsterCurrentState = MonsterStatus.Run;
        // 플레이어 방향으로 이동, 회전
        Vector3 direction = (_player.position - transform.position).normalized;
        direction.y = 0;

        if (direction.sqrMagnitude > 0)
        {
            transform.Translate(direction * _monsterStatus.Speed * Time.deltaTime, Space.World);

            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * _monsterStatus.RotSpeed);
        }
    }

    protected virtual bool CanMove()
    {
        _monsterAnimStateInfo = _monsterAnimator.GetCurrentAnimatorStateInfo(0);

        bool isInDead = _monsterAnimStateInfo.IsName("Dead");

        return !isInDead;
    }

    // 기본적으로 Run 상태로 돌아감
    protected virtual void HandleHitState()
    {
        _monsterCurrentState = MonsterStatus.Run;
    }
    protected virtual void HandleRushState()
    {
        // 기본은 아무것도 없음
    }

    protected virtual void HandleAttackState()
    {
        _monsterCurrentState = MonsterStatus.Run;
    }

    protected virtual void HandleDeadState()
    {
        // 기본은 아무것도 없음
    }

    // 몬스터 데이터 세팅
    protected void SetMonsterData(MonsterData monsterData, MonsterSpawnData monsterSpawnData)
    {
        _monsterStatus = new Status(monsterData, monsterSpawnData);
        _baseHp = _monsterStatus.MaxHp;
        _baseAtk = _monsterStatus.AttackPower;
        _spawnStartTime = _monsterStatus.SpawnStartTime;
        _isStatSettingEnd = true;
    }

    protected void SetMonsterKey(int key)
    {
        if (!_isSetting)
        {
            _isSetting = true;

            // 키값에 따른 데이터 세팅
            MonsterData data = MonsterDataManager.Instance.GetMonsterData(key);
            SetMonsterData(data, MonsterDataManager.Instance.GetMonsterSpawnData(data.Name));
        }
    }

    public virtual void MonsterGetDamage(float damage)
    {
        InitHpBarEffect();

        // 피격
        _curHp -= damage;

        // 죽으면 죽는 애니메이션, 충돌체 끄고 체력바 끄기
        if (_curHp <= 0)
        {
            // 몬스터 죽은 회수, 몬스터 콜라이더 끄기
            _curHp = 0;
            InGameUIManager.Instance.SetKillCountText();
            _monsterCollider.enabled = false;

            // 경험치와 랜덤 아이템 떨구기 (경험치 100%, 아이템 확률)
            ItemManager.Instance.SpawnExp(_monsterStatus.Exp, transform.position);
            ItemManager.Instance.SpawnRandomItem(transform.position);

            // 페이드 아웃하면서 죽는애니메이션
            StartCoroutine(FadeOutOnDeath());
            _monsterCurrentState = MonsterStatus.Dead;
        }
        else
        {
            // Hit이라는 파라미터가 애니메이션에 있는지 체크
            if (HasAnimParameter(_monsterAnimator, "Hit"))
            {
                // 맞는 애니메이션
                _monsterAnimator.SetTrigger("Hit");
                _monsterCurrentState = MonsterStatus.Hit;
            }
        }
    }

    public void MonsterKnockBack(Vector3 dir, float knockBackDistance, float duration)
    {
        // KnockBack 시작
        StartCoroutine(KnockBackRoutine(dir, knockBackDistance, duration));
    }

    private IEnumerator KnockBackRoutine(Vector3 dir, float knockBackDistance, float lerpTime)
    {
        Vector3 start = transform.position; // 시작 위치
        Vector3 end = start + dir * knockBackDistance; // 도착 위치
        float elapsed = 0f;

        // 보간을 이용해서 부드럽게 KnockBack
        while (elapsed < lerpTime)
        {
            transform.position = Vector3.Lerp(start, end, elapsed / lerpTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = end;
    }

    // 페이드 아웃 후 죽음 처리
    protected IEnumerator FadeOutOnDeath()
    {
        if (_monsterMaterials != null)
        {
            _monsterAnimator.SetTrigger("Dead");
            // 페이드 아웃
            float elapsed = 0.0f;
            while (elapsed < _fadeLerpTimer)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / _fadeLerpTimer;

                foreach (Material monsterMaterial in _monsterMaterials)
                {
                    // 알파 값 조절
                    Color color = monsterMaterial.color;
                    color.a = Mathf.Lerp(color.a, _minAlphaValue, t);
                    SetMonsterHpBarAlpha(color.a);
                    monsterMaterial.color = color;
                }

                yield return null;
            }

            foreach (Material monsterMaterial in _monsterMaterials)
            {
                // 최종 알파값 설정
                Color finalColor = monsterMaterial.color;
                finalColor.a = _minAlphaValue;
                SetMonsterHpBarAlpha(finalColor.a);
                monsterMaterial.color = finalColor;
            }

            gameObject.SetActive(false);
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
            // 무기 공격력 만큼 데미지주기
            float damage = other.GetComponent<Weapon>().WeaponAttackPower;
            MonsterGetDamage(damage);
        }

        // 플레이어랑 트리거 체크되면 플레이어 데미지 주기
        if (other.CompareTag("Player"))
        {
            _player.gameObject.GetComponent<PlayerGetDamage>().GetDamage(_attackPower);
        }
    }

    protected void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _attackTimer += Time.deltaTime;
            if (_attackTimer >= _monsterStatus.AttackInterval)
            {
                _attackTimer -= _monsterStatus.AttackInterval;
                _player.gameObject.GetComponent<PlayerGetDamage>().GetDamage(_attackPower);
            }
        }
    }

    protected void OnTriggerExit(Collider other)
    {
        // 플레이어와 충돌이 멈췄다면 초기화
        if (other.CompareTag("Player"))
        {
            _attackTimer = 0.0f;
        }
    }
}