using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerExpBar : Player
{
    private Slider _playerExpBarSlider;
    private Transform[] _playerExpBar;
    private TextMeshProUGUI _playerExpBarText;

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

    void Update()
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
            _playerExpBarText.text = $"{(_curExp / _maxExp).ToString("F2") + " %"}";
        }
    }
}