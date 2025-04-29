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
        // 아이템 개수 중 랜덤으로 뽑기
        int randomKey = _itemStartKey + Random.Range(0, _itemCount);
        // 랜덤으로 뽑은 키값으로 data 가져오기
        ItemData data = ItemDataManager.Instance.GetItemData(203);
        // 그 data에 DropRate에 맞게 뽑기위한 랜덤값 (0 ~ 99)
        int randomRate = Random.Range(0, _oneHundred);
        // DropRate가 randomRate 이상이면 호출
        if(data.DropRate >= randomRate)
        {
            // 랜덤 값으로 아이템 값을 받음
            int randomValue = Random.Range(data.MinValue, data.MaxValue);
            GameObject obj = PoolingManager.Instance.Pop(data.Name);
            obj.GetComponent<Item>().SetItemRandomValue(randomValue , pos);
        }
    }
}