using UnityEngine;

public class NPCAnimationAudio : MonoBehaviour
{
    public AudioManager audioManager;
    public Animator animator;
    public void NPCFootstep()
    {
        if (animator == null) //this is regular footstep, if there is no reason for different animation states. Ref in audiomanager as "Defaul"
        {
            audioManager.HandleFootstep(this.gameObject, "Default");
            return;
        }
        
        string currentAnimation = "";

        //this is for when an animation state is needed
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("beezaka idle")) // this is beezaka idle
        {
            currentAnimation = "Idle";
        }
        else
        {
            currentAnimation = "Default";
        }
        
        //this is calling the method IF there is an animator needed for the animation state 
        audioManager.HandleFootstep(this.gameObject, currentAnimation);
    }
    public void ZekeHorn()
    {
        if (animator == null) 
        {
            Debug.Log("Animator is null for NPCAnimationAudio");
            return;
        }
        string currentZekeAnimation = "";

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Zeke Welcome")) 
        {
            currentZekeAnimation = "Zeke Horn";
        }
        

        audioManager.HandleZekeHorn(this.gameObject, currentZekeAnimation);
    }
}
