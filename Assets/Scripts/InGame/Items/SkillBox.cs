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
        // 상자는 안돌고 가만히 있어야 하기 때문에 비워둠
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
            // 스킬 패널 열기
            InGameUIManager.Instance.SkillPanelOn();
            // 시간 멈춤
            Time.timeScale = 0;
        }
    }
}