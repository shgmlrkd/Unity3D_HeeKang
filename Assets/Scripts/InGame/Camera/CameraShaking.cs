using UnityEngine;

public class CameraShaking : MonoBehaviour
{
    private float shakeMagnitude = 0.1f; // ��鸲 ����
    private float shakeDuration = 1f;  // ��鸲 ���� �ð�
    private float shakeInterval = 0.5f; // ��鸲 �߻� ���� (��)
    private float maxShakeInterval = 1f; // ���� �ִ밪

    private Vector3 originalPos;  // ���� ��ġ
    private float currentShakeTime = 0f; // ���� ��鸲 �ð�
    private float timeSinceLastShake = 0f; // ������ ��鸲 �� ��� �ð�
    private bool isShaking = false;

    private void Update()
    {
        if (isShaking)
        {
            timeSinceLastShake += Time.deltaTime;

            // ��鸲�� �߻��� ������
            if (timeSinceLastShake >= shakeInterval)
            {
                // ������ �������� ��鸲
                Vector3 shakeOffset = new Vector3(
                    Random.Range(-shakeMagnitude, shakeMagnitude),
                    0,
                    Random.Range(-shakeMagnitude, shakeMagnitude));

                transform.localPosition = originalPos + shakeOffset;

                // ������ ��鸲 �� �ð� �ʱ�ȭ
                timeSinceLastShake = 0.0f;

                // ��鸲 ������ �����ϰ� ���ݾ� ��ȭ���Ѽ� �ұ�Ģ���� ������ ��
                shakeInterval = Random.Range(0.2f, maxShakeInterval);
            }

            // ��鸲�� ������ ���� ��ġ�� ����
            currentShakeTime += Time.deltaTime;
            if (currentShakeTime >= shakeDuration)
            {
                isShaking = false;
                transform.localPosition = originalPos;
            }
        }
    }

    // ��鸲 ���� �޼���
    public void StartShake(float duration, Vector3 pos)
    {
        originalPos = pos;
        shakeDuration = duration;
        isShaking = true;
        currentShakeTime = 0f;
        timeSinceLastShake = 0f;
    }
}
