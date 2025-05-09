using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerExpBar : MonoBehaviour
{
    private PlayerStatus _player;
    private Slider _playerExpBarSlider;
    private Transform[] _playerExpBar;
    private TextMeshProUGUI _playerExpBarText;
    private TextMeshProUGUI _playerLevelText;

    private readonly float _toPercent = 100.0f;
    private float _curExp = 0.0f;

    private enum ExpBar
    {
        PlayerExpBar, PlayerExpBarText = 4, PlayerLevelText
    }

    private void Start()
    {
       _player = GetComponent<PlayerStatus>();
       _playerExpBar = InGameUIManager.Instance.GetPlayerExpBarUI();
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
        // 경험치바에 현재 경험치 비율 표시
        if (_playerExpBarSlider != null)
        {
            _playerExpBarSlider.value = _curExp / _player.MaxExp;
        }
        // 텍스트로 경험치 비율 보여주기
        if (_playerExpBarText != null)
        {
            _playerExpBarText.text = $"{(_curExp / _player.MaxExp * _toPercent).ToString("F2") + " %"}";
        }
        // 텍스트로 인게임 레벨 보여주기
        if( _playerLevelText != null)
        {
            _playerLevelText.text = $"{"Lv " + (_player.ExpLevel + 1).ToString()}";
        }
    }

    // 경험치를 개당 20으로 했을 경우 1렙때
    // 레벨업이 2번 되는데 스킬 선택 버튼은 1번 실행되는 버그 있음
    public void SetPlayerCurExp(float exp)
    {
        if (_player.ExpLevel > 19)
        {
            _curExp = _player.MaxExp;
            return;
        }

        _curExp += exp;

        if(_curExp >= _player.MaxExp)
        {
            print(_player.MaxExp);
            _curExp -= _player.MaxExp;
            _player.LevelUp();
        }
    }
}