using UnityEngine;

public class MeleeMonster : Monster
{
    protected void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

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