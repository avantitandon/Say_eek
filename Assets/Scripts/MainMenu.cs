using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private InputAction start_game_button;

    public GameObject CreditsPopup;


    void Start()
    {
        start_game_button = InputSystem.actions.FindAction("PhoneToggle");
    }

    void Update()
    {
        if (start_game_button.WasPressedThisFrame())
        {
            PlayGame();
        }
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("newmain");
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