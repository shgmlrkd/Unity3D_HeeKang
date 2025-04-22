using UnityEngine;

public class MeleeMonster : Monster
{
    protected void Attack()
    {
        // ������ ������ ���¶�� Interval �ֱ�� ����
        if (_isAttackAble)
        {
            _attackTimer += Time.deltaTime;
            if (_attackTimer >= _attackInterval)
            {
                _attackTimer -= _attackInterval;
                _player.gameObject.GetComponent<PlayerGetDamage>().GetDamage(_attackPower);
            }
        }
    }

    public void OnHit()
    {
        _isHit = true;
    }

    public void OnAttackAble()
    {
        _isHit = false;
    }
    
    protected void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        // �÷��̾�� Ʈ���� üũ�Ǹ� �÷��̾� ������ �ֱ�
        if (other.CompareTag("Player") && !_isHit)
        {
            _isAttackAble = true;
            _player.gameObject.GetComponent<PlayerGetDamage>().GetDamage(_attackPower);
        }
    }

    protected void OnTriggerExit(Collider other)
    {
        // �÷��̾�� �浹�� ����ٸ� �ʱ�ȭ
        if (other.CompareTag("Player"))
        {
            _isAttackAble = false;
            _attackTimer = 0.0f;
        }
    }

}