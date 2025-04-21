using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;

public class Exp : MonoBehaviour
{
    private Transform _player;

    private Vector3 _rotationY;
    private Vector3 _directionToExp;

    private float _rotationSpeed = 100.0f; // 경험치 나왔을 때 회전 속도
    private float _moveDistance = 0.9f; // 경험치 먹을 때 반대 방향으로 가는 거리 계수
    private float _timeToReach = 0.35f; // 경험치가 플레이어 반대 방향으로 가는 시간
    private float _pickUpDistance = 0.5f; // 경험치가 플레이어 방향으로 돌아와서 경험치 비활성화 시키는 거리
    private float _timePassed = 0.0f; // 이동 시간이 경과한 시간

    private int _exp;

    private bool _isCollision = false;
    private bool _isReachedTargetPos = false;

    private void Awake()
    {
        _rotationY = Vector3.up;
    }

    private void OnEnable()
    {
        _timePassed = 0.0f;
        _isCollision = false;
        _isReachedTargetPos = false;
    }

    private void Start()
    {
        _player = GameManager.Instance.Player.transform;
    }

    private void Update()
    { 
        if (!_isCollision) // 충돌 안했을 때는
        {
            // 제자리에서 Y축 회전
            transform.Rotate(_rotationY * _rotationSpeed * Time.deltaTime);
        }
        else // 충돌 시
        {
            // 경험치 아이템 연출
            MoveExpItem();
        }
    }

    private void MoveExpItem()
    {
        if (!_isReachedTargetPos)
        {
            // 아이템이 반대 방향으로 이동할 목표 위치
            // _directionToExp는 아이템이 이동할 방향 벡터, _moveDistance는 이동할 거리
            Vector3 targetPosition = transform.position + _directionToExp * _moveDistance;

            // 시간 경과
            _timePassed += Time.deltaTime;
            // Lerp를 사용하여 현재 위치에서 목표 위치로 부드럽게 이동
            transform.position = Vector3.Lerp(transform.position, targetPosition, _timePassed);

            // _timeToReach 시간을 기준으로 목표 위치까지 이동
            if (_timePassed >= _timeToReach)
            {
                _isReachedTargetPos = true; // 목표 위치 도달
                _timePassed = 0.0f;  // 시간 초기화
            }
        }
        else
        {
            _timePassed += Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, _player.position, _timePassed);

            float distance = Vector3.Distance(transform.position, _player.position);

            // 다시 플레이어 위치로 돌아갔을 때 경험치를 추가
            if (distance <= _pickUpDistance)
            {
                // 경험치++
                _player.gameObject.GetComponent<PlayerExpBar>().SetPlayerCurExp(_exp);
                gameObject.SetActive(false); // 아이템 비활성화
            }
        }
    }

    public void SetExp(int value, Vector3 pos)
    {
        _exp = value;
        transform.position = pos;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            _isCollision = true;
            // 플레이어가 경험치 아이템으로 향하는 방향 벡터
            _directionToExp = (transform.position - _player.position).normalized;
            _directionToExp.y = 0.0f;
        }
    }
}
