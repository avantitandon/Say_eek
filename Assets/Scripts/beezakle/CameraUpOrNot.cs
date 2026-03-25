using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class CameraUpOrNot : MonoBehaviour

{   public bool CameraUp = false;
    public CameraController cameraController;
    public GameObject zeke;
    public GameObject beezaka;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        CameraUp = cameraController.IsCameraUp();

    }
}
