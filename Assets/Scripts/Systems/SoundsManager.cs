using Unity.VisualScripting;
using UnityEngine;

public class SoundsManager : MonoBehaviour
{
    public static SoundsManager Instance;
    [SerializeField] private AudioSource musicSource;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlayMusic(AudioClip clip)
    {
        musicSource.clip = clip;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }
    public void PlaySFX(AudioClip clip, Vector3 position)
    {
        GameObject sfxObject = new GameObject("SFX");
        sfxObject.transform.position = position;
        AudioSource source = sfxObject.AddComponent<AudioSource>();
        source.outputAudioMixerGroup = GameSettingsManager.Instance.audioMixer.FindMatchingGroups("Sfx")[0];
        source.clip = clip;
        source.Play();
        Destroy(sfxObject, clip.length);
    }


    public void SetMasterVolume(float volume)
    {
        float value = Mathf.Clamp(volume, 0.0001f, 1f);
        GameSettingsManager.Instance.audioMixer.SetFloat("Master", Mathf.Log10(value) * 20);
    }
    public void SetSFXVolume(float volume)
    {
        float value = Mathf.Clamp(volume, 0.0001f, 1f);
        GameSettingsManager.Instance.audioMixer.SetFloat("Sfx", Mathf.Log10(value) * 20);
    }
    public void SetMusicVolume(float volume)
    {
        float value = Mathf.Clamp(volume, 0.0001f, 1f);
        GameSettingsManager.Instance.audioMixer.SetFloat("Music", Mathf.Log10(value) * 20);
    }

}
