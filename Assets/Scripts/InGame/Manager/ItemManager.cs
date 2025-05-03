using System.Collections.Generic;
using UnityEngine;

public class ItemManager : Singleton<ItemManager>
{
    private readonly Vector3 _posOffsetY = new Vector3(0.0f, 0.5f, 0.0f);

    private readonly float _expInactiveTime = 300.0f;

    private readonly int _itemStartKey = 201;
    private readonly int _oneHundred = 100;

    private float _inGameTime;
    private int _itemCount;

    private bool _isDeadTime = false;
    private bool _isMagnetOn = false;
    public bool IsMagnetOn
    {
        get { return _isMagnetOn; }
    }

    
    private void Start()
    {
        _itemCount = ItemDataManager.Instance.GetItemsCount();    
    }
    private void Update()
    {
        // 인게임 시간 받아오기
        _inGameTime = InGameUIManager.Instance.GetInGameTimer();

        if (_inGameTime >= _expInactiveTime && !_isDeadTime)
        {
            _isDeadTime = true;
            AllExpInactive();
        }
    }

    private void AllExpInactive()
    {
        List<GameObject> exps = PoolingManager.Instance.GetObjects("Exp");
        
        foreach (GameObject exp in exps)
        {
            if(exp.activeSelf)
            {
                exp.SetActive(false);
            }
        }
    }

    public void CreateItems(int poolSize, string key)
    {
        GameObject weaponPrefab = Resources.Load<GameObject>("Prefabs/Items/" + key);
        PoolingManager.Instance.Add(key, poolSize, weaponPrefab, transform);
    }

    // 활성화되어 있는 경험치의 위치만 반환
    public List<Transform> GetEnabledExpList()
    {
        List<Transform> onEnableObjs = new List<Transform>();
        List<GameObject> objs = PoolingManager.Instance.GetObjects("Exp");

        foreach (GameObject obj in objs)
        {
            if(obj.activeSelf)
                onEnableObjs.Add(obj.transform);
        }

        return onEnableObjs;
    }
    public void SetMagnetState(bool state)
    {
        _isMagnetOn = state;
    }

    public void SpawnExp(float expValue, Vector3 pos)
    {
        GameObject exp = PoolingManager.Instance.Pop("Exp");
        exp.GetComponent<Exp>().SetExp(expValue, pos + _posOffsetY);
    }

    public void SpawnRandomItem(Vector3 pos)
    {
        // 아이템 개수 중 랜덤으로 뽑기
        int randomKey = _itemStartKey + Random.Range(0, _itemCount);
        // 랜덤으로 뽑은 키값으로 data 가져오기
        ItemData data = ItemDataManager.Instance.GetItemData(randomKey);
        // 그 data에 DropRate에 맞게 뽑기위한 랜덤값 (0 ~ 99)
        int randomRate = Random.Range(0, _oneHundred);
        // DropRate가 randomRate 이상이면 호출
        if(data.DropRate >= randomRate)
        {
            // 랜덤 값으로 아이템 값을 받음
            int randomValue = Random.Range(data.MinValue, data.MaxValue);
            GameObject obj = PoolingManager.Instance.Pop(data.Name);
            obj.GetComponent<Item>().SetItemRandomValue(randomValue, pos + _posOffsetY);
        }
    }
}