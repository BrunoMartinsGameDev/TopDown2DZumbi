using UnityEngine;

[RequireComponent(typeof(TMPro.TMP_Text))]
public class LocalizateText : MonoBehaviour
{
    [HideInInspector] public string key;
    private TMPro.TMP_Text tmpText;

    void Start()
    {
        tmpText = GetComponent<TMPro.TMP_Text>();
        UpdateText();
        // Inscreve para atualizar quando o idioma mudar
        if (LocalizationManager.Instance != null)
            LocalizationManager.Instance.OnLanguageChanged += UpdateText;
    }

    void OnDestroy()
    {
        // Remove inscrição para evitar memory leak
        if (LocalizationManager.Instance != null)
            LocalizationManager.Instance.OnLanguageChanged -= UpdateText;
    }

    public void UpdateText()
    {
        if (tmpText != null && !string.IsNullOrEmpty(key) && LocalizationManager.Instance != null)
        {
            tmpText.text = LocalizationManager.Instance.GetLocalizedValue(key);
        }
    }
}
