using UnityEngine;

public class GazeboManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EventManager.GazeboKissing += BridesKissing;
    }

    // Update is called once per frame
    private void BridesKissing()
    {
        Debug.Log("(em)(gazebo)Brides  are  kissing!!!!");
        
    }
}
