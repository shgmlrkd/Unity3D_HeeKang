using UnityEngine;
using UnityEngine.UI;

public class Skeleton : Monster
{
    private Slider _skeletonHpBarSlider;
    private Animator _skeletonAnimator;
    private GameObject _skeletonHpBarPrefab;

    private Vector3 _skeletonHpBarOffset;

    private float _attackTimer = 0.0f;
    private int _skeletonKey = 101;

    private bool _isAttackAble = false;

    private void Awake()
    {
        _skeletonHpBarPrefab = Resources.Load<GameObject>("Prefabs/MonsterUI/MonsterHpBar");
        _skeletonHpBarOffset = new Vector3(0.0f, 1.5f, 0.0f);
    }

    protected new void Start()
    {
        base.Start();

        // 키값에 따른 몬스터 데이터 세팅
        SetMonsterData(MonsterDataManager.Instance.GetMonsterData(_skeletonKey));

        // 체력바 생성
        GameObject _skeletonHpBarPanel = GameObject.Find("HpBarPanel");
        GameObject _skeletonHpBar = Instantiate(_skeletonHpBarPrefab, _skeletonHpBarPanel.transform);
        _skeletonHpBarSlider = _skeletonHpBar.GetComponent<Slider>();

        _skeletonAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        // 체력바 위치는 항상 갱신
        ShowHpBar();

        // 움직일 수 있는 상태에서만 동작
        if (CanMove())
        {
            Move();
            Attack();
        }
    }

    private void Move()
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

    private void ShowHpBar()
    {
        // 체력 상태 갱신해서 보여주기
        Vector3 worldPos = transform.position + _skeletonHpBarOffset;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        if (screenPos.z > 0)
        {
            _skeletonHpBarSlider.transform.position = screenPos;
            _skeletonHpBarSlider.value = _curHp / _maxHp;
        }
    }

    private void Attack()
    {
        // 공격이 가능한 상태라면 Interval 주기로 공격
        if(_isAttackAble)
        {
            _attackTimer += Time.deltaTime;
            if(_attackTimer >= _attackInterval)
            {
                _attackTimer -= _attackInterval;
                _player.gameObject.GetComponent<PlayerGetDamage>().GetDamage(_attackPower);
            }
        }
    }

    private bool CanMove()
    {
        // 애니메이션에 Base Layer를 가져온거고 Base Layer는 인덱스가 0 이어서 매개변수가 0임
        AnimatorStateInfo stateInfo = _skeletonAnimator.GetCurrentAnimatorStateInfo(0);
        bool isInHit = stateInfo.IsName("Hit");
        bool isInDead = stateInfo.IsName("Dead");
        // Hit이나 Dead 상태가 아니라면 true 반환
        return !(isInHit || isInDead);
    }

    private void OnTriggerEnter(Collider other)
    {
        // 플레이어랑 트리거 체크되면 플레이어 데미지 주기
        if(other.CompareTag("Player"))
        {
            _isAttackAble = true;
            _player.gameObject.GetComponent<PlayerGetDamage>().GetDamage(_attackPower);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // 플레이어와 충돌이 멈췄다면 초기화
        if (other.CompareTag("Player"))
        {
            _isAttackAble = false;
            _attackTimer = 0.0f;
        }
    }
}