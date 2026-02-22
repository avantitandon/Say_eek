using UnityEngine;

public class GhostController : MonoBehaviour
{

    // should refactor to properties or serializefield?
    public int ID;
    public bool isCelebrity;
    public int baseScore;

    private static int num_placeholders;
    public bool isPlaceholder;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (isPlaceholder)
        {
            num_placeholders++;
            ID = 100 + num_placeholders;
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}
