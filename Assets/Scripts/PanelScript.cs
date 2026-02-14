using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PanelScript : MonoBehaviour
{

    public GameObject panel;
    public Image screenshotDisplay;
    public bool IsPhoneOpen => panel != null && panel.activeSelf;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.iKey.wasPressedThisFrame)
        {
            if (panel != null)
            {
                panel.SetActive(!panel.activeSelf);
            }
        }
    }
    // DEBUG LIKE MY LIFE DEPENDS ON IT AHDJSAHFKDSAHFDJKSHAFKDSAHFKEAHFKSAHFDSAFKDSHFDKSAF

    public void DisplayScreenshot(Texture2D screenshot)
    {
        
        Debug.Log("DisplayScreenshot called");
        if (screenshotDisplay != null)
        {
            screenshotDisplay.sprite = Sprite.Create(screenshot, new Rect(0, 0, screenshot.width, screenshot.height), Vector2.zero);
            Debug.Log("Screenshot displayed on panel");
            if (panel != null && !panel.activeSelf)
            { panel.SetActive(true);
                Debug.Log("Panel activated to show screenshot");
            }
        }

        // more debug more debug more debuvbdjsahfskjahfdkjsahjkfvshakfdh
        else
        {
            Debug.LogError("screenshotDisplay is null!");
        }
    }
}
