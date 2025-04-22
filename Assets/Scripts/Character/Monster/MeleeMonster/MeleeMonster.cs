using UnityEngine;

public class MeleeMonster : Monster
{
    protected void Attack()
    {
        // 공격이 가능한 상태라면 Interval 주기로 공격
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

        // 플레이어랑 트리거 체크되면 플레이어 데미지 주기
        if (other.CompareTag("Player") && !_isHit)
        {
            _isAttackAble = true;
            _player.gameObject.GetComponent<PlayerGetDamage>().GetDamage(_attackPower);
        }
    }

    protected void OnTriggerExit(Collider other)
    {
        // 플레이어와 충돌이 멈췄다면 초기화
        if (other.CompareTag("Player"))
        {
            _isAttackAble = false;
            _attackTimer = 0.0f;
        }
    }

}