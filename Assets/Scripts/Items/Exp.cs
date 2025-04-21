using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;

public class Exp : MonoBehaviour
{
    private Transform _player;

    private Vector3 _rotationY;
    private Vector3 _directionToExp;

    private float _rotationSpeed = 100.0f; // ����ġ ������ �� ȸ�� �ӵ�
    private float _moveDistance = 0.9f; // ����ġ ���� �� �ݴ� �������� ���� �Ÿ� ���
    private float _timeToReach = 0.35f; // ����ġ�� �÷��̾� �ݴ� �������� ���� �ð�
    private float _pickUpDistance = 0.5f; // ����ġ�� �÷��̾� �������� ���ƿͼ� ����ġ ��Ȱ��ȭ ��Ű�� �Ÿ�
    private float _timePassed = 0.0f; // �̵� �ð��� ����� �ð�

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
        if (!_isCollision) // �浹 ������ ����
        {
            // ���ڸ����� Y�� ȸ��
            transform.Rotate(_rotationY * _rotationSpeed * Time.deltaTime);
        }
        else // �浹 ��
        {
            // ����ġ ������ ����
            MoveExpItem();
        }
    }

    private void MoveExpItem()
    {
        if (!_isReachedTargetPos)
        {
            // �������� �ݴ� �������� �̵��� ��ǥ ��ġ
            // _directionToExp�� �������� �̵��� ���� ����, _moveDistance�� �̵��� �Ÿ�
            Vector3 targetPosition = transform.position + _directionToExp * _moveDistance;

            // �ð� ���
            _timePassed += Time.deltaTime;
            // Lerp�� ����Ͽ� ���� ��ġ���� ��ǥ ��ġ�� �ε巴�� �̵�
            transform.position = Vector3.Lerp(transform.position, targetPosition, _timePassed);

            // _timeToReach �ð��� �������� ��ǥ ��ġ���� �̵�
            if (_timePassed >= _timeToReach)
            {
                _isReachedTargetPos = true; // ��ǥ ��ġ ����
                _timePassed = 0.0f;  // �ð� �ʱ�ȭ
            }
        }
        else
        {
            _timePassed += Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, _player.position, _timePassed);

            float distance = Vector3.Distance(transform.position, _player.position);

            // �ٽ� �÷��̾� ��ġ�� ���ư��� �� ����ġ�� �߰�
            if (distance <= _pickUpDistance)
            {
                // ����ġ++
                _player.gameObject.GetComponent<PlayerExpBar>().SetPlayerCurExp(_exp);
                gameObject.SetActive(false); // ������ ��Ȱ��ȭ
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
            // �÷��̾ ����ġ ���������� ���ϴ� ���� ����
            _directionToExp = (transform.position - _player.position).normalized;
            _directionToExp.y = 0.0f;
        }
    }
}
