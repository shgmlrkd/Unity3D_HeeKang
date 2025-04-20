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
        // MeshFilter 컴포넌트를 통해 현재 GameObject의 Mesh를 가져옴
        _mesh = GetComponent<MeshFilter>().sharedMesh;

        // 메시의 로컬 크기를 가져옴 (메시 자체의 크기)
        Vector3 meshSize = _mesh.bounds.size;
        // 오브젝트의 월드 스케일을 가져옴 (컴포넌트의 실제 크기 비율)
        Vector3 worldScale = transform.lossyScale;
        // 실제 월드 상에서의 크기를 계산
        // 메시의 로컬 크기와 오브젝트의 월드 스케일을 곱해 최종 크기 계산 (월드 상의 실제 사이즈)
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
        // 카메라 범위 밖으로 나갔다면
        if (IsPlayerOutsideGround())
        {
            // 반대로 땅 움직이기
            MoveGround();
        }
    }

    private bool IsPlayerOutsideGround()
    {
        Vector3 groundPos = transform.position;
        Vector3 playerPos = _playerPos.position;

        // X, Z 방향으로 거리 계산
        bool outsideX = Mathf.Abs(playerPos.x - groundPos.x) > _groundSize.x;
        bool outsideZ = Mathf.Abs(playerPos.z - groundPos.z) > _groundSize.z;

        return outsideX || outsideZ;
    }

    private void MoveGround()
    {
        // 땅의 위치에서 플레이어 위치로 향하는 방향벡터
        Vector3 dir = (_playerPos.position - transform.position).normalized;
        dir.y = 0.0f;

        // x축으로 많이 갔는지 z축으로 많이 갔는지 체크
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.z))
        {
            // 양수인지 음수인지 체크 해서 방향 구하기
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
            // 양수인지 음수인지 체크 해서 방향 구하기
            if (dir.z > 0.0f)
            {
                _groundDir = Vector3.forward;
            }
            else if (dir.z < 0.0f)
            {
                _groundDir = Vector3.back;
            }
        }

        // 원래 사이즈의 2배 이동해야 반대 방향으로 감
        _groundMoveOffset = Vector3.Scale(_groundDir, _groundSize) * _movementScale;
        transform.position += _groundMoveOffset;
    }
}
