using UnityEngine;

public class CameraShaking : MonoBehaviour
{
    private Vector3 _originalPos;

    private readonly float _perlinOffset = 0.5f; // �߾� ���Ŀ� (PerlinNoise ���� 0~1 ����, 0.5�� ���� -0.5~0.5 ������ ��ȯ)
    private readonly float _perlinScale = 2.0f; // -0.5~0.5 ������ -1~1�� Ȯ��

    private float _magnitude = 0.4f; // ��鸲 ����
    private float _frequency = 25.0f; // ��鸲 ��ȭ �ӵ� (���� Ŭ���� �� ��� ����)
    private float _shakeTimer = 0.0f; // ��鸲 ���� �ð�
    private float _shakeElapsed = 0.0f; // ���� ��鸰 �ð� ����

    private bool _isShakeEnd = false;
    public bool IsShakeEnd
    {
        get { return _isShakeEnd; }
    }

    private void Update()
    {
        // ī�޶� ���⸦ �����ϸ�
        if (_shakeTimer > 0.0f)
        {
            // ī�޶�� �÷��̾ ����ٴϰ� �����Ƿ�
            // ��� �ڱ� �ڽ��� ��ġ�� originalPos�� ����
            _originalPos = transform.position;

            _shakeElapsed += Time.deltaTime;

            // ���� �ð��� �帧�� ���� ���� �پ��� ���ΰ��� ��
            float damper = 1.0f - Mathf.Clamp01(_shakeElapsed / _shakeTimer); // ���� (0 ~ 1)

            // PerlinNoise�� ����ؼ� �ε巯�� ���� ���� ����
            // ��鸲�� X��� Z�� �������� ���� �ٸ��� ����Ͽ� �� �� �ڿ������� ��鸲�� ����
            float offsetX = (Mathf.PerlinNoise(Time.time * _frequency, 0.0f) - _perlinOffset) * _perlinScale * _magnitude * damper;
            float offsetZ = (Mathf.PerlinNoise(0.0f, Time.time * _frequency) - _perlinOffset) * _perlinScale * _magnitude * damper;
            
            // ���� ��ġ�� �������� ���� ���ο� ��ġ�� �̵�
            transform.position = _originalPos + new Vector3(offsetX, 0.0f, offsetZ);

            // �ð� ������
            if (_shakeElapsed >= _shakeTimer)
            {
                _isShakeEnd = true;
                _shakeTimer = 0.0f;
                _shakeElapsed = 0.0f;
                transform.localPosition = _originalPos; // ��ġ�� ������� ����
            }
        }
    }

    public void StartShake(float duration)
    {
        _shakeTimer = duration;
        _shakeElapsed = 0.0f;
    }
}
