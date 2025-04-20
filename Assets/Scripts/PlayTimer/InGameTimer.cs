using TMPro;
using UnityEngine;

public class InGameTimer : MonoBehaviour
{
    private TextMeshProUGUI _timerText;

    private readonly float _oneMinute = 60.0f;

    private float _minute;
    private float _second;
    private float _timer;
    public float Timer
    {
        get { return _timer; }
    }

    private void Awake()
    {
        _minute = 0.0f;
        _second = 0.0f;
        _timer = 0.0f;
    }

    private void Start()
    {
        _timerText = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        _timer += Time.deltaTime;

        _minute = Mathf.FloorToInt(_timer / _oneMinute);
        _second = Mathf.FloorToInt(_timer % _oneMinute);

        _timerText.text = $"{_minute.ToString("00") + " : " + _second.ToString("00")}";
    }
}