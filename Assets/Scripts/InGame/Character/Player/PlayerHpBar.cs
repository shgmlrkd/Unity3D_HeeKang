using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHpBar : MonoBehaviour
{
    private ParticleSystem[] _healParticles;
    private PlayerStatus _player;
    private Slider _playerHpBarSlider;
    private Transform[] _playerHpBar;
    private TextMeshProUGUI _playerHpBarText;

    private float _curHp = 0.0f;

    private enum HpBar
    { 
        PlayerHpBar, PlayerHpBarText = 4
    }

    private void Start()
    {
        _healParticles = GameObject.Find("Healing").GetComponentsInChildren<ParticleSystem>();  

        _player = GetComponent<PlayerStatus>();
        _curHp = _player.Status.MaxHp;
        _playerHpBar = InGameUIManager.Instance.GetPlayerHpBarUI();
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
            _playerHpBarSlider.value = _curHp / _player.Status.MaxHp;
        }
        // 텍스트로 남은 체력 보여주기
        if (_playerHpBarText != null)
        {
            _playerHpBarText.text = $"{_curHp.ToString("F0") + " / " + _player.Status.MaxHp.ToString()}";
        }
    }

    public void PlayerGetDamage(float damage)
    {
        _curHp -= damage;

        if (_curHp <= 0.0f)
        {
            _curHp = 0.0f;
        }
    }
    
    public void PlayerGetHeal(float  heal)
    {
        _curHp += heal;

        if(_curHp >= _player.Status.MaxHp)
        {
            _curHp = _player.Status.MaxHp;
        }

        foreach(ParticleSystem healParticle in _healParticles)
        {
            if (healParticle.isPlaying)
            {
                // 파티클을 먼저 정지하고 초기화
                healParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }

            if (!healParticle.isPlaying)
            {
                healParticle.Play();
            }
        }
    }
}