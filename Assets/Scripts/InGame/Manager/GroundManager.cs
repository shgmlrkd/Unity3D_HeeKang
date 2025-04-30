using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GroundManager : MonoBehaviour
{
    private List<GameObject> _grounds;
    private GameObject _groundPrefab;
    private GameObject _groundObject;
    private Vector3[] _groundPositions;

    private void Awake()
    {
        _grounds = new List<GameObject>();
        _groundPrefab = Resources.Load<GameObject>("Prefabs/Ground");

        _groundPositions = new Vector3[4];
        // ������ ��
        _groundPositions[0] = new Vector3(40.0f, 0.0f, 40.0f);
        // ������ �Ʒ�
        _groundPositions[1] = new Vector3(40.0f, 0.0f, -40.0f);
        // ���� ��
        _groundPositions[2] = new Vector3(-40.0f, 0.0f, 40.0f);
        // ���� �Ʒ�
        _groundPositions[3] = new Vector3(-40.0f, 0.0f, -40.0f);
    }

    void Start()
    {
        _groundObject = GameObject.Find("GroundObjects");

        foreach (Vector3 groundPos in _groundPositions)
        {
            GameObject ground = Instantiate(_groundPrefab, _groundObject.transform);
            ground.transform.position = groundPos;
            _grounds.Add(ground);
        }
    }
}
