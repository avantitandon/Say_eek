using UnityEngine;

public class zanimator : MonoBehaviour

{   
    Animator animator;
    Zeehaviours zeehaviours;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {   
        animator = GetComponent<Animator>();
        Zeehaviours.BlowBigHorn += blowbighorn;
        Zeehaviours.BlowLittleHorn += blowlittlehorn;
        Zeehaviours.PanicHorn += panichorn;
        Zeehaviours.SPECIALHORN += specialhorn;
        Zeehaviours.overHERE += overhere;
        Zeehaviours.WelcomeHorn += welcomehorn;
        zeehaviours = GetComponent<Zeehaviours>();

        
    }

    // Update is called once per frame
    void Update()
    {
        if (zeehaviours.Waving)
        {
            animator.SetBool("waving", true);
            if(zeehaviours.Waving == false)
            {
                animator.SetBool("waving", false);
            }
        }
    }

    private void blowbighorn()
    {
        animator.SetTrigger("big_horn");
    }
    private void blowlittlehorn()
    {
        animator.SetTrigger("littlehorn");
    }
    private void panichorn()
    {
        animator.SetTrigger("panic");
    }
    private void specialhorn()
    {
        animator.SetTrigger("specialevent");
    }
    private void overhere()
    {
        animator.SetTrigger("littlehorn");
    }
    private void welcomehorn()
    {
        animator.SetTrigger("welcome");
    }

}
