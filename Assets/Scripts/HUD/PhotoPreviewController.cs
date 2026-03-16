using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

using System;
using System.IO;
using System.Collections;

public class PhotoPreviewController : MonoBehaviour
{
    // CONSTANTS //

    // score thresholds for the different tiers of borders
    private const int T1_THRESHOLD = 50;
    private const int T2_THRESHOLD = 100;
    private const int T3_THRESHOLD = 200;

    // how long a photo preview is
    private const float PREVIEW_DURATION = 1.5f;

    // AUDIO //

    // GAME COMPONENTS //

    [SerializeField] private GameObject backplate;
    [SerializeField] private GameObject t1_border;
    [SerializeField] private GameObject t2_border;
    [SerializeField] private GameObject t3_border;
    [SerializeField] private RawImage photoPreview;


    // the currently running photo display routine
    private Coroutine previewRoutine;

    // VARIABLES //

    // LOGGING //


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        backplate.SetActive(false);
        t1_border.SetActive(false);
        t2_border.SetActive(false);
        t3_border.SetActive(false);
        photoPreview.gameObject.SetActive(false);

        previewRoutine = null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisplayPhotoPreview(int score, Texture2D photo)
    {
        // cancel currently running routine
        // i.e. fucntion for current photo on display
        // i don't think this takes down the photo, just stops the execution (and timer)
        // doesn't matter that photo isn't taken down; we are putting up another right now.
        if (previewRoutine != null)
        {
            StopCoroutine(previewRoutine);
        }

        // launch a new photo preview
        previewRoutine = StartCoroutine(PreviewRoutine(score, photo));
    }

    private IEnumerator PreviewRoutine(int score, Texture2D photo)
    {
        // attach the photo texture to the photo ui element
        photoPreview.texture = photo;

        // enable the game objects for the photo and the white backplate
        photoPreview.gameObject.SetActive(true);
        backplate.SetActive(true);
        // enable the gameobject for the border glow
        SetBorder(score);

        // wait for the duration of the preview
        yield return new WaitForSecondsRealtime(PREVIEW_DURATION);

        // disable all the preview gameobjects
        photoPreview.gameObject.SetActive(false);
        backplate.SetActive(false);
        SetNoBorder();

        // mark the routine as not running
        previewRoutine = null;
    }

    private void SetBorder(int score)
    {
        if (score > T3_THRESHOLD)
        {
            SetT3Border();
        }
        else if (score > T2_THRESHOLD)
        {
            SetT2Border();
        }
        else if (score > T1_THRESHOLD)
        {
            SetT1Border();
        }
        else
        {
            SetNoBorder();
        }
    }

    private void SetNoBorder()
    {
        t1_border.SetActive(false);
        t2_border.SetActive(false);
        t3_border.SetActive(false);
    }
    private void SetT1Border()
    {
        t1_border.SetActive(true);
        t2_border.SetActive(false);
        t3_border.SetActive(false);
    }

    private void SetT2Border()
    {
        t1_border.SetActive(false);
        t2_border.SetActive(true);
        t3_border.SetActive(false);
    }

    private void SetT3Border()
    {
        t1_border.SetActive(false);
        t2_border.SetActive(false);
        t3_border.SetActive(true);
    }
}
