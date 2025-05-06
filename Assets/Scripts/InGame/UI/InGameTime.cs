using System.Collections;
using TMPro;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class InGameTime : MonoBehaviour
{
    private enum TimerPhase
    {
        Countdown, // 300�� �� 0��
        Countup    // 0�ʺ��� ���
    }

    private PlayerMove _playerMove;
    private PlayerSkill _playerSkill;
    private CameraShaking _camShake;
    private Coroutine _timerCoroutine;
    private TextMeshProUGUI _timerText;

    private TimerPhase _timerPhase = TimerPhase.Countdown;

    private readonly float _oneMinute = 60.0f;
    private readonly float _camShakeDuration = 5.0f;

    private float _minute;
    private float _second;
    private float _inGameTimer;
    private float _countupTimer;
    public float InGameTimer
    {
        get { return _countupTimer; }
    }

    private bool _isBossSpawnTime = false;
    public bool IsBossSpawnTime
    {
        get { return _isBossSpawnTime; }
    }

    private bool _isInitTimer = false;
   
    private void Awake()
    {
        _minute = 0.0f;
        _second = 0.0f;
    }

    private void Start()
    {
        _inGameTimer = 1;// MonsterManager.Instance.InitTime;
        _countupTimer = 0.0f;
        _timerPhase = TimerPhase.Countdown;
        _playerMove = InGameManager.Instance.Player.GetComponent<PlayerMove>();
        _playerSkill = InGameManager.Instance.Player.GetComponent<PlayerSkill>();
        _timerText = GetComponent<TextMeshProUGUI>();
        _timerCoroutine = StartCoroutine(UpdateTimerCoroutine());
        _camShake = GameObject.Find("Main Camera").GetComponent<CameraShaking>();
    }

    private void Update()
    {
        _minute = Mathf.FloorToInt(_inGameTimer / _oneMinute);
        _second = Mathf.FloorToInt(_inGameTimer % _oneMinute);

        _timerText.text = $"{_minute.ToString("00") + " : " + _second.ToString("00")}";
        
        // ���� ���� 5���� ������
        if (_timerPhase == TimerPhase.Countdown && _inGameTimer <= 0.0f && _timerCoroutine != null)
        {
            StopCoroutine(_timerCoroutine); // �ð� �ڷ�ƾ�� ����
            _timerCoroutine = null; // �ڷ�ƾ�� �������Ƿ� null�� ����

            _playerMove.IsMoveStop = true;
            _isBossSpawnTime = true; // �� �ð��� ������ ���� ���� ��ȯ��Ű������ ����

            _playerSkill.DisablePlayerSkills(); // ī�޶� ��鸱 �� ��ų ��ų���
            _camShake.StartShake(_camShakeDuration); // ī�޶� ��鸲�� ����
        }

        // ī�޶� ��鸲�� ������ Ÿ�̸Ӹ� 0�ʺ��� �ٽ� ����
        if (_camShake.IsShakeEnd && !_isInitTimer)
        {
            _inGameTimer = 0.0f; // �ð� �ʱ�ȭ
            _isInitTimer = true;
            _playerMove.IsMoveStop = false;
            _playerSkill.EnablePlayerSkills(); // ī�޶� ��鸲 ������ �ٽ� ��ų ����

            _timerPhase = TimerPhase.Countup; // ���� ��ȯ
            _timerCoroutine = StartCoroutine(UpdateTimerCoroutine());
        }
    }

    private IEnumerator UpdateTimerCoroutine()
    {
        while (true)
        {
            // ó�� 5�� Ÿ�̸�
            if (_timerPhase == TimerPhase.Countdown)
            {
                _inGameTimer -= Time.deltaTime;
                _countupTimer += Time.deltaTime;
                if (_inGameTimer <= 0.0f)
                {
                    _inGameTimer = 0.0f;
                }
            } // ���� ���� 1 : 1 Ÿ�̸� (0�ʺ��� ����)
            else if (_timerPhase == TimerPhase.Countup)
            {
                _inGameTimer += Time.deltaTime;
            }

            yield return null;
        }
    }
}