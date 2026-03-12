
using System.Collections;
using TMPro;
using UnityEngine;

public class BossTextScript : MonoBehaviour
{
    // Message text inside the boss text UI object.
    [SerializeField] private TMP_Text resultsText;
    [SerializeField] private GameObject bossTextRoot;
    private Coroutine activeCoRoutine;

    private void Start()
    {
        if (bossTextRoot != null)
        {
            bossTextRoot.SetActive(false);
        }
    }

    public void SetTextMessage(string message, float duration)
    {
        if (resultsText == null || bossTextRoot == null) return;
        resultsText.text = message;


        if (activeCoRoutine != null)
            StopCoroutine(activeCoRoutine);
        activeCoRoutine = StartCoroutine(ShowBoss(duration));
    }



    private IEnumerator ShowBoss(float duration)
    {
        bossTextRoot.SetActive(true);
        yield return new WaitForSeconds(duration);
        bossTextRoot.SetActive(false);
        activeCoRoutine = null;
    }
}
