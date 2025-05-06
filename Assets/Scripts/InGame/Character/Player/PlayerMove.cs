using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private AudioClip _playerRunSoundClip;
    private AudioSource _playerRunSound;
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

        _playerRunSoundClip = Resources.Load<AudioClip>("Sounds/PlayerRunSound");
        _playerRunSound = GetComponent<AudioSource>();
        _playerRunSound.clip = _playerRunSoundClip;
        _playerRunSound.volume = 0.5f;
    }

    void Update()
    {
        if (!_isMoveStop)
        {
            Move();
        }
        else
        {   
            // ���� ��Ʈ�� ��, ��ȿ �Ҷ� �ִϸ��̼� ���߱� ���Ѱ�
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

            if (!_playerRunSound.isPlaying)
            {
                _playerRunSound.loop = true; // �ݺ� ���
                _playerRunSound.Play();
            }
        }
        else
        {
            _isRunning = false;

            // ������ ������ �� �Ҹ� ����
            if (_playerRunSound.isPlaying)
            {
                _playerRunSound.Stop();
            }
        }

        _playerAnim.SetBool("IsRunning", _isRunning);
    }

    public void StopRunSound()
    {
        _playerRunSound.Stop();
    }

    public void SetPlayerRunSoundSlow()
    {
        _playerRunSound.pitch = 0.25f;
    }

    public void ResetPlayerRunSoundPitch()
    {
        _playerRunSound.pitch = 1.0f;
    }
}