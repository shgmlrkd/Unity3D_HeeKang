using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHpBar : MonoBehaviour
{
    private Slider _playerHpBarSlider;
    private Transform[] _playerHpBar;
    private TextMeshProUGUI _playerHpBarText;

    private float _maxHp;
    private float _curHp;

    private enum HpBar
    { 
        PlayerHpBar, PlayerHpBarText = 4
    }

    void Start()
    {
        _playerHpBar = GameObject.Find("PlayerHpBar").GetComponentsInChildren<Transform>();
        _playerHpBarSlider = _playerHpBar[(int)HpBar.PlayerHpBar].GetComponent<Slider>();
        _playerHpBarText = _playerHpBar[(int)HpBar.PlayerHpBarText].GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        
    }
}
