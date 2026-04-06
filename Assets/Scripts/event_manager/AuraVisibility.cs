using UnityEngine;

public class AuraVisibility : MonoBehaviour
{   public AlphaDithering AlphaDithering;
    public float spectrawareness;
    public Renderer rend;
    public Material newMaterial;
    public Material oldMaterial;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //GetComponent<Renderer>().sharedMaterial = newMaterial;
        AlphaDithering = AlphaDithering.GetComponent<AlphaDithering>();
        rend = GetComponent<Renderer>();
        //rend.material = Resources.Load("ghost_aura")as Material;
        GetComponent<Renderer>().sharedMaterial = newMaterial;

        
    }

    // Update is called once per frame
    void Update()
    {   
        //rend.sharedMaterial.shader = Shader.Find("_spectrawareness");
        spectrawareness = AlphaDithering.spectrawareness;
        //rend.sharedMaterial.SetFloat("_spectrawareness",spectrawareness);
        if (spectrawareness == 1.0)
        {
            rend.material = newMaterial;
        }
        else
        {   
            rend.material = oldMaterial;
            return;
        }
    }
}
