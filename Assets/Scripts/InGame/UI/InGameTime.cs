using System.Collections;
using TMPro;
using UnityEngine;

public class InGameTime : MonoBehaviour
{
    private TextMeshProUGUI _timerText;
    private CameraShaking _camShake;
    private Coroutine _timerCoroutine;

    private readonly float _oneMinute = 60.0f;

    private float _inGameTimerInitTime;
    private float _minute;
    private float _second;
    private float _inGameTimer;
    public float InGameTimer
    {
        get { return _inGameTimer; }
    }

    private void Awake()
    {
        _minute = 0.0f;
        _second = 0.0f;
        _inGameTimer = 0.0f;
    }

    private void Start()
    {
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

        if (_inGameTimer >= 5 && _timerCoroutine != null)
        {
            _inGameTimer = 0.0f;
            StopCoroutine(_timerCoroutine);
            _timerCoroutine = null; // �ڷ�ƾ�� �������Ƿ� null�� ����

            // �÷��̾��� ���� ��ǥ ���� ���� ī�޶��� ���� ��ǥ�� x,z�� ������Ѵ�.
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(InGameManager.Instance.Player.transform.position);

            // ī�޶� ��鸲�� ����
            _camShake.StartShake(1.5f, screenPosition);
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