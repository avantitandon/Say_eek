using UnityEngine;

public class lightTracking : MonoBehaviour
{
    public Transform celebrity1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
         
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(celebrity1.transform.position);
    }
}
