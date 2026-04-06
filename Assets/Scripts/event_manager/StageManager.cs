using UnityEngine;

public class StageManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EventManager.StageDomSolo += DjDom;
        EventManager.StagePlusIdol += plusIdol;
        EventManager.StagePlusBhaddie += plusBhaddie;
    }

    // Update is called once per frame
    private void DjDom()
    {
        //put dj dom  here 
        Debug.Log("(em)(stage)DJ dom in da house!!!");
    }
    private void plusIdol()
    {
        Debug.Log("(em)(stage)idol is here!!!!");
    }
    private void plusBhaddie()
    {
        Debug.Log("(em)(stage)caught outside!!!!");
    }
}
