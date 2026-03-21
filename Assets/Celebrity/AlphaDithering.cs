using UnityEngine;

public class AlphaDithering : MonoBehaviour
{   
    float time;
    
    public Renderer rend;
    public float spectrawareness = 0f;
    public CameraUpOrNot CameraUpOrNot;
    public GameObject Main_Camera;
    public float alphadithering;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Awake()
    {   
        
        time = 0;
        CameraUpOrNot = Main_Camera.GetComponent<CameraUpOrNot>();

        
        

    }
    // Update is called once per frame
    void Update()
    {
        if (CameraUpOrNot.CameraUp)
        {
            spectrawareness = 1.0f;
            
        }
        else
        {
            spectrawareness = 0.0f;
            return;
        }
        
             /* 
            time += Time.deltaTime;
            if (CameraUpOrNot.CameraUp)
            {
                float x = time; 
                float spectrawareness = Mathf.Sin(time)+1.0f; 


                rend.material.SetFloat("_DitherSize",spectrawareness);
                
            }
            else
            {
                spectrawareness = 0;
                rend.material.SetFloat("_DitherSize",spectrawareness);
                return;
            }*/
    }

    
}
