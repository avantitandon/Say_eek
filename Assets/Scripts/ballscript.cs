using UnityEngine;

public class ballscript : MonoBehaviour
{
    public Material inactive;
    public Material active;

    bool spinning = false;
    float spinttime = 0;


    private MeshRenderer mesh;

    public GameObject hat;
    float xRotation = 0;
    float yRotation = 0;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mesh = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!spinning)
        {
            if (Time.time > spinttime + 6)
            {
                mesh.material = active;
                spinttime = Time.time;
                spinning = true;
            }
        }
        else
        {
            if (Time.time > spinttime + 2)
            {
                mesh.material = inactive;
                spinttime = Time.time;
                spinning = false;
            }

            yRotation += 1f;
            hat.transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
        }
    }
}
