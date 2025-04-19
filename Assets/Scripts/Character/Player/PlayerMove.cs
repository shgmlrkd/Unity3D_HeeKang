using UnityEngine;

public class PlayerMove : Player
{
    private Animator _playerAnim;

    private float _playerRotateSpeed = 12.0f;
    private bool _isRunning = false;

    protected override void Start()
    {
        base.Start();
        _playerAnim = GetComponent<Animator>();
    }

    void Update()
    {
        Move();
    }

    private void Move()
    {
        Vector3 inputDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if (inputDir.sqrMagnitude > 0)
        {
            _isRunning = true;
            transform.Translate(inputDir.normalized * _speed * Time.deltaTime, Space.World);

            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(inputDir), Time.deltaTime * _playerRotateSpeed);
        }
        else
        {
            _isRunning = false;
        }

        _playerAnim.SetBool("IsRunning", _isRunning);
    }
}
