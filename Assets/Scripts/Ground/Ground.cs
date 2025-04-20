using UnityEngine;

public class Ground : MonoBehaviour
{
    private Transform _playerPos;
    private Mesh _mesh;

    private Vector3 _groundDir;
    private Vector3 _groundSize;
    public Vector3 GroundSize
    {
        get { return _groundSize; }
    }

    private Vector3 _groundMoveOffset;

    private float _offset = 0.5f;
    private int _movementScale = 2;

    void Start()
    {
        // MeshFilter ������Ʈ�� ���� ���� GameObject�� Mesh�� ������
        _mesh = GetComponent<MeshFilter>().sharedMesh;

        // �޽��� ���� ũ�⸦ ������ (�޽� ��ü�� ũ��)
        Vector3 meshSize = _mesh.bounds.size;
        // ������Ʈ�� ���� �������� ������ (������Ʈ�� ���� ũ�� ����)
        Vector3 worldScale = transform.lossyScale;
        // ���� ���� �󿡼��� ũ�⸦ ���
        // �޽��� ���� ũ��� ������Ʈ�� ���� �������� ���� ���� ũ�� ��� (���� ���� ���� ������)
        _groundSize = new Vector3
            (
                meshSize.x * worldScale.x,
                meshSize.y * worldScale.y,
                meshSize.z * worldScale.z
            );

        _playerPos = GameObject.FindWithTag("Player").GetComponent<Transform>();
    }
 
    void Update()
    {
        // ī�޶� ���� ������ �����ٸ�
        if (IsPlayerOutsideGround())
        {
            // �ݴ�� �� �����̱�
            MoveGround();
        }
    }

    private bool IsPlayerOutsideGround()
    {
        Vector3 groundPos = transform.position;
        Vector3 playerPos = _playerPos.position;

        // X, Z �������� �Ÿ� ���
        bool outsideX = Mathf.Abs(playerPos.x - groundPos.x) > _groundSize.x;
        bool outsideZ = Mathf.Abs(playerPos.z - groundPos.z) > _groundSize.z;

        return outsideX || outsideZ;
    }

    private void MoveGround()
    {
        // ���� ��ġ���� �÷��̾� ��ġ�� ���ϴ� ���⺤��
        Vector3 dir = (_playerPos.position - transform.position).normalized;
        dir.y = 0.0f;

        // x������ ���� ������ z������ ���� ������ üũ
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.z))
        {
            // ������� �������� üũ �ؼ� ���� ���ϱ�
            if (dir.x > 0.0f)
            {
                _groundDir = Vector3.right;
            }
            else if (dir.x < 0.0f)
            {
                _groundDir = Vector3.left;
            }
        }
        else if (Mathf.Abs(dir.x) < Mathf.Abs(dir.z))
        {
            // ������� �������� üũ �ؼ� ���� ���ϱ�
            if (dir.z > 0.0f)
            {
                _groundDir = Vector3.forward;
            }
            else if (dir.z < 0.0f)
            {
                _groundDir = Vector3.back;
            }
        }

        // ���� �������� 2�� �̵��ؾ� �ݴ� �������� ��
        _groundMoveOffset = Vector3.Scale(_groundDir, _groundSize) * _movementScale;
        transform.position += _groundMoveOffset;
    }
}
