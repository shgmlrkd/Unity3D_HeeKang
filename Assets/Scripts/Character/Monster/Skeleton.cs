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

        // Ű���� ���� ���� ������ ����
        SetMonsterData(MonsterDataManager.Instance.GetMonsterData(_skeletonKey));

        // ü�¹� ����
        GameObject _skeletonHpBarPanel = GameObject.Find("HpBarPanel");
        GameObject _skeletonHpBar = Instantiate(_skeletonHpBarPrefab, _skeletonHpBarPanel.transform);
        _skeletonHpBarSlider = _skeletonHpBar.GetComponent<Slider>();

        _skeletonAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        // ü�¹� ��ġ�� �׻� ����
        ShowHpBar();

        // ������ �� �ִ� ���¿����� ����
        if (CanMove())
        {
            Move();
            Attack();
        }
    }

    private void Move()
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

    private void ShowHpBar()
    {
        // ü�� ���� �����ؼ� �����ֱ�
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
        // ������ ������ ���¶�� Interval �ֱ�� ����
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
        // �ִϸ��̼ǿ� Base Layer�� �����°Ű� Base Layer�� �ε����� 0 �̾ �Ű������� 0��
        AnimatorStateInfo stateInfo = _skeletonAnimator.GetCurrentAnimatorStateInfo(0);
        bool isInHit = stateInfo.IsName("Hit");
        bool isInDead = stateInfo.IsName("Dead");
        // Hit�̳� Dead ���°� �ƴ϶�� true ��ȯ
        return !(isInHit || isInDead);
    }

    private void OnTriggerEnter(Collider other)
    {
        // �÷��̾�� Ʈ���� üũ�Ǹ� �÷��̾� ������ �ֱ�
        if(other.CompareTag("Player"))
        {
            _isAttackAble = true;
            _player.gameObject.GetComponent<PlayerGetDamage>().GetDamage(_attackPower);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // �÷��̾�� �浹�� ����ٸ� �ʱ�ȭ
        if (other.CompareTag("Player"))
        {
            _isAttackAble = false;
            _attackTimer = 0.0f;
        }
    }
}