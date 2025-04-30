using UnityEngine;

public class Slime : FlashDamagedMonster
{
    private int _slimeKey = 101;

    private void Awake()
    {
        base.Awake();

        _flashColor = Color.white;
    }

    private void OnEnable()
    {
        SetMonsterKey(_slimeKey);

        base.OnEnable();
    }
}