using System.Collections.Generic;
using System;
using UnityEngine;
using System.Collections;

public enum SoundKey
{
    NormalWeaponHitSound, LaserHitSound, AxeHitSound,
    ExpSound, HpRecoverySound,
    ButtonClickSound, SkillButtonClickSound,
    BossAttackSound, BossIntroSound, BossRushSound, BossRoarSound, BossDeathSound, VictorySound,
    TitleBGM, NormalBGM, BossBGM
}

[Serializable]
public struct SoundInfo
{
    public SoundKey Key;
    public AudioClip Clip;
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField]
    private List<SoundInfo> _soundsInfo = new List<SoundInfo>();

    private Dictionary<SoundKey, AudioClip> _soundData = new Dictionary<SoundKey, AudioClip>();

    [SerializeField]
    private AudioSource _bgmSource;

    [SerializeField]
    private AudioSource _fxSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        foreach (SoundInfo info in _soundsInfo)
        {
            _soundData.Add(info.Key, info.Clip);
        }
    }

    public void PlayBGM(SoundKey key, float volume, float fadeDuration = 0.75f)
    {
        StartCoroutine(FadeInBGM(key, volume, fadeDuration));
    }

    private IEnumerator FadeInBGM(SoundKey key, float targetVolume, float duration)
    {
        _bgmSource.clip = _soundData[key];
        _bgmSource.loop = true;
        _bgmSource.volume = 0.0f;
        _bgmSource.Play();

        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            _bgmSource.volume = Mathf.Lerp(0.0f, targetVolume, timer / duration);
            yield return null;
        }

        _bgmSource.volume = targetVolume;
    }

    public void PlayFX(SoundKey key, float volume)
    {
        _fxSource.PlayOneShot(_soundData[key], volume);
    }

    public void PauseBGM(bool pause = true)
    {
        if (pause)
        {
            _bgmSource.Pause();
        }
        else
        {
            _bgmSource.UnPause();
        }
    }

    public void StopBGM()
    {
        _bgmSource.Stop();
    }

    /*public void StopBGM(float fadeDuration = 0.25f)
    {
        StartCoroutine(FadeOutBGM(fadeDuration));
    }

    private IEnumerator FadeOutBGM(float duration)
    {
        float startVolume = _bgmSource.volume;

        while (_bgmSource.volume > 0)
        {
            _bgmSource.volume -= startVolume * Time.deltaTime / duration;
            yield return null;
        }

        _bgmSource.Stop();
        _bgmSource.volume = startVolume; // 다음 재생을 위해 볼륨 복원
    }*/

    public void SetMasterVolume(float volume)
    {
        AudioListener.volume = volume;
    }
}
