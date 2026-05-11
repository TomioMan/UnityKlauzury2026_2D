using UnityEngine;
using TMPro; // Required for TextMeshPro
using System.Collections;

public class AlertSystem : MonoBehaviour
{
    public TextMeshProUGUI alertText;
    private CanvasGroup canvasGroup;
    private Coroutine currentCoroutine;


    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();

        // Ensure it starts invisible
        canvasGroup.alpha = 0;
    }

    // This is the function you call from other scripts!
    public void ShowAlert(string message)
    {
        alertText.text = message;

        // If an alert is already running, stop it and start over
        if (currentCoroutine != null) StopCoroutine(currentCoroutine);

        currentCoroutine = StartCoroutine(AlertRoutine());
    }

    private IEnumerator AlertRoutine()
    {
        // 1. Make visible immediately
        canvasGroup.alpha = 1f;

        // 2. Stay visible for 3 seconds
        yield return new WaitForSeconds(3f);

        // 3. Fade out over 1 second
        float duration = 1f;
        float currentTime = 0f;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, currentTime / duration);
            yield return null;
        }

        canvasGroup.alpha = 0f;
    }
}