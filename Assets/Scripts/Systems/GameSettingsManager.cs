using UnityEngine;
using UnityEngine.Audio;

public class GameSettingsManager : MonoBehaviour
{
    public static GameSettingsManager Instance { get; private set; }

    public AudioMixer audioMixer;


    public float masterVolume = 1f;
    public float musicVolume = 1f;
    public float sfxVolume = 1f;
    public bool musicEnabled = true;
    public bool sfxEnabled = true;
    public int resolutionIndex = 0;
    public int fullscreenMode = 0;
    public int languageIndex = 0;
    public int colorBlindMode = 0;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadSettings();
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.SetInt("MusicEnabled", musicEnabled ? 1 : 0);
        PlayerPrefs.SetInt("SFXEnabled", sfxEnabled ? 1 : 0);
        PlayerPrefs.SetInt("ResolutionIndex", resolutionIndex);
        PlayerPrefs.SetInt("FullscreenMode", fullscreenMode);
        PlayerPrefs.SetInt("LanguageIndex", languageIndex);
        PlayerPrefs.SetInt("ColorBlindMode", colorBlindMode);
        PlayerPrefs.Save();
    }

    public void LoadSettings()
    {
        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        musicEnabled = PlayerPrefs.GetInt("MusicEnabled", 1) == 1;
        sfxEnabled = PlayerPrefs.GetInt("SFXEnabled", 1) == 1;
        resolutionIndex = PlayerPrefs.GetInt("ResolutionIndex", 0);
        fullscreenMode = PlayerPrefs.GetInt("FullscreenMode", 1);
        languageIndex = PlayerPrefs.GetInt("LanguageIndex", 0);
        colorBlindMode = PlayerPrefs.GetInt("ColorBlindMode", 0);
    }
}
