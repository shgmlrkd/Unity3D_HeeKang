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
            // ��ų �г� ����
            InGameUIManager.Instance.SkillPanelOn();
            // �ð� ����
            Time.timeScale = 0;
        }
    }
}