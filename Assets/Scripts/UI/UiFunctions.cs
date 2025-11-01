using UnityEngine;
using UnityEngine.SceneManagement;

public class UiFunctions : MonoBehaviour
{
    
    [SerializeField] private string GameSceneName = "Main";
    public void PlayGame()
    {
        SceneManager.LoadScene(GameSceneName);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    [SerializeField] private GameObject CreditsUI;
    public void OpenCredits()
    {
        if (CreditsUI != null)
            CreditsUI.SetActive(true);
    }
    public void CloseCredits()
    {
        if (CreditsUI != null)
            CreditsUI.SetActive(false);
    }

    [SerializeField] private GameObject ConfigsUI;
    public void OpenConfigs()
    {
        if (ConfigsUI != null)
            ConfigsUI.SetActive(true);
    }
    public void CloseConfigs()
    {
        if (ConfigsUI != null)
            ConfigsUI.SetActive(false);
    }
}
