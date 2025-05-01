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
        // ���ڴ� �ȵ��� ������ �־�� �ϱ� ������ �����
    }

    public override void SetItemRandomValue(int value, Vector3 pos)
    {
        Vector3 position = pos;
        position.y = 0.0f;

        _itemValue = value;
        transform.position = position;
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