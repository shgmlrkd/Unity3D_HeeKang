using System.Collections.Generic;
using UnityEngine;

public class PoolingManager : Singleton<PoolingManager>
{

    private Dictionary<string, List<GameObject>> _totalObject = new Dictionary<string, List<GameObject>>();

    public List<GameObject> GetObjects(string key) { return _totalObject[key]; }

    public void Add(string key, int poolSize, GameObject prefab, Transform parent = null)
    {
        // 오브젝트를 넣을 리스트
        List<GameObject> objects = new List<GameObject>(poolSize);
        // 부모가 없다면 key를 이름으로 만듦
        if (parent == null)
        {
             GameObject parentObject = new GameObject(key);
            parent = parentObject.transform;
        }
        // poolSize만큼 생성 후 리스트에 담음
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(prefab, parent);
            obj.name = key + "_" + i;
            obj.SetActive(false);
            objects.Add(obj);
        }
        // 딕셔너리에 이름을 키값으로 리스트를 넣음
        _totalObject.Add(key, objects);
    }

    public GameObject Pop(string key)
    {
        foreach (GameObject obj in _totalObject[key])
        {
            // 비활성화인 오브젝트면 활성화 후 리턴
            if (!obj.activeSelf)
            {
                obj.SetActive(true);
                return obj;
            }
        }

        return null;
    }
}
