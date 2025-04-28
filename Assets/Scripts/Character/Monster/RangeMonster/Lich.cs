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

        // 플레이어 방향으로 이동, 회전
        Vector3 direction = (_player.position - transform.position).normalized;
        direction.y = 0.0f;

        // 플레이어와의 거리가 멀면
        if (direction.sqrMagnitude > 0 && _distance >= _monsterStatus.AttackDistance)
        {
            transform.Translate(direction * _monsterStatus.Speed * Time.deltaTime, Space.World);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * _monsterStatus.RotSpeed);
        }
        else // 일정 거리가 되면 attack 상태
        {
            // 플레이어가 범위 안에 새로 들어왔으면 발사 준비
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

        // 회전 방향 잡아주기
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * _monsterStatus.RotSpeed);

        // 플레이어와의 거리 구하기
        _distance = Vector3.Distance(_player.position, transform.position);

        // 멀어지면 Run 상태로 바꿈
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
        _canFireNow = false; // 발사했으니까 잠깐 막음

        _monsterFireBallSkill.Fire(dir); // 발사

        float fireAnimLength = _monsterAnimator.GetCurrentAnimatorStateInfo(0).length;

        // FireInterval과 Fire 애니메이션 길이 비교
        float remainingTime = Mathf.Max(0, _monsterFireBallSkill.AttackInterval - fireAnimLength);

        // Fire 애니메이션이 끝날 때까지 기다림
        yield return new WaitForSeconds(fireAnimLength);

        // 대기하다가 이미 Hp가 없다면 종료
        if (_curHp <= 0)
            yield break;

        // Fire 애니메이션이 끝난 후 바로 Idle 상태로 전환
        _monsterAnimator.SetTrigger("Idle");

        // 남은 FireInterval 시간만큼 기다림
        yield return new WaitForSeconds(remainingTime);

        _canFireNow = true;
    }
}