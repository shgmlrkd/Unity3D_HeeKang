using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.UI;

public class Skeleton : Monster
{
    private int _skeletonKey = 101;

    private void Awake()
    {
        _monsterHpBarPrefab = Resources.Load<GameObject>("Prefabs/MonsterUI/MonsterHpBar");
        _monsterHpBarOffset = new Vector3(0.0f, 1.5f, 0.0f);
    }

    private void OnEnable()
    {
        base.OnEnable();
    }

    private void Start()
    {
        base.Start();

        // Ű���� ���� ���� ������ ����
        SetMonsterData(MonsterDataManager.Instance.GetMonsterData(_skeletonKey));
    }

    private void Update()
    { 
        base.Update();

        // ü�¹� ��ġ�� �׻� ����
        ShowHpBar();
        // �÷��̾�� �Ÿ��� �ʹ� �ָ� �ݴ������� ������
        Reposition();
        // �ݶ��̴��� ������ ���� ���߱�
        StopAttack();

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
        Vector3 worldPos = transform.position + _monsterHpBarOffset;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        if (screenPos.z > 0)
        {
            _monsterHpBarSlider.transform.position = screenPos;
            _monsterHpBarSlider.value = _curHp / _maxHp;
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

    private void StopAttack()
    {
        if (!_monsterCollider.enabled)
        {
            _isAttackAble = false;
            _attackTimer = 0.0f;
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
        if(distance >= _distanceThreshold)
        {
            // �ݴ� �������� �̵�
            transform.position += Vector3.Scale(dir, _moveOffset);
        }
    }

    private bool CanMove()
    {
        // �ִϸ��̼ǿ� Base Layer�� �����°Ű� Base Layer�� �ε����� 0 �̾ �Ű������� 0��
        AnimatorStateInfo stateInfo = _monsterAnimator.GetCurrentAnimatorStateInfo(0);
        bool isInHit = stateInfo.IsName("Hit");
        bool isInDead = stateInfo.IsName("Dead");

        // Hit�̳� Dead ���°� �ƴ϶�� true ��ȯ
        return !(isInHit || isInDead);
    }

    private void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

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