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

        Vector3 center = transform.position; // 중심은 플레이어 위치
        center.y = 0.0f;

        float radius = _weaponData.AttackRange; // 도는 반지름
        float angleStep = Mathf.PI * 2 / _weaponData.ProjectileCount; // 라디안 값

        for (int i = 0; i < _weaponData.ProjectileCount; i++)
        {
            // 라디안 값으로 angle구함
            float angle = angleStep * i;

            // 각도를 기준으로 위치 계산
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;
            // 스폰 위치
            Vector3 spawnPos = new Vector3(x, 0.0f, z) + center;
            // 플레이어 기준으로 스폰 후 움직이기 때문에 transform도 전달
            WeaponManager.Instance.StartAxeSpin(transform, spawnPos, _weaponData);
        }

        // 활성화 되어있는 도끼들만 넣음
        foreach(GameObject axe in _axes)
        {
            if(axe.activeSelf)
                _activateAxes.Add(axe);
        }

        // 실행 중인 코루틴 저장 및 코루틴 시작
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
                StartCoroutine(FadeAxe(axe, _minAlphaValue, _maxAlphaValue, _fadeLerpTime)); // 0.5초 동안 페이드 인
            }
            yield return new WaitForSeconds(lifeTime);
            
            foreach (GameObject axe in axes)
            {
                StartCoroutine(FadeAxe(axe, _maxAlphaValue, _minAlphaValue, _fadeLerpTime)); // 0.5초 동안 페이드 아웃
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

        // 없으면 멈춤
        if (renderers.Length == 0) 
            yield break;

        List<Material> materials = new List<Material>();
        foreach (Renderer renderer in renderers)
        {
            materials.Add(renderer.material); // 각각 인스턴스 확보
        }

        // 일정 시간동안 보간으로 페이드 인 or 페이드 아웃
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

        // 알파값 세팅 해주기
        foreach (Material mat in materials)
        {
            Color color = mat.color;
            color.a = toAlpha;
            mat.color = color;
        }
    }

    // 다시 세팅하기 위해 초기화 시키는 작업
    private void ClearSpinAxe()
    {
        // 레벨업 했을 경우 다시 활성화 된 
        // 도끼들을 찾아야하므로 비워 줌
        _activateAxes.Clear();

        // 실행 중인 코루틴은 정지
        foreach (Coroutine coroutine in _runningCoroutines)
        {
            StopCoroutine(coroutine);
        }
        // 이후 비워 줌
        _runningCoroutines.Clear();

        _axes = WeaponManager.Instance.GetObjects("Axe");
        // 도끼들 중 활성화 상태인 도끼는 모두 비활성화
        foreach(GameObject axe in _axes)
        {
            if(axe.activeSelf)
                axe.SetActive(false);
        }
    }
}
