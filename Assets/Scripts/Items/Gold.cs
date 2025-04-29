using UnityEngine;

public class Gold : Item
{
    private enum GoldObject
    {
        GoldModel = 1, GoldParticle
    }

    private Transform[] _goldChildren;
    private ParticleSystem _goldParticle;

    private bool _isCollision = false;

    private void OnEnable()
    {
        _isCollision = false;

        if (_goldChildren != null)
        { 
            _goldChildren[(int)GoldObject.GoldModel].gameObject.SetActive(true); 
        }
    }

    private void Start()
    {
        _itemKey = 201;
        base.Start();

        _goldChildren = GetComponentsInChildren<Transform>();
        _goldParticle = GetComponentInChildren<ParticleSystem>();
    }

    protected override void Update()
    {
        base.Update();

        // 충돌 후 파티클이 끝나면 비활성화
        if (_isCollision)
        {
            if (!_goldParticle.isPlaying)
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
            // 골드 모델 비활성화
            _goldChildren[(int)GoldObject.GoldModel].gameObject.SetActive(false);
            // 골드를 먹으면 파티클 플레이
            _goldParticle.Play();

            InGameUIManager.Instance.SetGoldCountText();
        }
    }
}