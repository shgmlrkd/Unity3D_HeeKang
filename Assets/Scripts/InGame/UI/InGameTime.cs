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
        
        // ���� ���� 5���� ������
        if (_inGameTimer > _inGameTimerInitTime && _timerCoroutine != null)
        {
            StopCoroutine(_timerCoroutine); // �ð� �ڷ�ƾ�� ����
            _timerCoroutine = null; // �ڷ�ƾ�� �������Ƿ� null�� ����
            _isPlayerStop = true; 
            _isBossSpawnTime = true; // �� �ð��� ������ ���� ���� ��ȯ��Ű������ ����
            _playerSkill.DisablePlayerSkillsForBossIntro(); // ī�޶� ��鸱 �� ��ų ��ų���
            _camShake.StartShake(_camShakeDuration); // ī�޶� ��鸲�� ����
        }

        // ī�޶� ��鸲�� ������ Ÿ�̸Ӹ� �ٽ� ����
        if (_camShake.IsShakeEnd && !_isInitTimer)
        {
            _inGameTimer = 0.0f; // �ð��� �ٽ� 0�� 0�ʷ� �ʱ�ȭ
            _isInitTimer = true;
            _isPlayerStop = false;
            _playerSkill.EnablePlayerSkillsAfterBossIntro(); // ī�޶� ��鸲 ������ ��ų ����
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