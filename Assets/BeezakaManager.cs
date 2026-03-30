using UnityEngine;

public class BeezakaManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EventManager.BEEZAKAPOCALYPSENOW += BZisNigh;
    }

    // Update is called once per frame
    private void BZisNigh()
    {
        Debug.Log("(em)(bz is nigh)  she is here  ......");
    }
}
