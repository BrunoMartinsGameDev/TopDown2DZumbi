using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;

public class LocalizationDB : Dictionary<string, Dictionary<string, string>> { }

public enum Language
{
    EN_US,
    PT_BR,
}

public class LocalizationManager : MonoBehaviour
{
    private LocalizationDB localizationDB;
    public static LocalizationManager Instance;
    public Language currentLanguage = Language.EN_US;

    // Evento chamado quando o idioma muda
    public event System.Action OnLanguageChanged;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadLocalizationData();
            currentLanguage = (Language)GameSettingsManager.Instance.languageIndex;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadLocalizationData()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "localization.json");
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            localizationDB = JsonConvert.DeserializeObject<LocalizationDB>(json);
        }
        else
        {
            Debug.LogError("Localization file not found");
        }
    }

    public string GetLocalizedValue(string key)
    {
        if (localizationDB != null && localizationDB.TryGetValue(key, out var translations))
        {
            if (translations.TryGetValue(currentLanguage.ToString(), out var value))
            {
                return value;
            }
        }
        return key; // Fallback to key if not found
    }

    public void SetLanguage(Language language)
    {
        currentLanguage = language;
        OnLanguageChanged?.Invoke();
    }
}
