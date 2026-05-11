using UnityEngine;
using System.Collections;

public class Crash : MonoBehaviour
{
    [Header("IMPORTANT")]
    public GameObject blackScreen;
    public AudioSource audioSource;
    public AudioClip crashAudio;
    public bool canCrash = false;
    public Rigidbody2D car;
    public CarControl carConstrol;

    [Header("things to mute")]
    public AudioSource mute1;
    public AudioSource mute2;
    public AudioSource mute3;
    public AudioSource mute4;
    public GameObject UI;

    private Collider2D myCollider;

    void Awake()
    {
        myCollider = GetComponent<Collider2D>();
        if (myCollider != null)
        {
            myCollider.isTrigger = canCrash;
        }
    }

    public void EnableCrashing()
    {
        canCrash = true;
        if (myCollider != null)
        {
            myCollider.isTrigger = true;
        }
        Debug.Log("The wall is now a trigger! Ready to crash.");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (canCrash)
        {
            canCrash = false;

            // Stop the car instantly
            car.linearVelocity = Vector2.zero;
            car.angularVelocity = 0;
            carConstrol.isJittering = false;

            audioSource.PlayOneShot(crashAudio);
            StartCoroutine(FadeInRoutine(blackScreen, 3f));
        }
    }

    private IEnumerator FadeInRoutine(GameObject target, float duration)
    {
        SpriteRenderer sprite = target.GetComponent<SpriteRenderer>();
        if (sprite == null) { target.SetActive(true); yield break; }

        if (UI != null) UI.SetActive(false);

        float startVol1 = (mute1 != null) ? mute1.volume : 0f;
        float startVol2 = (mute2 != null) ? mute2.volume : 0f;
        float startVol3 = (mute3 != null) ? mute3.volume : 0f;
        float startVol4 = (mute4 != null) ? mute4.volume : 0f;

        Color c = sprite.color;
        c.a = 0f;
        sprite.color = c;
        target.SetActive(true);

        float currentTime = 0f;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            float progress = currentTime / duration;
            float newAlpha = Mathf.Lerp(0f, 1f, progress);
            c.a = newAlpha;
            sprite.color = c;

            if (mute1 != null) mute1.volume = Mathf.Lerp(startVol1, 0f, progress);
            if (mute2 != null) mute2.volume = Mathf.Lerp(startVol2, 0f, progress);
            if (mute3 != null) mute3.volume = Mathf.Lerp(startVol3, 0f, progress);
            if (mute4 != null) mute4.volume = Mathf.Lerp(startVol4, 0f, progress);

            yield return null;
        }
        c.a = 1f;
        sprite.color = c;
    }
}