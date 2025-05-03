using System.Collections;
using TMPro;
using UnityEngine;

public class InGameTime : MonoBehaviour
{
    private PlayerSkill _playerSkill;
    private CameraShaking _camShake;
    private Coroutine _timerCoroutine;
    private TextMeshProUGUI _timerText;

    private readonly float _oneMinute = 60.0f;
    private readonly float _camShakeDuration = 5.0f;

    private float _inGameTimerInitTime;
    private float _minute;
    private float _second;
    private float _inGameTimer;
    public float InGameTimer
    {
        get { return _inGameTimer; }
    }

    private bool _isBossSpawnTime = false;
    public bool IsBossSpawnTime
    {
        get { return _isBossSpawnTime; }
    }
    private bool _isPlayerStop = false;
    public bool IsPlayerStop
    {
        get { return _isPlayerStop; }
    }

    private bool _isInitTimer = false;
   

    private void Awake()
    {
        _minute = 0.0f;
        _second = 0.0f;
        _inGameTimer = 0.0f;
    }

    private void Start()
    {
        _playerSkill = InGameManager.Instance.Player.GetComponent<PlayerSkill>();
        _timerText = GetComponent<TextMeshProUGUI>();
        _timerCoroutine = StartCoroutine(UpdateTimerCoroutine());
        _inGameTimerInitTime = MonsterManager.Instance.InitTime;
        _camShake = GameObject.Find("Main Camera").GetComponent<CameraShaking>();
    }

    private void Update()
    {
        _minute = Mathf.FloorToInt(_inGameTimer / _oneMinute);
        _second = Mathf.FloorToInt(_inGameTimer % _oneMinute);

        _timerText.text = $"{_minute.ToString("00") + " : " + _second.ToString("00")}";
        
        // 게임 시작 5분이 지나면
        if (_inGameTimer > _inGameTimerInitTime && _timerCoroutine != null)
        {
            StopCoroutine(_timerCoroutine); // 시간 코루틴은 멈춤
            _timerCoroutine = null; // 코루틴을 멈췄으므로 null로 설정
            _isPlayerStop = true; 
            _isBossSpawnTime = true; // 이 시간이 지나면 보스 몹을 소환시키기위한 변수
            _playerSkill.DisablePlayerSkillsForBossIntro(); // 카메라 흔들릴 때 스킬 잠궈놓기
            _camShake.StartShake(_camShakeDuration); // 카메라 흔들림을 시작
        }

        // 카메라 흔들림이 끝나면 타이머를 다시 시작
        if (_camShake.IsShakeEnd && !_isInitTimer)
        {
            _inGameTimer = 0.0f; // 시간을 다시 0분 0초로 초기화
            _isInitTimer = true;
            _isPlayerStop = false;
            _playerSkill.EnablePlayerSkillsAfterBossIntro(); // 카메라 흔들림 끝나고 스킬 해제
            StartCoroutine(UpdateTimerCoroutine());
        }
    }

    private IEnumerator UpdateTimerCoroutine()
    {
        while (true)
        {
            _inGameTimer += Time.deltaTime;
            yield return null;
        }
    }
}