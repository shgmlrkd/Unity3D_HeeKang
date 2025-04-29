using UnityEngine;

public class ItemManager : Singleton<ItemManager>
{
    private readonly int _itemStartKey = 201;
    private readonly int _oneHundred = 100;

    private int _itemCount;

    private void Start()
    {
        _itemCount = ItemDataManager.Instance.GetItemsCount();    
    }

    public void CreateItems(int poolSize, string key)
    {
        GameObject weaponPrefab = Resources.Load<GameObject>("Prefabs/Items/" + key);
        PoolingManager.Instance.Add(key, poolSize, weaponPrefab, transform);
    }

    public void SpawnExp(float expValue, Vector3 pos)
    {
        GameObject exp = PoolingManager.Instance.Pop("Exp");
        exp.GetComponent<Exp>().SetExp(expValue, pos);
    }

    public void SpawnRandomItem(Vector3 pos)
    {
        // ������ ���� �� �������� �̱�
        int randomKey = _itemStartKey + Random.Range(0, _itemCount);
        // �������� ���� Ű������ data ��������
        ItemData data = ItemDataManager.Instance.GetItemData(203);
        // �� data�� DropRate�� �°� �̱����� ������ (0 ~ 99)
        int randomRate = Random.Range(0, _oneHundred);
        // DropRate�� randomRate �̻��̸� ȣ��
        if(data.DropRate >= randomRate)
        {
            // ���� ������ ������ ���� ����
            int randomValue = Random.Range(data.MinValue, data.MaxValue);
            GameObject obj = PoolingManager.Instance.Pop(data.Name);
            obj.GetComponent<Item>().SetItemRandomValue(randomValue , pos);
        }
    }
}