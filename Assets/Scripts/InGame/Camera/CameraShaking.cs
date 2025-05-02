using UnityEngine;

public class CameraShaking : MonoBehaviour
{
    private float shakeMagnitude = 0.1f; // 흔들림 강도
    private float shakeDuration = 1f;  // 흔들림 지속 시간
    private float shakeInterval = 0.5f; // 흔들림 발생 간격 (초)
    private float maxShakeInterval = 1f; // 간격 최대값

    private Vector3 originalPos;  // 원래 위치
    private float currentShakeTime = 0f; // 현재 흔들림 시간
    private float timeSinceLastShake = 0f; // 마지막 흔들림 후 경과 시간
    private bool isShaking = false;

    private void Update()
    {
        if (isShaking)
        {
            timeSinceLastShake += Time.deltaTime;

            // 흔들림이 발생할 때마다
            if (timeSinceLastShake >= shakeInterval)
            {
                // 랜덤한 방향으로 흔들림
                Vector3 shakeOffset = new Vector3(
                    Random.Range(-shakeMagnitude, shakeMagnitude),
                    0,
                    Random.Range(-shakeMagnitude, shakeMagnitude));

                transform.localPosition = originalPos + shakeOffset;

                // 마지막 흔들림 후 시간 초기화
                timeSinceLastShake = 0.0f;

                // 흔들림 간격을 랜덤하게 조금씩 변화시켜서 불규칙적인 느낌을 줌
                shakeInterval = Random.Range(0.2f, maxShakeInterval);
            }

            // 흔들림이 끝나면 원래 위치로 복귀
            currentShakeTime += Time.deltaTime;
            if (currentShakeTime >= shakeDuration)
            {
                isShaking = false;
                transform.localPosition = originalPos;
            }
        }
    }

    // 흔들림 시작 메서드
    public void StartShake(float duration, Vector3 pos)
    {
        originalPos = pos;
        shakeDuration = duration;
        isShaking = true;
        currentShakeTime = 0f;
        timeSinceLastShake = 0f;
    }
}
