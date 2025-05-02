using UnityEngine;

public class CameraShaking : MonoBehaviour
{
    private Vector3 _originalPos;

    private readonly float _perlinOffset = 0.5f; // 중앙 정렬용 (PerlinNoise 값은 0~1 사이, 0.5를 빼서 -0.5~0.5 범위로 변환)
    private readonly float _perlinScale = 2.0f; // -0.5~0.5 범위를 -1~1로 확장

    private float _magnitude = 0.4f; // 흔들림 강도
    private float _frequency = 25.0f; // 흔들림 변화 속도 (값이 클수록 더 잦게 변함)
    private float _shakeTimer = 0.0f; // 흔들림 지속 시간
    private float _shakeElapsed = 0.0f; // 현재 흔들린 시간 누적

    private bool _isShakeEnd = false;
    public bool IsShakeEnd
    {
        get { return _isShakeEnd; }
    }

    private void Update()
    {
        // 카메라 흔들기를 시작하면
        if (_shakeTimer > 0.0f)
        {
            // 카메라는 플레이어를 따라다니고 있으므로
            // 계속 자기 자신의 위치를 originalPos로 저장
            _originalPos = transform.position;

            _shakeElapsed += Time.deltaTime;

            // 흔드는 시간의 흐름에 따라 점점 줄어들게 뎀핑값을 줌
            float damper = 1.0f - Mathf.Clamp01(_shakeElapsed / _shakeTimer); // 감쇠 (0 ~ 1)

            // PerlinNoise를 사용해서 부드러운 랜덤 값을 받음
            // 흔들림의 X축과 Z축 오프셋을 각각 다르게 계산하여 좀 더 자연스러운 흔들림을 생성
            float offsetX = (Mathf.PerlinNoise(Time.time * _frequency, 0.0f) - _perlinOffset) * _perlinScale * _magnitude * damper;
            float offsetZ = (Mathf.PerlinNoise(0.0f, Time.time * _frequency) - _perlinOffset) * _perlinScale * _magnitude * damper;
            
            // 원래 위치에 오프셋을 더해 새로운 위치로 이동
            transform.position = _originalPos + new Vector3(offsetX, 0.0f, offsetZ);

            // 시간 끝나면
            if (_shakeElapsed >= _shakeTimer)
            {
                _isShakeEnd = true;
                _shakeTimer = 0.0f;
                _shakeElapsed = 0.0f;
                transform.localPosition = _originalPos; // 위치를 원래대로 복귀
            }
        }
    }

    public void StartShake(float duration)
    {
        _shakeTimer = duration;
        _shakeElapsed = 0.0f;
    }
}
