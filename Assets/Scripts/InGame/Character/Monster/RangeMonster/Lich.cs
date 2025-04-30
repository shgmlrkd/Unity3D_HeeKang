using System.Collections;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;

public class Lich : FlashDamagedMonster
{
    private MonsterFireBallSkill _monsterFireBallSkill;

    private float _distance;
    private int _lichKey = 105;

    private bool _canFireNow = true;

    private void Awake()
    {
        base.Awake();

        _flashColor = Color.red;
    }

    private void Start()
    {
        base.Start();
        _monsterFireBallSkill = GetComponent<MonsterFireBallSkill>();
    }

    private void OnEnable()
    {
        SetMonsterKey(_lichKey);

        base.OnEnable();
    }

    protected override void Move()
    {
        _monsterCurrentState = MonsterStatus.Run;

        _distance = Vector3.Distance(_player.position, transform.position);

        // �÷��̾� �������� �̵�, ȸ��
        Vector3 direction = (_player.position - transform.position).normalized;
        direction.y = 0.0f;

        // �÷��̾���� �Ÿ��� �ָ�
        if (direction.sqrMagnitude > 0 && _distance >= _monsterStatus.AttackDistance)
        {
            transform.Translate(direction * _monsterStatus.Speed * Time.deltaTime, Space.World);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * _monsterStatus.RotSpeed);
        }
        else // ���� �Ÿ��� �Ǹ� attack ����
        {
            // �÷��̾ ���� �ȿ� ���� �������� �߻� �غ�
            if (_monsterCurrentState != MonsterStatus.Attack)
            {
                _canFireNow = true;
            }

            _monsterCurrentState = MonsterStatus.Attack;
        }
    }

    protected override bool CanMove()
    {
        _monsterAnimStateInfo = _monsterAnimator.GetCurrentAnimatorStateInfo(0);

        bool isInAttack = _monsterAnimStateInfo.IsName("Attack");
        bool isInDead = _monsterAnimStateInfo.IsName("Dead");

        return !(isInDead || isInAttack);
    }

    protected override void HandleHitState()
    {
        if (_distance <= _monsterStatus.AttackDistance)
        {
            _monsterCurrentState = MonsterStatus.Attack;
        }
        else
        {
            _monsterCurrentState = MonsterStatus.Run;
        }
    }

    protected override void HandleAttackState()
    {
        Vector3 direction = (_player.position - transform.position).normalized;
        direction.y = 0.0f;

        // ȸ�� ���� ����ֱ�
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * _monsterStatus.RotSpeed);

        // �÷��̾���� �Ÿ� ���ϱ�
        _distance = Vector3.Distance(_player.position, transform.position);

        // �־����� Run ���·� �ٲ�
        if (_distance > _monsterStatus.AttackDistance)
        {
            _monsterCurrentState = MonsterStatus.Run;
        }

        if (_canFireNow)
        {
            StartCoroutine(FireRoutine(direction));
        }
    }

    private IEnumerator FireRoutine(Vector3 dir)
    {
        _monsterAnimator.SetTrigger("Fire");
        _canFireNow = false; // �߻������ϱ� ��� ����

        _monsterFireBallSkill.Fire(dir); // �߻�

        float fireAnimLength = _monsterAnimator.GetCurrentAnimatorStateInfo(0).length;

        // FireInterval�� Fire �ִϸ��̼� ���� ��
        float remainingTime = Mathf.Max(0, _monsterFireBallSkill.AttackInterval - fireAnimLength);

        // Fire �ִϸ��̼��� ���� ������ ��ٸ�
        yield return new WaitForSeconds(fireAnimLength);

        // ����ϴٰ� �̹� Hp�� ���ٸ� ����
        if (_curHp <= 0)
            yield break;

        // Fire �ִϸ��̼��� ���� �� �ٷ� Idle ���·� ��ȯ
        _monsterAnimator.SetTrigger("Idle");

        // ���� FireInterval �ð���ŭ ��ٸ�
        yield return new WaitForSeconds(remainingTime);

        _canFireNow = true;
    }
}