using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeSkill : Skill
{
    private List<GameObject> _axes;
    private List<GameObject> _activateAxes;
    private List<Coroutine> _runningCoroutines; 

    private int _axeIndexKey = 315;

    private void Awake()
    {
        _fadeLerpTime = 0.5f;
        _axes = new List<GameObject>();
        _activateAxes = new List<GameObject>();
        _runningCoroutines = new List<Coroutine>();
        _weaponData = WeaponDataManager.Instance.GetWeaponData(_axeIndexKey);
    }

    private void Start()
    {
        StartSpinAxe();
    }

    public override void LevelUp()
    {
        base.LevelUp();
        _weaponData = WeaponDataManager.Instance.GetWeaponData(_axeIndexKey + _level);
        StartSpinAxe();
    }

    private void StartSpinAxe()
    {
        ClearSpinAxe();

        Vector3 center = transform.position; // �߽��� �÷��̾� ��ġ
        center.y = 0.0f;

        float radius = _weaponData.AttackRange; // ���� ������
        float angleStep = Mathf.PI * 2 / _weaponData.ProjectileCount; // ���� ��

        for (int i = 0; i < _weaponData.ProjectileCount; i++)
        {
            // ���� ������ angle����
            float angle = angleStep * i;

            // ������ �������� ��ġ ���
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;
            // ���� ��ġ
            Vector3 spawnPos = new Vector3(x, 0.0f, z) + center;
            // �÷��̾� �������� ���� �� �����̱� ������ transform�� ����
            WeaponManager.Instance.StartAxeSpin(transform, spawnPos, _weaponData);
        }

        // Ȱ��ȭ �Ǿ��ִ� �����鸸 ����
        foreach(GameObject axe in _axes)
        {
            if(axe.activeSelf)
                _activateAxes.Add(axe);
        }

        // ���� ���� �ڷ�ƾ ���� �� �ڷ�ƾ ����
        Coroutine coroutine = StartCoroutine(AxesLifeCycle(_activateAxes, _weaponData.LifeTime));
        _runningCoroutines.Add(coroutine);
    }

    private IEnumerator AxesLifeCycle(List<GameObject> axes, float lifeTime)
    {
        while (true)
        {
            foreach (GameObject axe in axes)
            {
                axe.SetActive(true);
                StartCoroutine(FadeAxe(axe, _minAlphaValue, _maxAlphaValue, _fadeLerpTime)); // 0.5�� ���� ���̵� ��
            }
            yield return new WaitForSeconds(lifeTime);
            
            foreach (GameObject axe in axes)
            {
                StartCoroutine(FadeAxe(axe, _maxAlphaValue, _minAlphaValue, _fadeLerpTime)); // 0.5�� ���� ���̵� �ƿ�
            }

            yield return new WaitForSeconds(_fadeLerpTime);


            foreach (GameObject axe in axes)
            {
                axe.SetActive(false);
            }
            yield return new WaitForSeconds(lifeTime);
        }
    }

    private IEnumerator FadeAxe(GameObject axe, float fromAlpha, float toAlpha, float duration)
    {
        Renderer[] renderers = axe.GetComponentsInChildren<Renderer>();

        // ������ ����
        if (renderers.Length == 0) 
            yield break;

        List<Material> materials = new List<Material>();
        foreach (Renderer renderer in renderers)
        {
            materials.Add(renderer.material); // ���� �ν��Ͻ� Ȯ��
        }

        // ���� �ð����� �������� ���̵� �� or ���̵� �ƿ�
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            foreach (Material mat in materials)
            {
                Color color = mat.color;
                color.a = Mathf.Lerp(fromAlpha, toAlpha, t);
                mat.color = color;
            }
            yield return null;
        }

        // ���İ� ���� ���ֱ�
        foreach (Material mat in materials)
        {
            Color color = mat.color;
            color.a = toAlpha;
            mat.color = color;
        }
    }

    // �ٽ� �����ϱ� ���� �ʱ�ȭ ��Ű�� �۾�
    private void ClearSpinAxe()
    {
        // ������ ���� ��� �ٽ� Ȱ��ȭ �� 
        // �������� ã�ƾ��ϹǷ� ��� ��
        _activateAxes.Clear();

        // ���� ���� �ڷ�ƾ�� ����
        foreach (Coroutine coroutine in _runningCoroutines)
        {
            StopCoroutine(coroutine);
        }
        // ���� ��� ��
        _runningCoroutines.Clear();

        _axes = WeaponManager.Instance.GetObjects("Axe");
        // ������ �� Ȱ��ȭ ������ ������ ��� ��Ȱ��ȭ
        foreach(GameObject axe in _axes)
        {
            if(axe.activeSelf)
                axe.SetActive(false);
        }
    }
}
