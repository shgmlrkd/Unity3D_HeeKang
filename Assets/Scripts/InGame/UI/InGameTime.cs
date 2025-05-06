using System.Collections;
using TMPro;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class InGameTime : MonoBehaviour
{
    private enum TimerPhase
    {
        Countdown, // 300초 → 0초
        Countup    // 0초부터 상승
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
        
        // 게임 시작 5분이 지나면
        if (_timerPhase == TimerPhase.Countdown && _inGameTimer <= 0.0f && _timerCoroutine != null)
        {
            StopCoroutine(_timerCoroutine); // 시간 코루틴은 멈춤
            _timerCoroutine = null; // 코루틴을 멈췄으므로 null로 설정

            _playerMove.IsMoveStop = true;
            _isBossSpawnTime = true; // 이 시간이 지나면 보스 몹을 소환시키기위한 변수

            _playerSkill.DisablePlayerSkills(); // 카메라 흔들릴 때 스킬 잠궈놓기
            _camShake.StartShake(_camShakeDuration); // 카메라 흔들림을 시작
        }

        // 카메라 흔들림이 끝나면 타이머를 0초부터 다시 시작
        if (_camShake.IsShakeEnd && !_isInitTimer)
        {
            _inGameTimer = 0.0f; // 시간 초기화
            _isInitTimer = true;
            _playerMove.IsMoveStop = false;
            _playerSkill.EnablePlayerSkills(); // 카메라 흔들림 끝나고 다시 스킬 해제

            _timerPhase = TimerPhase.Countup; // 상태 전환
            _timerCoroutine = StartCoroutine(UpdateTimerCoroutine());
        }
    }

    private IEnumerator UpdateTimerCoroutine()
    {
        while (true)
        {
            // 처음 5분 타이머
            if (_timerPhase == TimerPhase.Countdown)
            {
                _inGameTimer -= Time.deltaTime;
                _countupTimer += Time.deltaTime;
                if (_inGameTimer <= 0.0f)
                {
                    _inGameTimer = 0.0f;
                }
            } // 이후 보스 1 : 1 타이머 (0초부터 시작)
            else if (_timerPhase == TimerPhase.Countup)
            {
                _inGameTimer += Time.deltaTime;
            }

            yield return null;
        }
    }
}