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

    // ������ �ؽ�Ʈ ����
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
        float amplitude = 1.5f; // Ƣ�� ����
        float xSpeed = 1.3f;    // ������ �̵� �ӵ�

        Color originalColor = _damageText.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;

            // ���� Ƣ�� Y ������
            float bounceY = Mathf.Abs(Mathf.Sin(elapsed * frequency)) * amplitude * (1.0f - progress);
            // X�� Right�������� ��� ���°�
            float moveX = xSpeed * elapsed;

            // ���� ��ġ ���� + ���� ������
            Vector3 worldPos = _monsterTransform.position + _offset + new Vector3(moveX, bounceY, 0.0f);

            // ���� ��ġ�� ��ũ�� ��ǥ�� ��ȯ
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
            transform.position = screenPos;

            // ���� ������� ����
            Color newColor = originalColor;
            newColor.a = Mathf.Lerp(1.0f, 0.0f, progress);
            _damageText.color = newColor;

            yield return null;
        }

        gameObject.SetActive(false);
    }
}