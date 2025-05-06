using TMPro;
using UnityEngine;

public class DamageTextManager : Singleton<DamageTextManager>
{
    public void CreateDamageTexts(int poolSize, string key)
    {
        GameObject damageTextPrefab = Resources.Load<GameObject>("Prefabs/UI/DamageText");
        PoolingManager.Instance.Add(key, poolSize, damageTextPrefab, GameObject.Find("DamageUIPanel").transform);
    }

    public void ShowDamageText(Transform transform, float damage, Color color)
    {
        GameObject damageTextObj = PoolingManager.Instance.Pop("DamageText");

        DamageTextUI damageTextScript = damageTextObj.GetComponent<DamageTextUI>();

        // 텍스트 설정
        damageTextScript.SetDamageText(transform, damage, color);
    }
}