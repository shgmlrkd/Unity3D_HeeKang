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

        // 플레이어 위치를 중심으로 설정
        Vector3 center = transform.position;
        center.y = 0.0f;

        // 회전 반경 설정
        float distance = _weaponData.AttackRange;
        // 각도 간격 계산
        float angleStep = Mathf.PI * 2 / _weaponData.ProjectileCount;

        for (int i = 0; i < _weaponData.ProjectileCount; i++)
        {
            // 라디안 값으로 angle구함
            float angle = angleStep * i;

            // 각도를 기준으로 위치 계산
            float x = Mathf.Cos(angle) * distance;
            float z = Mathf.Sin(angle) * distance;
            // 회전 위치 계산
            Vector3 spawnPos = new Vector3(x, 0.0f, z) + center;
            // 플레이어 중심으로 회전시키기 위해 transform 전달
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
            // 도끼들을 활성화하고 페이드 인 연출 시작
            foreach (GameObject axe in axes)
            {
                axe.SetActive(true);
                StartCoroutine(FadeAxe(axe, _minAlphaValue, _maxAlphaValue, _fadeLerpTime));
            }
            yield return new WaitForSeconds(lifeTime);
            
            // 도끼들을 페이드 아웃
            foreach (GameObject axe in axes)
            {
                StartCoroutine(FadeAxe(axe, _maxAlphaValue, _minAlphaValue, _fadeLerpTime));
            }
            yield return new WaitForSeconds(_fadeLerpTime);

            // 일정 시간 동안 비활성화 처리
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

        // 일정 시간 동안 알파 값을 선형 보간해 페이드 인/아웃 처리
        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            foreach (Material mat in materials)
            {
                Color color = mat.color;
                // 알파 값을 보간
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

    public override void StartSkill()
    {
        StartSpinAxe();
    }

    public override void StopSkill()
    {
        ClearSpinAxe();
    }
}