using System;
using System.Collections.Generic;
using UnityEngine;

public enum SoundKey
{
    PlayerRunSound
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

    public void PlayBGM(SoundKey key)
    {
        _bgmSource.clip = _soundData[key];
        _bgmSource.playOnAwake = true;
        _bgmSource.loop = true;
        _bgmSource.volume = 0.3f;
        _bgmSource.Play();
    }

    public void PlayFX(SoundKey key)
    {
        _fxSource.volume = 0.5f;
        _fxSource.PlayOneShot(_soundData[key]);
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
    public void StopFX()
    {
        _fxSource.Stop();  // 효과음 정지
    }

    public void StopBGM()
    {
        _bgmSource.Stop();
    }

    public void SetMasterVolume(float volume)
    {
        AudioListener.volume = volume;
    }
}
