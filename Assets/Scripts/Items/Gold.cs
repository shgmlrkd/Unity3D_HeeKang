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

        // �浹 �� ��ƼŬ�� ������ ��Ȱ��ȭ
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
            // ��� �� ��Ȱ��ȭ
            _goldChildren[(int)GoldObject.GoldModel].gameObject.SetActive(false);
            // ��带 ������ ��ƼŬ �÷���
            _goldParticle.Play();

            InGameUIManager.Instance.SetGoldCountText();
        }
    }
}