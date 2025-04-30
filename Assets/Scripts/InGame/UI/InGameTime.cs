using System.Collections;
using TMPro;
using UnityEngine;

public class InGameTime : MonoBehaviour
{
    private TextMeshProUGUI _timerText;

    private readonly float _oneMinute = 60.0f;

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
        StartCoroutine(UpdateTimerCoroutine());
    }

    private void Update()
    {
        _minute = Mathf.FloorToInt(_inGameTimer / _oneMinute);
        _second = Mathf.FloorToInt(_inGameTimer % _oneMinute);

        _timerText.text = $"{_minute.ToString("00") + " : " + _second.ToString("00")}";
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