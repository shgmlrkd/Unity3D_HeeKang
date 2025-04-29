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

        // �ڼ� �������� �Ծ��ٸ�
        if (_isCollision)
        {
            // �ڼ� ��ƼŬ�� ��ġ�� �÷��̾� ��ġ��
            _magnetParticle.transform.position = _player.position;
            // ��� Exp�� �Ծ����� Ȯ���ϴ� �뵵
            bool allExpInactive = true;
            // �������� Exp ������ �̵�
            foreach (Transform exp in _onEnableExps)
            {
                if (exp != null && exp.gameObject.activeInHierarchy)
                {
                    // ���� �̵�
                    exp.position = Vector3.Lerp(exp.position, _player.position, _itemData.LerpTime * Time.deltaTime);
                    allExpInactive = false;
                }
            }

            // ��� ����ġ�� �Ծ��� �� ��Ȱ��ȭ
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
            //�÷��̾� ��ġ �ޱ�
            _player = other.gameObject.GetComponent<Transform>();
            // Ȱ��ȭ�� ����ġ���� ����
            _onEnableExps = ItemManager.Instance.GetEnabledExpList();
            // �ڼ� �� ��Ȱ��ȭ
            _magnetChildren[(int)MagnetObject.MagnetModel].gameObject.SetActive(false);
            // �ڼ��� ������ ��ƼŬ �÷���
            _magnetParticle.Play();

            InGameUIManager.Instance.SetGoldCountText();
        }
    }
}