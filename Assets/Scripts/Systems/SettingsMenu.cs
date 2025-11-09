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
    public TMP_Dropdown fullscreenDropdown;

    [Header("Jogabilidade")]
    public TMP_Dropdown languageDropdown;

    [Header("Acessibilidade")]
    public TMP_Dropdown colorBlindDropdown;

    private void Start()
    {
        ResolutionDropdownPopulate();
        FullScreenDropdownPopulate();
        ColorBlindDropdownPopulate();
        LanguageDropdownPopulate();

        LocalizationManager.Instance.OnLanguageChanged += FullScreenDropdownPopulate;
        
        // Inicializa os controles com os valores salvos
        var gsm = GameSettingsManager.Instance;
        if (masterVolumeSlider != null) masterVolumeSlider.value = gsm.masterVolume;
        if (musicVolumeSlider != null) musicVolumeSlider.value = gsm.musicVolume;
        if (sfxVolumeSlider != null) sfxVolumeSlider.value = gsm.sfxVolume;
        if (musicToggle != null) musicToggle.isOn = gsm.musicEnabled;
        if (sfxToggle != null) sfxToggle.isOn = gsm.sfxEnabled;
        if (fullscreenDropdown != null) fullscreenDropdown.value = gsm.fullscreenMode;
        if (colorBlindDropdown != null) colorBlindDropdown.value = gsm.colorBlindMode;
        if (languageDropdown != null) languageDropdown.value = gsm.languageIndex;
        if (resolutionDropdown != null) resolutionDropdown.value = gsm.resolutionIndex;
    }

    void OnDestroy()
    {
        LocalizationManager.Instance.OnLanguageChanged -= FullScreenDropdownPopulate;
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
            Screen.SetResolution(res.width, res.height, (FullScreenMode)GameSettingsManager.Instance.fullscreenMode);
        }
    }
    public void OnFullscreenChanged()
    {
        GameSettingsManager.Instance.fullscreenMode = fullscreenDropdown.value;
        GameSettingsManager.Instance.SaveSettings();
        switch (fullscreenDropdown.value)
        {
            case 0: // Tela cheia tradicional
                Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                break;
            case 1: // Tela cheia sem borda
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                break;
            case 2: // Tela cheia maximizada
                Screen.fullScreenMode = FullScreenMode.MaximizedWindow;
                break;
            case 3: // Janela 
                Screen.fullScreenMode = FullScreenMode.Windowed;
                break;
        }
    }

    // Jogabilidade(Not implemented yet)
    public void OnLanguageChanged()
    {
        GameSettingsManager.Instance.languageIndex = languageDropdown.value;
        GameSettingsManager.Instance.SaveSettings();
        LocalizationManager.Instance.SetLanguage((Language)languageDropdown.value);
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
    void FullScreenDropdownPopulate()
    {
        if (fullscreenDropdown != null)
        {
            fullscreenDropdown.ClearOptions();
            var options = new System.Collections.Generic.List<string>();
            switch (LocalizationManager.Instance.currentLanguage)
            {
                case Language.PT_BR:
                    options.Add("Tela Cheia");
                    options.Add("Sem Borda");
                    options.Add("Maximizada");
                    options.Add("Janela");
                    break;
                case Language.ES_ES:
                    options.Add("Pantalla Completa");
                    options.Add("Sin Bordes");
                    options.Add("Maximizada");
                    options.Add("Ventana");
                    break;
                default:
                    options.Add("Fullscreen");
                    options.Add("Borderless");
                    options.Add("Maximized");
                    options.Add("Windowed");
                    break;
            }

            fullscreenDropdown.AddOptions(options);
            int savedIndex = GameSettingsManager.Instance.fullscreenMode;
            fullscreenDropdown.value = (savedIndex < options.Count) ? savedIndex : 0;
            fullscreenDropdown.RefreshShownValue();
        }
    }
    void ColorBlindDropdownPopulate()
    {
        if (colorBlindDropdown != null)
        {
            colorBlindDropdown.ClearOptions();
            var options = new System.Collections.Generic.List<string>();
            foreach (var name in System.Enum.GetNames(typeof(VisionTypeNames)))
            {
                options.Add(name);
            }
            colorBlindDropdown.AddOptions(options);
            int savedIndex = GameSettingsManager.Instance.colorBlindMode;
            colorBlindDropdown.value = (savedIndex < options.Count) ? savedIndex : 0;
            colorBlindDropdown.RefreshShownValue();
        }
    }
    void LanguageDropdownPopulate()
    {
        if (languageDropdown != null)
        {
            languageDropdown.ClearOptions();
            var options = new System.Collections.Generic.List<string>();
            foreach (var name in System.Enum.GetNames(typeof(Language)))
            {
                options.Add(name);
            }
            languageDropdown.AddOptions(options);
            int savedIndex = GameSettingsManager.Instance.languageIndex;
            languageDropdown.value = (savedIndex < options.Count) ? savedIndex : 0;
            languageDropdown.RefreshShownValue();
        }
    }
}
