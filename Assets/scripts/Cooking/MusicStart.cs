using UnityEngine;

public class MusicStart : MonoBehaviour
{
    [Header("AUDIO SETTINGS")]
    public AudioSource audioSource;
    public AudioClip audioClip; // Input your audio file here!

    [Range(0f, 0.5f)]
    public float overlapAmount = 0.04f; // Seconds to start the next loop early

    private double nextStartTime;
    private float clipLength;

    void Start()
    {
        // 1. Validation: Make sure we have a source and a clip
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        if (audioClip == null)
        {
            Debug.LogError("Please assign an Audio Clip to the OverlapLooper script!");
            return;
        }

        // 2. Setup initial timing
        clipLength = audioClip.length;
        audioSource.loop = false; // Disable built-in looping
        audioSource.clip = audioClip; // Assign the clip to the source

        // 3. Start the first loop
        nextStartTime = AudioSettings.dspTime + 0.1;
        audioSource.PlayScheduled(nextStartTime);
    }

    void Update()
    {
        if (audioClip == null) return;

        // Check if it's time to schedule the next overlap
        // We look ahead by 0.1s to ensure the audio engine has time to prepare
        if (AudioSettings.dspTime > nextStartTime + (clipLength - overlapAmount) - 0.1)
        {
            // Calculate exactly when the next loop starts
            nextStartTime = nextStartTime + (clipLength - overlapAmount);

            // Play the clip again on the same source
            // PlayOneShot allows the 'tail' of the last loop to finish during the overlap
            audioSource.PlayOneShot(audioClip);
        }
    }
}