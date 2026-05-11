using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;

public class OutroScript : MonoBehaviour
{
    [Header("SPRITES")]
    public SpriteRenderer targetRenderer;
    public List<Sprite> spriteList;

    [Header("SETTINGS")]
    public float rightOffset = 2f;

    [Header("AUDIO")]
    public AudioSource audioSource;
    public AudioClip clip;

    private Vector3 originalPosition;
    private Coroutine activeMoveRoutine;

    void Awake()
    {
        StartOutroSequence();
        originalPosition = targetRenderer.transform.position;
    }
    private void Start()
    {
        StartOutroSequence();
        originalPosition = targetRenderer.transform.position;
    }

    public void StartOutroSequence()
    {
        StartCoroutine(HardcodedTimelineRoutine());
    }

    private IEnumerator HardcodedTimelineRoutine()
    {
        Debug.Log("Outro Sequence Started");

        SwapAndSlide("page1_0", 3);
        audioSource.PlayOneShot(clip);
        yield return new WaitForSeconds(2f);

        SwapAndSlide("page2_0", 2);
        yield return new WaitForSeconds(3.5f);

        SwapAndSlide("page3_0", 1);
        yield return new WaitForSeconds(1.2f);

        SwapAndSlide("page4_0", 1);
        yield return new WaitForSeconds(2.0f);

        SwapAndSlide("page5_0", 1);
        yield return new WaitForSeconds(2.0f);

        Debug.Log("Outro Sequence Finished");
    }

    public void SwapAndSlide(string spriteName, float slideDuration)
    {
        // Find the sprite
        Sprite newSprite = spriteList.Find(s => s.name == spriteName);

        if (newSprite != null)
        {
            targetRenderer.sprite = newSprite;

            // Set starting position for the slide
            targetRenderer.transform.position = originalPosition + Vector3.right * (rightOffset - 2);

            // Start the slide
            if (activeMoveRoutine != null) StopCoroutine(activeMoveRoutine);
            activeMoveRoutine = StartCoroutine(SlideRoutine(slideDuration));
        }
        else
        {
            Debug.LogWarning($"OutroScript: Sprite '{spriteName}' not found!");
        }
    }

    private IEnumerator SlideRoutine(float slideDuration)
    {
        float elapsed = 0f;
        Vector3 startingPos = targetRenderer.transform.position;

        while (elapsed < slideDuration)
        {
            elapsed += Time.deltaTime;
            float percent = elapsed / slideDuration;
            targetRenderer.transform.position = Vector3.Lerp(startingPos, originalPosition, percent);
            yield return null;
        }

        targetRenderer.transform.position = originalPosition;
    }
}