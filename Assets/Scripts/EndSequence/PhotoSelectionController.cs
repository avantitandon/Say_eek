using UnityEngine;
using UnityEngine.UI;

using System.Collections.Generic;

public class PhotoSelectionController : MonoBehaviour
{
    // CONSTANTS

    // how many featured photos should be selected
    private const int FEATURED_COUNT = 5;

    // USER INPUTS

    // AUDIO

    // GAME COMPONENTS

    [SerializeField] private GameObject photoTemplate;
    [SerializeField] private GameObject photoList;
    private List<GameObject> photoPreviews;
    private GridLayoutGroup listLayout;

    
    // VARIABLES

    // how many columns there are in the list of photos
    private int listColumns;


    private int totalPhotos;

    // photo ids are 0 through the max in the 
    private int currPhotoId;
    // list of integers of selected photos
    private List<int> selectedPhotos;
    private int selectedCount;

    
    // definitely give each object a text child for the score that will be set when creating this list

    // LOGGING

    // initialize the end sequence
    void Init(List<Texture2D> gamePhotos)
    {
        // create the list of photo objects
        populateList(gamePhotos);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // get how many columns are in the list
        listLayout = photoList.GetComponent<GridLayoutGroup>();
        listColumns = listLayout.constraintCount;

        selectedPhotos = new List<int>();
        // set current selected photo
        currPhotoId = 0;
    }

    // Update is called once per frame
    void Update()
    {
        // HANDLE ENLARGING SELECTED PHOTO HERE (maybe)

        // remaining considerations:
        // unselecting photos from list, not necessarily popping from end
        // not allowing choosing the same photo that is already selected
        // greying out selected photos in the list view, and restoring colour when unselected

        // need to decide if the controller is purely cosmetic or if it holds the logic for selected photos
    }

    // Add photos to the list
    // still should take scores to add to text children of photos
    void populateList(List<Texture2D> gamePhotos)
    {
        
    }

    void changeSelection(Vector2 input)
    {
        
    }

    void selectCurrentPhoto()
    {   
        // can't select the current photo if it is currently selected or we are at max
        if (selectedPhotos.Contains(currPhotoId) || selectedCount >= FEATURED_COUNT)
        {
            return;
        }

        // otherwise, select it
        selectedPhotos.Add(currPhotoId);
        brightenPhoto(currPhotoId);
    }

    void deselectCurrentPhoto()
    {
        // can't do this if the current photo isn't selected
        if (!selectedPhotos.Contains(currPhotoId))
        {
            return;
        }

        // remove the current photo
        selectedPhotos.Remove(currPhotoId);
        dimPhoto(currPhotoId);
    }

    void brightenPhoto(int id)
    {
        
    }

    void dimPhoto(int id)
    {
        
    }
}
