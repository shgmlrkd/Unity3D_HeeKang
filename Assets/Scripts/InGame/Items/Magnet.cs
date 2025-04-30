using UnityEngine;
using System.Collections.Generic;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Magnet : Item
{
    private enum MagnetObject
    {
        MagnetModel = 1, MagnetParticle
    }
    private Transform _player;
    private Transform[] _magnetChildren;
    private List<Transform> _onEnableExps = new List<Transform>();
    private ParticleSystem _magnetParticle;

    private bool _isCollision = false;

    private void OnEnable()
    {
        _isCollision = false;

        if (_magnetChildren != null)
        {
            _magnetChildren[(int)MagnetObject.MagnetModel].gameObject.SetActive(true);
        }
    }

    private void Start()
    {
        _itemKey = 204;
        base.Start();

        _magnetChildren = GetComponentsInChildren<Transform>();
        _magnetParticle = GetComponentInChildren<ParticleSystem>();
    }

    protected override void Update()
    {
        base.Update();

        // 자석 아이템을 먹었다면
        if (_isCollision)
        {
            // 자석 파티클의 위치는 플레이어 위치로
            _magnetParticle.transform.position = _player.position;
            // 모든 Exp를 먹었는지 확인하는 용도
            bool allExpInactive = true;
            // 보간으로 Exp 아이템 이동
            foreach (Transform exp in _onEnableExps)
            {
                if (exp != null && exp.gameObject.activeInHierarchy)
                {
                    // 보간 이동
                    exp.position = Vector3.Lerp(exp.position, _player.position, _itemData.LerpTime * Time.deltaTime);
                    allExpInactive = false;
                }
            }

            // 모든 경험치를 먹었을 때 비활성화
            if (allExpInactive)
            {
                gameObject.SetActive(false);
            }

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !_isCollision)
        {
            _isCollision = true;
            //플레이어 위치 받기
            _player = other.gameObject.GetComponent<Transform>();
            // 활성화된 경험치들을 받음
            _onEnableExps = ItemManager.Instance.GetEnabledExpList();
            // 자석 모델 비활성화
            _magnetChildren[(int)MagnetObject.MagnetModel].gameObject.SetActive(false);
            // 자석을 먹으면 파티클 플레이
            _magnetParticle.Play();

            InGameUIManager.Instance.SetGoldCountText();
        }
    }
}