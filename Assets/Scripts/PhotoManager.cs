using UnityEngine;
using System.Collections;
using System.Collections.Generic;


// for summing across lists
using System.Linq;



namespace PhotoSystem
{

using ScoreMap = Dictionary<int, HitData>;

public class PhotoManager : MonoBehaviour
{
    // CONSTANTS //

    // note the length and height of the camera view are both 1f. So 0.5f of the height is half the height.

    // the frequency of rays on the x axis
    private const float X_STEP = 1/30f;
    // the frequency of rays on the y axis
    private const float Y_STEP = 1/20f;
    // the radius of dense rays at the center of the photo
    private const float RADIUS = 0.1f;


    // AUDIO //
    [SerializeField] private AudioManager audioManager;

    // GAME COMPONENTS //

    [SerializeField] public Camera photoCamera;
    [SerializeField] public RenderTexture photoTexture;
    [SerializeField] private ApertureShot apertureFx;

    // VARIABLES //

    // what layers the photo taking mechanism should ignore
    public LayerMask ignoreLayers;


    // LOGGING //

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public (int, Texture2D) TakePhoto()
    {
        if (photoCamera.targetTexture != photoTexture)
        {
            photoCamera.targetTexture = photoTexture;
        }
        

        // "take" the photo (writes to rendertexture)
        photoCamera.Render();

        // play the aperture visual effect
        apertureFx.PlayShutter();

        // Capture the photo (creates 2d texture from current rendertexture)
        Texture2D photo = CapturePhotoTexture();

        // get the photo's score with raytracing
        int score = ComputeScore();

        //ref camer capture method from audio manager
        audioManager.CameraCapture(score);

        return (score, photo);
    }

    private Texture2D CapturePhotoTexture()
    {
        if (photoTexture == null)
        {
            Debug.LogError("photoTexture is null! Assign the RenderTexture in the inspector.");
            return null;
        }

        Texture2D screenshot = new Texture2D(photoTexture.width, photoTexture.height, TextureFormat.RGB24, false);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = photoTexture;
        screenshot.ReadPixels(new Rect(0, 0, photoTexture.width, photoTexture.height), 0, 0);
        screenshot.Apply();
        RenderTexture.active = previous;
        return screenshot;
    }

    // use raycasts out of the photo camera to compute the score of the current view
    private int ComputeScore()
    {
        // coordinates of current ray
        Vector3 pos = new Vector3(0.0f, 0.0f, 0.0f);
        // the center of the camera's view
        Vector3 center = new Vector3(0.5f, 0.5f, 0.0f);
        // the amount of horizontal lengths travelled
        // we are looping through the camera's view through horizontal
        // lines that are Y_STEP apart.
        float x_dist = 0.0f;

        Ray ray;
        RaycastHit hit;

        int score = 0;
        ScoreMap scoreMap = new ScoreMap();

        // while the current coordinates are within the camera's view, send and tally rays
        while (pos.x < 1.0f && pos.y < 1.0f)
        {

            // create the ray pointing towards the current coordinates
            ray = photoCamera.ViewportPointToRay(pos);

            // send the ray, ignoring layers we don't want to hit
            if (Physics.Raycast(ray, out hit, 1000f, layerMask: ~ignoreLayers))
            {

                // the gameobject we hit
                GameObject hitobj = hit.collider.gameObject;

                // if the ray hit the ghost layer
                if (hitobj.layer == LayerMask.NameToLayer("Ghost"))
                {
                    // if the ghost object has a controller, take the score from there
                    if (hitobj.TryGetComponent<GhostController>(out GhostController ghost))
                    {
                        ProcessHit(scoreMap, ghost, pos);
                    }
                }
            }

            // next, compute the coordinates of the next ray

            // if we are near the center of the camera's view, we should be reducing the horizontal
            // step size, to send more rays
            // if ((pos - center).magnitude < radius)

            // add to the horizontal distance travelled (amount of screens) 
            x_dist = x_dist + X_STEP;

            // calculate the current coordinates on the screen
            // e.g. if we have travelled 6.7 screens, horizontally we are 0.7 of the way through the screen right now
            pos.x = x_dist - Mathf.Floor(x_dist);
            // e.g. if we have travelled 6.7 screens, vertically we are on row 6 right now. 
            pos.y = Y_STEP * Mathf.Floor(x_dist);
        }


        PenalizeProximity(scoreMap);

        score = score + GetFinalScore(scoreMap);

        return score;
    }


    // accumulate points contributed from ghost
    void ProcessHit(ScoreMap scoreMap, GhostController ghost, Vector3 pos)
    {
        // if the ghost has been seen before
        if (!scoreMap.ContainsKey(ghost.ID))
        {
            // can cast here because pos is supposed to be ints
            scoreMap.Add(ghost.ID, new HitData(pos.x));

        }
        HitData hitdata = scoreMap[ghost.ID];

        if (pos.x > hitdata.maxX)
        {
            hitdata.maxX = pos.x;
        }
        if (pos.x < hitdata.minX)
        {
            hitdata.minX = pos.x;
        }

        // increment score
        hitdata.score += ghost.baseScore;

        scoreMap[ghost.ID] = hitdata;
    }

    // penalize proximity
    void PenalizeProximity(ScoreMap scoreMap)
    {
        ScoreMap origmap = new ScoreMap(scoreMap);
        foreach (var (id, data) in origmap)
        {
            HitData hitdata = data;
            
            if (hitdata.maxX - hitdata.minX > 0.5f)
            {
                // Debug.Log(Screen.width * 2);
                Debug.Log(hitdata.minX);
                Debug.Log(hitdata.maxX);
                hitdata.score = (int) (hitdata.score * 0.1f);
            }
            scoreMap[id] = hitdata;
        }
    }

    // get final score
    int GetFinalScore(ScoreMap scoreMap)
    {
        return scoreMap.Values.Sum(HitData => HitData.score);
    }
}


// ok so this works magically
// class/struct defined without monobehaviour has to come AFTER
// monobehaviour class
public struct HitData
{
    public HitData(float x)
    {
        minX = x;
        maxX = x;
        score = 0;
    }

    public float minX;
    public float maxX;
    public int score;
}


}