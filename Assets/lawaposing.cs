using UnityEngine;

public class lawaposing : MonoBehaviour
{
    Animator animator;
    bool posing = false;
    public Transform fih;
    public Transform player;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CameraUpScript.CameraUP += sayEEK;
        CameraUpScript.CameraDOWN += fihing;
        animator = GetComponent <Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (posing)
        {
            animator.SetBool("posing", true);

        }
        else
        {
            animator.SetBool("posing", false);
           
        }
    }

    private void sayEEK()
    {
        posing = true;
        transform.LookAt(player.transform.position);
    }

    private void fihing()
    {
        posing = false;
        transform.LookAt(fih.transform.position);
        
    }

}

