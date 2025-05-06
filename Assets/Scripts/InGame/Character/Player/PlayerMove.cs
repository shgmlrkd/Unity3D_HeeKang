using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private Animator _playerAnim;
    private PlayerStatus _player;

    private float _playerRotateSpeed = 12.0f;
    private bool _isRunning = false;
    private bool _isMoveStop = false;
    public bool IsMoveStop
    {
        get { return _isMoveStop; }
        set { _isMoveStop = value; }
    }

    private void Start()
    {
        _player = GetComponent<PlayerStatus>();
        _playerAnim = GetComponent<Animator>();
    }

    void Update()
    {
        if (!_isMoveStop)
        {
            Move();
        }
        else
        {   
            // 보스 인트로 씬, 포효 할때 애니메이션 멈추기 위한거
            _playerAnim.SetBool("IsRunning", false);
        }
    }

    private void Move()
    {
        Vector3 inputDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if (inputDir.sqrMagnitude > 0)
        {
            _isRunning = true;
            transform.Translate(inputDir.normalized * _player.Status.Speed * Time.deltaTime, Space.World);

            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(inputDir), Time.deltaTime * _playerRotateSpeed);
        }
        else
        {
            _isRunning = false;
        }

        _playerAnim.SetBool("IsRunning", _isRunning);
    }
}
