using UnityEngine;

public class MeleeMonster : Monster
{
    protected void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

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