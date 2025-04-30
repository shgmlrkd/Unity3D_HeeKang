using UnityEngine;

public class Medkit : Item
{
    private void Start()
    {
        _itemKey = 203;
        base.Start();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            gameObject.SetActive(false);
            PlayerHpBar playerHp = other.GetComponent<PlayerHpBar>();
            playerHp.PlayerGetHeal(_itemValue);
        }
    }
}
