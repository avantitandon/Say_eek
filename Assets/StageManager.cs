using UnityEngine;

public class StageManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EventManager.StageDomSolo += DjDom;
    }

    // Update is called once per frame
    private void DjDom()
    {
        //put dj dom  here 
        Debug.Log("DJ dom in da house!!!");
    }
}
