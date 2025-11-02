using UnityEngine;

using UnityEngine.UI;
using TMPro;
using SOG.CVDFilter;
public class SettingsMenu : MonoBehaviour
{
    [Header("Áudio")]
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    public Toggle musicToggle;
    public Toggle sfxToggle;

    [Header("Vídeo/Gráficos")]
    public TMP_Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;

    [Header("Jogabilidade")]
    public TMP_Dropdown languageDropdown;

    [Header("Acessibilidade")]
    public TMP_Dropdown colorBlindDropdown;

    private void Start()
    {
        ResolutionDropdownPopulate();
        ColorBlindDropdownPopulate();
        // Inicializa os controles com os valores salvos
        var gsm = GameSettingsManager.Instance;
        if (masterVolumeSlider != null) masterVolumeSlider.value = gsm.masterVolume;
        if (musicVolumeSlider != null) musicVolumeSlider.value = gsm.musicVolume;
        if (sfxVolumeSlider != null) sfxVolumeSlider.value = gsm.sfxVolume;
        if (musicToggle != null) musicToggle.isOn = gsm.musicEnabled;
        if (sfxToggle != null) sfxToggle.isOn = gsm.sfxEnabled;
        if (fullscreenToggle != null) fullscreenToggle.isOn = gsm.fullscreen;
        if (colorBlindDropdown != null) colorBlindDropdown.value = gsm.colorBlindMode;
        if (languageDropdown != null) languageDropdown.value = gsm.languageIndex;
        if (resolutionDropdown != null) resolutionDropdown.value = gsm.resolutionIndex;
    }

    // Áudio
    public void OnMasterVolumeChanged()
    {
        GameSettingsManager.Instance.masterVolume = masterVolumeSlider.value;
        GameSettingsManager.Instance.SaveSettings();
        SoundsManager.Instance.SetMasterVolume(masterVolumeSlider.value);
    }
    public void OnMusicVolumeChanged()
    {
        GameSettingsManager.Instance.musicVolume = musicVolumeSlider.value;
        GameSettingsManager.Instance.SaveSettings();
        SoundsManager.Instance.SetMusicVolume(musicVolumeSlider.value);
    }
    public void OnSFXVolumeChanged()
    {
        GameSettingsManager.Instance.sfxVolume = sfxVolumeSlider.value;
        GameSettingsManager.Instance.SaveSettings();
        SoundsManager.Instance.SetSFXVolume(sfxVolumeSlider.value);
    }
    public void OnMusicToggleChanged()
    {
        GameSettingsManager.Instance.musicEnabled = musicToggle.isOn;
        GameSettingsManager.Instance.SaveSettings();
        musicVolumeSlider.interactable = musicToggle.isOn;
        SoundsManager.Instance.SetMusicVolume(musicToggle.isOn ? GameSettingsManager.Instance.musicVolume : 0f);
    }
    public void OnSFXToggleChanged()
    {
        GameSettingsManager.Instance.sfxEnabled = sfxToggle.isOn;
        GameSettingsManager.Instance.SaveSettings();
        sfxVolumeSlider.interactable = sfxToggle.isOn;
        SoundsManager.Instance.SetSFXVolume(sfxToggle.isOn ? GameSettingsManager.Instance.sfxVolume : 0f);
    }

    // Vídeo/Gráficos
    public void OnResolutionChanged()
    {
        int index = resolutionDropdown.value;
        GameSettingsManager.Instance.resolutionIndex = index;
        GameSettingsManager.Instance.SaveSettings();
        // Exemplo: aplicar resolução
        Resolution[] resolutions = Screen.resolutions;
        if (index >= 0 && index < resolutions.Length)
        {
            Resolution res = resolutions[index];
            Screen.SetResolution(res.width, res.height, GameSettingsManager.Instance.fullscreen);
        }
    }
    public void OnFullscreenChanged()
    {
        GameSettingsManager.Instance.fullscreen = fullscreenToggle.isOn;
        GameSettingsManager.Instance.SaveSettings();
        Screen.fullScreen = fullscreenToggle.isOn;
    }

    // Jogabilidade(Not implemented yet)
    public void OnLanguageChanged()
    {
        Debug.LogWarning("Language change functionality is not yet implemented.");
        GameSettingsManager.Instance.languageIndex = languageDropdown.value;
        GameSettingsManager.Instance.SaveSettings();
        // Troque textos do jogo conforme idioma
    }

    // Acessibilidade
    public void OnColorBlindModeChanged()
    {
        FindFirstObjectByType<CVDFilter>().ChangeProfile(colorBlindDropdown.value);
        GameSettingsManager.Instance.colorBlindMode = colorBlindDropdown.value;
        GameSettingsManager.Instance.SaveSettings();
    }
    
    //Populate Dropdowns
    void ResolutionDropdownPopulate()
    {
        if (resolutionDropdown != null)
        {
            resolutionDropdown.ClearOptions();
            var options = new System.Collections.Generic.List<string>();
            Resolution[] resolutions = Screen.resolutions;
            int currentResIndex = 0;
            for (int i = 0; i < resolutions.Length; i++)
            {
                string option = resolutions[i].width + " x " + resolutions[i].height + " @ " + resolutions[i].refreshRateRatio + "Hz";
                options.Add(option);
                if (resolutions[i].width == Screen.currentResolution.width &&
                    resolutions[i].height == Screen.currentResolution.height &&
                    resolutions[i].refreshRateRatio.Equals(Screen.currentResolution.refreshRateRatio))
                {
                    currentResIndex = i;
                }
            }
            resolutionDropdown.AddOptions(options);
            // Seleciona a resolução salva ou a atual
            int savedIndex = GameSettingsManager.Instance.resolutionIndex;
            resolutionDropdown.value = (savedIndex < options.Count) ? savedIndex : currentResIndex;
            resolutionDropdown.RefreshShownValue();
        }
    }  
    void ColorBlindDropdownPopulate()
    {
        if (colorBlindDropdown != null)
        {
            colorBlindDropdown.ClearOptions();
            var options = new System.Collections.Generic.List<string>();
            foreach(var name in System.Enum.GetNames(typeof(VisionTypeNames)))
            {
                options.Add(name);
            }
            colorBlindDropdown.AddOptions(options);
            int savedIndex = GameSettingsManager.Instance.colorBlindMode;
            colorBlindDropdown.value = (savedIndex < options.Count) ? savedIndex : 0;
            colorBlindDropdown.RefreshShownValue();
        }
    }
}
