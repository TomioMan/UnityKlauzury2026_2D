using System.Collections;
using UnityEngine;

public class CameraEffects : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float zoomSize = 4.9f;
    [SerializeField] private float defaultSize = 5f;
    [SerializeField] private float shakeIntensity = 0.2f;

    public void PlayHitEffect(float timeBetweenFrames, float shakeIntensity)
    {
        StartCoroutine(CameraImpact(timeBetweenFrames, shakeIntensity));
    }

    IEnumerator CameraImpact(float timeBetweenFrames, float shakeIntensity)
    {
        if (mainCamera == null) mainCamera = GetComponent<Camera>();

        Vector3 originalPos = mainCamera.transform.localPosition;

        // Use orthographicSize instead of fieldOfView
        mainCamera.orthographicSize = zoomSize;

        // Shake logic
        mainCamera.transform.localPosition = originalPos + new Vector3(-shakeIntensity, 0, 0);
        yield return new WaitForSeconds(timeBetweenFrames);

        mainCamera.transform.localPosition = originalPos + new Vector3(shakeIntensity, 0, 0);
        yield return new WaitForSeconds(timeBetweenFrames);

        yield return new WaitForSeconds(timeBetweenFrames);

        // Reset
        mainCamera.transform.localPosition = originalPos;
        mainCamera.orthographicSize = defaultSize;
    }
}