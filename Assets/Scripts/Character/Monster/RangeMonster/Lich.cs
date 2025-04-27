using System.Collections;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;

public class Lich : FlashDamagedMonster
{
    private LichFireBallSkill _lichFireBallSkill;

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
        gameObject.AddComponent<LichFireBallSkill>();
        _lichFireBallSkill = GetComponent<LichFireBallSkill>();
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
        if(_distance <= _monsterStatus.AttackDistance)
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
            _monsterAnimator.SetTrigger("Fire");
            StartCoroutine(FireRoutine(direction));
        }
    }

    private IEnumerator FireRoutine(Vector3 dir)
    {
        _canFireNow = false; // 발사했으니까 잠깐 막음

        _lichFireBallSkill.Fire(dir); // 발사
        yield return _lichFireBallSkill.FireInterval; // 기다림

        _canFireNow = true; // 다시 발사 가능
    }
}