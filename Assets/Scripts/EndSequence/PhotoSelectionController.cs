using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections.Generic;


using TMPro;

public class PhotoSelectionController : MonoBehaviour
{
    // CONSTANTS

    // how many featured photos should be selected
    private const int FEATURED_COUNT = 5;

    private const float STICK_THRESHOLD = 0.5f; // how far stick to be pulled to move
    private const float STICK_COOLDOWN = 0.2f; // time in seconds between each photo change when moving

    // USER INPUTS

    // AUDIO

    // GAME COMPONENTS

    [SerializeField] private GameObject canvas;
    [SerializeField] private GameObject sendButton;
    [SerializeField] private GameObject photoTemplate;
    [SerializeField] private GameObject photoList;

    [SerializeField] private GameObject bigPhotoPreview;

    [SerializeField] private TMP_Text countText;

    private List<GameObject> photoPreviews;
    private GridLayoutGroup listLayout;

    
    // VARIABLES

    // how many columns there are in the list of photos
    private int listColumns;


    private int totalPhotos;

    private int targetPhotoCount;

    // photo ids are 0 through the max in the 
    private int currPhotoId;
    // list of integers of selected photos
    private List<int> selectedPhotos;
    private int selectedCount;



    private float last_stick;

    
    // definitely give each object a text child for the score that will be set when creating this list

    // LOGGING


    public bool attemptSubmit(bool submitInput)
    {
        return submitInput && (selectedCount == targetPhotoCount);
    }

    public List<int> getFeaturedIds()
    {
        return selectedPhotos;
    }

    public void toggleCanvas(bool enabled)
    {
        canvas.SetActive(enabled);
    }

    // initialize the end sequence
    public void Init(List<Texture2D> gamePhotos)
    {
        // get how many columns are in the list
        listLayout = photoList.GetComponent<GridLayoutGroup>();
        listColumns = listLayout.constraintCount;

        // get how many photos were taken
        totalPhotos = gamePhotos.Count;
        targetPhotoCount = Math.Min(totalPhotos, FEATURED_COUNT);

        selectedPhotos = new List<int>();
        // set current selected photo
        currPhotoId = 0;
        // create the list of photo objects
        photoPreviews = new List<GameObject>();
        populateList(gamePhotos);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        countText.text = string.Concat(selectedCount, "/", targetPhotoCount, " Photos Selected");

        setBigPhotoPreview(currPhotoId);


        if (selectedCount == targetPhotoCount)
        {
            sendButton.transform.Find("SendActive").gameObject.SetActive(true);
        }
        else
        {
            sendButton.transform.Find("SendActive").gameObject.SetActive(false);
        }


        // HANDLE ENLARGING SELECTED PHOTO HERE (maybe)

        // remaining considerations:
        // unselecting photos from list, not necessarily popping from end
        // not allowing choosing the same photo that is already selected
        // greying out selected photos in the list view, and restoring colour when unselected

        // need to decide if the controller is purely cosmetic or if it holds the logic for selected photos
    }

    // Add photos to the list
    // still should take scores to add to text children of photos
    private void populateList(List<Texture2D> gamePhotos)
    {
        Debug.Log("NEXT");
        Debug.Log(gamePhotos.Count);
        int i = 0;
        foreach (Texture2D photo_texture in gamePhotos)
        {
            i += 1;
            Debug.Log(i);
            GameObject photo_obj = Instantiate(photoTemplate, photoList.transform);
            RawImage image_obj = photo_obj.transform.Find("Image").GetComponent<RawImage>();
            image_obj.texture = photo_texture;
            photoPreviews.Add(photo_obj);
        }
    }

    public void changeSelection(Vector2 input)
    {
        if (Time.time - last_stick < STICK_COOLDOWN)
        {
            return;
        }

        last_stick = Time.time;

        toggleGlow(false, currPhotoId);

        // change to a photo horizontally
        // if the x direction is significant
        if (Math.Abs(input.x) > STICK_THRESHOLD)
        {
            int idx_in_row = currPhotoId % listColumns; // which column we are in
            int final_idx_of_row = (currPhotoId / listColumns != (totalPhotos - 1) / listColumns) ? listColumns - 1 : (totalPhotos - 1) % listColumns; // the last index in the current row (could be less in final row)
            // i.e. it is always list columns 1 but may be less if we are in the final row (case 2)
            int new_idx_in_row = Math.Clamp(idx_in_row + (input.x > 0 ? 1 : -1), 0, final_idx_of_row);
            currPhotoId += new_idx_in_row - idx_in_row;
        }

        // change to a photo vertically
        if (Math.Abs(input.y) > STICK_THRESHOLD)
        {
            // negative y is down, and photo id increases downwards.
            int idx_in_cols = currPhotoId / listColumns; // which row we are in
            int last_row_for_column = (totalPhotos - 1 - (currPhotoId % listColumns)) / listColumns; // the last row in the current column
            int new_idx_in_cols = Math.Clamp(idx_in_cols + (input.y > 0 ? -1 : 1), 0,  last_row_for_column);
            currPhotoId += listColumns * (new_idx_in_cols - idx_in_cols);
        }

        toggleGlow(true, currPhotoId);
    }

    public void selectCurrentPhoto()
    {   
        // can't select the current photo if it is currently selected or we are at max
        if (selectedPhotos.Contains(currPhotoId) || selectedCount >= targetPhotoCount)
        {
            return;
        }

        selectedCount += 1;

        // otherwise, select it
        selectedPhotos.Add(currPhotoId);
        dimPhoto(currPhotoId);
    }

    public void deselectCurrentPhoto()
    {
        // can't do this if the current photo isn't selected
        if (!selectedPhotos.Contains(currPhotoId))
        {
            return;
        }

        selectedCount -= 1;

        // remove the current photo
        selectedPhotos.Remove(currPhotoId);
        brightenPhoto(currPhotoId);
    }

    void brightenPhoto(int id)
    {
        GameObject photo = photoPreviews[id];
        CanvasGroup canvas_group = photo.GetComponent<CanvasGroup>();
        canvas_group.alpha = 1.0f;
    }

    void dimPhoto(int id)
    {
        GameObject photo = photoPreviews[id];
        CanvasGroup canvas_group = photo.GetComponent<CanvasGroup>();
        canvas_group.alpha = 0.5f;
    }

    void setBigPhotoPreview(int id)
    {
        RawImage target_img = photoPreviews[id].transform.Find("Image").GetComponent<RawImage>();
        RawImage big_preview_img = bigPhotoPreview.transform.Find("Image").GetComponent<RawImage>();
        big_preview_img.texture = target_img.texture;
    }

    void toggleGlow(bool enabled, int id)
    {
        GameObject glow = photoPreviews[id].transform.Find("Glow").gameObject;
        glow.SetActive(enabled);
    }
}
