using UnityEngine;
using Yarn.Unity;
using System.Collections.Generic;
using System.Collections;
using TMPro;

public class YarnAudioManager : MonoBehaviour
{
    public AudioSource voiceSource;
    public List<AudioClip> voiceClips;
    public TextMeshProUGUI dialogueText;

    public float timePerLetter = 0.019f;

    private float remainingTime = 0f;
    private int lastLetterCount = 0;
    private AudioClip currentClip;
    private bool isPlaying = false;

    private void Awake()
    {
        var runner = FindFirstObjectByType<DialogueRunner>();
        runner.AddCommandHandler<string>("play_voice", PlayVoice);
    }

    public void PlayVoice(string clipName)
    {
        currentClip = voiceClips.Find(c => c.name == clipName);
        if (currentClip != null)
        {
            remainingTime = 0f;
            lastLetterCount = 0;
            isPlaying = true;
        }
    }

    private void Update()
    {
        if (!isPlaying || dialogueText == null || currentClip == null) return;

        // 1. Check how many letters are currently visible
        int currentCount = dialogueText.text.Length;

        // 2. If the count increased, add 0.1s for every new letter
        if (currentCount > lastLetterCount)
        {
            int newLetters = currentCount - lastLetterCount;
            remainingTime += (newLetters * timePerLetter);
            lastLetterCount = currentCount;

            // 3. If the audio isn't playing yet, start it
            if (!voiceSource.isPlaying)
            {
                voiceSource.clip = currentClip;
                voiceSource.loop = true;
                voiceSource.Play();
            }
        }

        // 4. Count down the time
        if (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
        }
        else
        {
            // 5. Time is up! Stop the sound
            if (voiceSource.isPlaying)
            {
                voiceSource.Stop();
                isPlaying = false;
            }
        }
    }
}