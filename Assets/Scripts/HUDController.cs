using UnityEngine;

// for ui text
using TMPro;


public class HUDController : MonoBehaviour
{
    [SerializeField] private GameController gameController;

    public TMP_Text PhotosLeft;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        PhotosLeft.text = string.Concat("Photos left: ", GameController.MAX_PHOTOS - gameController.photosTaken);
    }
}
