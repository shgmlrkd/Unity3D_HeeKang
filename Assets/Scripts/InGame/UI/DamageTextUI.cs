using System.Collections;
using TMPro;
using UnityEngine;

public class DamageTextUI : MonoBehaviour
{
    private TextMeshProUGUI _damageText; 
    private Transform _monsterTransform;

    private Vector3 _offset = new Vector3(0.0f, 1.0f, 0.0f);

    private Vector3 _targetPos;

    private void Awake()
    {
        _damageText = GetComponent<TextMeshProUGUI>();
    }

    // 데미지 텍스트 설정
    public void SetDamageText(Transform transform, float damage, Color color)
    {
        _damageText.text = damage.ToString("F0");
        color.a = 1.0f;
        _damageText.color = color;
        _monsterTransform = transform;

        StartCoroutine(DamageTextEffect());
    }

    private IEnumerator DamageTextEffect()
    {
        float duration = 1.0f;
        float elapsed = 0.0f;
        float frequency = 10.0f;
        float amplitude = 1.5f; // 튀는 높이
        float xSpeed = 1.3f;    // 오른쪽 이동 속도

        Color originalColor = _damageText.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;

            // 통통 튀는 Y 오프셋
            float bounceY = Mathf.Abs(Mathf.Sin(elapsed * frequency)) * amplitude * (1.0f - progress);
            // X축 Right방향으로 계속 가는거
            float moveX = xSpeed * elapsed;

            // 월드 위치 기준 + 연출 오프셋
            Vector3 worldPos = _monsterTransform.position + _offset + new Vector3(moveX, bounceY, 0.0f);

            // 현재 위치를 스크린 좌표로 변환
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
            transform.position = screenPos;

            // 점점 사라지는 알파
            Color newColor = originalColor;
            newColor.a = Mathf.Lerp(1.0f, 0.0f, progress);
            _damageText.color = newColor;

            yield return null;
        }

        gameObject.SetActive(false);
    }
}