using UnityEngine;

public class Item : MonoBehaviour
{
    protected ItemData _itemData;
    protected Vector3 _rotationY;

    protected float _rotationSpeed = 100.0f; // ����ġ ������ �� ȸ�� �ӵ�
    protected int _itemKey; 
    protected int _itemValue;

    protected void Awake()
    {
        _rotationY = Vector3.up;
    }

    protected void Start()
    {
        _itemData = ItemDataManager.Instance.GetItemData(_itemKey);
    }

    protected virtual void Update()
    {
        // ��ų ���� â ������ ����
        if (Time.timeScale == 0) return;

        transform.Rotate(_rotationY * _rotationSpeed * Time.deltaTime);
    }

    public virtual void SetItemRandomValue(int value, Vector3 pos)
    {
        _itemValue = value;
        transform.position = pos;
    }
}