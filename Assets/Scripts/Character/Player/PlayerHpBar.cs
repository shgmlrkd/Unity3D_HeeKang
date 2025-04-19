using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHpBar : Player
{
    private Slider _playerHpBarSlider;
    private Transform[] _playerHpBar;
    private TextMeshProUGUI _playerHpBarText;

    private enum HpBar
    { 
        PlayerHpBar, PlayerHpBarText = 4
    }

    protected override void Start()
    {
        base.Start();
        _playerHpBar = GameObject.Find("PlayerHpBar").GetComponentsInChildren<Transform>();
        _playerHpBarSlider = _playerHpBar[(int)HpBar.PlayerHpBar].GetComponent<Slider>();
        _playerHpBarText = _playerHpBar[(int)HpBar.PlayerHpBarText].GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        UpdatePlayerHpBarUI();
    }

    private void UpdatePlayerHpBarUI()
    {
        // 체력바에 현재 남은 Hp 표시
        if (_playerHpBarSlider != null)
        {
            _playerHpBarSlider.value = _curHp / _maxHp;
        }
        // 텍스트로 남은 체력 보여주기
        if (_playerHpBarText != null)
        {
            if(_curHp < 0)
            {
                _curHp = 0;
            }

            _playerHpBarText.text = $"{_curHp.ToString() + " / " + _maxHp.ToString()}";
        }
    }

    public void SetPlayerCurHp(float damage)
    {
        _curHp -= damage;
    }
}