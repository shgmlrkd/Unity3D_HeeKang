using UnityEngine;

public class SkillBox : Item
{
    private void Start()
    {
        _itemKey = 202;
        base.Start();
    }

    protected override void Update()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameObject.SetActive(false);
            // Ω∫≈≥ ∆–≥Œ ø≠±‚
            InGameUIManager.Instance.SkillPanelOn();
            // Ω√∞£ ∏ÿ√„
            Time.timeScale = 0;
        }
    }
}