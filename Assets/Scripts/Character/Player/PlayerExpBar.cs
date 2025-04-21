using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerExpBar : Player
{
    private Slider _playerExpBarSlider;
    private Transform[] _playerExpBar;
    private TextMeshProUGUI _playerExpBarText;
    private TextMeshProUGUI _playerLevelText;

    private float _toPercent = 100.0f;

    private enum ExpBar
    {
        PlayerExpBar, PlayerExpBarText = 4, PlayerLevelText
    }

    private void Start()
    {
        base.Start();
        _playerExpBar = GameObject.Find("PlayerExpBar").GetComponentsInChildren<Transform>();
       _playerExpBarSlider = _playerExpBar[(int)ExpBar.PlayerExpBar].GetComponent<Slider>();
       _playerExpBarText = _playerExpBar[(int)ExpBar.PlayerExpBarText].GetComponent<TextMeshProUGUI>();
       _playerLevelText = _playerExpBar[(int)ExpBar.PlayerLevelText].GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        UpdatePlayerExpBarUI();
    }

    private void UpdatePlayerExpBarUI()
    {
        // ����ġ�ٿ� ���� ����ġ ���� ǥ��
        if (_playerExpBarSlider != null)
        {
            _playerExpBarSlider.value = _curExp / _maxExp;
        }
        // �ؽ�Ʈ�� ����ġ ���� �����ֱ�
        if (_playerExpBarText != null)
        {
            _playerExpBarText.text = $"{(_curExp / _maxExp * _toPercent).ToString("F2") + " %"}";
        }
        // �ؽ�Ʈ�� �ΰ��� ���� �����ֱ�
        if( _playerLevelText != null)
        {
            _playerLevelText.text = $"{"Lv " + _expLevel}";
        }
    }

    public void SetPlayerCurExp(float exp)
    {
        _curExp += exp;

        if(_curExp >= _maxExp)
        {
            _curExp %= _maxExp;
            LevelUp();
        }
    }
}