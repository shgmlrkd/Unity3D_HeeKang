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

        // ü�¹� ����
        GameObject _monsterHpBarPanel = GameObject.Find("InGameCanvas/MonsterHpBarPanel");
        GameObject _monsterHpBar = Instantiate(_monsterHpBarPrefab, _monsterHpBarPanel.transform);
        _monsterHpBarSlider = _monsterHpBar.GetComponent<Slider>();
        _monsterHpBarSlider.gameObject.SetActive(false);

        _maxHp = _baseHp;
        _curHp = _maxHp;
        _attackPower = _baseAtk;

        // ü�¹� �̹����� �޾ƿ���
        _monsterHpBarImages = _monsterHpBar.GetComponentsInChildren<Image>();
        SetMonsterHpBarAlpha(_one);

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
        // ������ ������ �Ϸ�Ǿ��� ��� 
        if (_isStatSettingEnd)
        {
            // �ΰ��� �ð� �޾ƿ���
            _inGameTime = InGameUIManager.Instance.GetInGameTimer();
            // ���� ��ȭ Ƚ�� ���
            int upgradeCount = MonsterUpgradeManager.GetUpgradeCount(_inGameTime, _monsterStatus.SpawnStartTime, _monsterStatus.StatUpdateInterval);
            // �������� ��ȭ Ƚ���� �������� ��� �ɷ�ġ ����
            if (upgradeCount > _prevUpgradeCount)
            {
                float scale = MonsterUpgradeManager.GetScaleFactor(upgradeCount, _monsterStatus.StatUpdateInterval, _monsterStatus.StateScaleFactor);
                // HP �� ���ݷ� ����
                _maxHp = _baseHp * scale;
                _curHp = _maxHp;
                _attackPower = _baseAtk * scale;
                // ���� ��ȭ Ƚ�� ����
                _prevUpgradeCount = upgradeCount;
            }
        }

        // �ʱ�ȭ
        _curHp = _maxHp;

        _hpBarVisibleTimer = 0.0f;
        _monsterCurrentState = MonsterStatus.Run;

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
        // ���İ� �ǵ�����
        if (_monsterMaterials != null)
        {
            // ���İ� 1�� ��������
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
        // �ݶ��̴� ������ �ʱ�ȭ
        StopAttackIfColliderOff();
        // ü�¹� ��ġ�� �׻� ����
        ShowHpBar();
        // ü�¹� ���̵� �ƿ� ����
        FadeOutMonsterHpBar();
        // �÷��̾�� �Ÿ��� �ʹ� �ָ� �ݴ������� ������
        Reposition();
        // ���� �׼� FSM
        Action();
    }

    private void Action()
    {
        // ������ ���� ���¿� ���� �ൿ ó��
        switch (_monsterCurrentState)
        {
            // ���Ͱ� Run ���¸� ���� Ȯ�� �� Move() ����
            case MonsterStatus.Run:
                if (CanMove())
                { 
                    Move();
                }
                break;
            // ���Ͱ� Hit ���¸� HandleHitState() ����
            case MonsterStatus.Hit:
                HandleHitState();
                break;
            // ���Ͱ� Rush ���¸� HandleRushState() ����
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
    private void StopAttackIfColliderOff()
    {
        if (!_monsterCollider.enabled)
        {
            _attackTimer = 0.0f;
        }
    }

    // ü�� ���� �����ؼ� �����ֱ�
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
        // �Ķ���Ϳ� �Ű������� ���� paramName�� �ִ��� Ȯ��
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == paramName)
                return true;
        }
        return false;
    }

    private void InitHpBarEffect()
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
    }

    // ü�¹� ���İ� ����
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

    protected virtual void Move()
    {
        _monsterCurrentState = MonsterStatus.Run;
        // �÷��̾� �������� �̵�, ȸ��
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

    // �⺻������ Run ���·� ���ư�
    protected virtual void HandleHitState()
    {
        _monsterCurrentState = MonsterStatus.Run;
    }
    protected virtual void HandleRushState()
    {
        // �⺻�� �ƹ��͵� ����
    }

    protected virtual void HandleAttackState()
    {
        _monsterCurrentState = MonsterStatus.Run;
    }

    protected virtual void HandleDeadState()
    {
        // �⺻�� �ƹ��͵� ����
    }

    // ���� ������ ����
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

            // Ű���� ���� ������ ����
            MonsterData data = MonsterDataManager.Instance.GetMonsterData(key);
            SetMonsterData(data, MonsterDataManager.Instance.GetMonsterSpawnData(data.Name));
        }
    }

    public virtual void MonsterGetDamage(float damage)
    {
        InitHpBarEffect();

        // �ǰ�
        _curHp -= damage;

        // ������ �״� �ִϸ��̼�, �浹ü ���� ü�¹� ����
        if (_curHp <= 0)
        {
            // ���� ���� ȸ��, ���� �ݶ��̴� ����
            _curHp = 0;
            InGameUIManager.Instance.SetKillCountText();
            _monsterCollider.enabled = false;

            // ����ġ�� ���� ������ ������ (����ġ 100%, ������ Ȯ��)
            ItemManager.Instance.SpawnExp(_monsterStatus.Exp, transform.position);
            ItemManager.Instance.SpawnRandomItem(transform.position);

            // ���̵� �ƿ��ϸ鼭 �״¾ִϸ��̼�
            StartCoroutine(FadeOutOnDeath());
            _monsterCurrentState = MonsterStatus.Dead;
        }
        else
        {
            // Hit�̶�� �Ķ���Ͱ� �ִϸ��̼ǿ� �ִ��� üũ
            if (HasAnimParameter(_monsterAnimator, "Hit"))
            {
                // �´� �ִϸ��̼�
                _monsterAnimator.SetTrigger("Hit");
                _monsterCurrentState = MonsterStatus.Hit;
            }
        }
    }

    public void MonsterKnockBack(Vector3 dir, float knockBackDistance, float duration)
    {
        // KnockBack ����
        StartCoroutine(KnockBackRoutine(dir, knockBackDistance, duration));
    }

    private IEnumerator KnockBackRoutine(Vector3 dir, float knockBackDistance, float lerpTime)
    {
        Vector3 start = transform.position; // ���� ��ġ
        Vector3 end = start + dir * knockBackDistance; // ���� ��ġ
        float elapsed = 0f;

        // ������ �̿��ؼ� �ε巴�� KnockBack
        while (elapsed < lerpTime)
        {
            transform.position = Vector3.Lerp(start, end, elapsed / lerpTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = end;
    }

    // ���̵� �ƿ� �� ���� ó��
    protected IEnumerator FadeOutOnDeath()
    {
        if (_monsterMaterials != null)
        {
            _monsterAnimator.SetTrigger("Dead");
            // ���̵� �ƿ�
            float elapsed = 0.0f;
            while (elapsed < _fadeLerpTimer)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / _fadeLerpTimer;

                foreach (Material monsterMaterial in _monsterMaterials)
                {
                    // ���� �� ����
                    Color color = monsterMaterial.color;
                    color.a = Mathf.Lerp(color.a, _minAlphaValue, t);
                    SetMonsterHpBarAlpha(color.a);
                    monsterMaterial.color = color;
                }

                yield return null;
            }

            foreach (Material monsterMaterial in _monsterMaterials)
            {
                // ���� ���İ� ����
                Color finalColor = monsterMaterial.color;
                finalColor.a = _minAlphaValue;
                SetMonsterHpBarAlpha(finalColor.a);
                monsterMaterial.color = finalColor;
            }

            gameObject.SetActive(false);
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
            // ���� ���ݷ� ��ŭ �������ֱ�
            float damage = other.GetComponent<Weapon>().WeaponAttackPower;
            MonsterGetDamage(damage);
        }

        // �÷��̾�� Ʈ���� üũ�Ǹ� �÷��̾� ������ �ֱ�
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
        // �÷��̾�� �浹�� ����ٸ� �ʱ�ȭ
        if (other.CompareTag("Player"))
        {
            _attackTimer = 0.0f;
        }
    }
}