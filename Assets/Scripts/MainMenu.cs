using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject CreditsPopup;

    public void PlayGame()
    {
        SceneManager.LoadScene("mainmap");
    }

    public void OpenCredits()
    {
        CreditsPopup.SetActive(true);
    }

    public void CloseCredits()
    {
        CreditsPopup.SetActive(false);
    }
}