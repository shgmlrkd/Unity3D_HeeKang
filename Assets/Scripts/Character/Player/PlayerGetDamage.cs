using UnityEngine;

public class PlayerGetDamage : Player
{
    private PlayerHpBar _playerHpBar;

    protected new void Start()
    {
        _playerHpBar = GetComponent<PlayerHpBar>();
    }

    public void GetDamage(float damage)
    {
        _playerHpBar.SetPlayerCurHp(damage);
    }
}
