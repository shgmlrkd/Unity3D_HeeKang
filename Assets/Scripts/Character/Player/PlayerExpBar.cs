using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerExpBar : Player
{
    private Slider _playerExpBarSlider;
    private Transform[] _playerExpBar;
    private TextMeshProUGUI _playerExpBarText;

    private float _toPercent = 100.0f;

    private enum ExpBar
    {
        PlayerExpBar, PlayerExpBarText = 4
    }

    private void Start()
    {
        base.Start();
        _playerExpBar = GameObject.Find("PlayerExpBar").GetComponentsInChildren<Transform>();
       _playerExpBarSlider = _playerExpBar[(int)ExpBar.PlayerExpBar].GetComponent<Slider>();
       _playerExpBarText = _playerExpBar[(int)ExpBar.PlayerExpBarText].GetComponent<TextMeshProUGUI>();

    }

    private void Update()
    {
        UpdatePlayerExpBarUI();
    }

    private void UpdatePlayerExpBarUI()
    {
        // 경험치바에 현재 경험치 비율 표시
        if (_playerExpBarSlider != null)
        {
            _playerExpBarSlider.value = _curExp / _maxExp;
        }
        // 텍스트로 경험치 비율 보여주기
        if (_playerExpBarText != null)
        {
            _playerExpBarText.text = $"{(_curExp / _maxExp * _toPercent).ToString("F2") + " %"}";
        }
    }

    public void SetPlayerCurExp(float exp)
    {
        _curExp += exp;
    }
}