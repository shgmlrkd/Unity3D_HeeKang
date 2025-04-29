using System.Collections.Generic;
using UnityEngine;

public class ItemManager : Singleton<ItemManager>
{
    private readonly Vector3 _posOffsetY = new Vector3(0.0f, 0.5f, 0.0f);

    private readonly int _itemStartKey = 201;
    private readonly int _oneHundred = 100;

    private int _itemCount;

    private bool _isMagnetOn = false;
    public bool IsMagnetOn
    {
        get { return _isMagnetOn; }
        set { _isMagnetOn = value; }
    }

    private void Start()
    {
        _itemCount = ItemDataManager.Instance.GetItemsCount();    
    }

    public void CreateItems(int poolSize, string key)
    {
        GameObject weaponPrefab = Resources.Load<GameObject>("Prefabs/Items/" + key);
        PoolingManager.Instance.Add(key, poolSize, weaponPrefab, transform);
    }

    // Ȱ��ȭ�Ǿ� �ִ� ����ġ�� ��ġ�� ��ȯ
    public List<Transform> GetEnabledExpList()
    {
        List<Transform> onEnableObjs = new List<Transform>();
        List<GameObject> objs = PoolingManager.Instance.GetObjects("Exp");

        foreach (GameObject obj in objs)
        {
            if(obj.activeSelf)
                onEnableObjs.Add(obj.transform);
        }

        if(onEnableObjs.Count > 0)
        {
            _isMagnetOn = true;
        }
        else
        {
            _isMagnetOn = false;
        }

        return onEnableObjs;
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
        ItemData data = ItemDataManager.Instance.GetItemData(randomKey);
        // �� data�� DropRate�� �°� �̱����� ������ (0 ~ 99)
        int randomRate = Random.Range(0, _oneHundred);
        // DropRate�� randomRate �̻��̸� ȣ��
        if(data.DropRate >= randomRate)
        {
            // ���� ������ ������ ���� ����
            int randomValue = Random.Range(data.MinValue, data.MaxValue);
            GameObject obj = PoolingManager.Instance.Pop(data.Name);
            obj.GetComponent<Item>().SetItemRandomValue(randomValue, pos + _posOffsetY);
        }
    }
}