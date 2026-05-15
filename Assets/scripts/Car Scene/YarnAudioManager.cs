using UnityEngine;
using Yarn.Unity;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using System.Runtime.CompilerServices;

public class YarnAudioManager : MonoBehaviour
{
    [Header("TEXT FIELD")]
    public TextMeshProUGUI dialogueText;
    public float timePerLetter = 0.0185f;

    [Header("VOICE SOUNDS")]
    public AudioSource voiceSource;
    public List<AudioClip> voiceClips;

    [Header("SPRITES")] 
    public SpriteRenderer targetRenderer; // Direct reference to the renderer component
    public List<Sprite> spriteList; 
    public float dropDistance = 1.5f;
    public float slideDuration = 0.5f;
    private Vector3 originalPosition; 
    private Coroutine activeRoutine;

    [Header("ENVIROMENTAL SOUNDS")]
    public AudioSource audioSource;
    public List<AudioClip> audioClips;

    
    [Header("ENVIROMENTAL SOUNDS")]
    public ItemData fatItem;
    public ItemData lyeItem;
    public InventoryObject inventoryObject;
    

    private float remainingTime = 0f;
    private int lastLetterCount = 0;
    private AudioClip currentClip;
    private bool isPlaying = false;

    [Header("OPTIONAL")]
    public Transform targetPoint;
    public CarControl carControl;
    public AlertSystem alertSystem;
    public Crash crashObj1;
    public Crash crashObj2;
    public ProgressSave progressSave;

    private void Awake()
    {
        var runner = FindFirstObjectByType<DialogueRunner>();
        runner.AddCommandHandler<string>("play_voice", PlayVoice);
        // general use
        runner.AddCommandHandler<float>("wait_seconds", WaitCommand);
        runner.AddCommandHandler<string>("play_sound", PlaySound);
        runner.AddCommandHandler<string[]>("alert", Alert);
        // car scene commands
        runner.AddCommandHandler<float, float>("vehicle_go_to", VehicleGoTo);
        runner.AddCommandHandler<bool>("inControl", controlSwitch);
        //dialogue changing sprites
        originalPosition = targetRenderer.transform.position;
        runner.AddCommandHandler<string>("CharSprite", SwapAndPop);
        runner.AddCommandHandler<string>("CharSpriteSwap", SwapSpriteInstant);
        // walking
        runner.AddCommandHandler<int>("nodeIndex", nodeIndex);
        runner.AddFunction("CheckFatHeist", CheckFatHeist);
    }

    public bool CheckFatHeist()
    {
        if (inventoryObject == null || fatItem == null || lyeItem == null) return false;

        bool hasEnoughFat = inventoryObject.HasItem(fatItem, 2);
        bool hasEnoughLye = inventoryObject.HasItem(lyeItem, 2);

        return hasEnoughFat && hasEnoughLye;
    }
    public void nodeIndex(int index)
    {
        progressSave.TylerDialogueIndex = index;
    }

        public void PlayVoice(string clipName)
    {
        currentClip = voiceClips.Find(c => c.name == clipName);
        if (currentClip != null)
        {
            // Removed pitch randomization from here
            remainingTime = 0f;
            lastLetterCount = 0;
            isPlaying = true;
        }
    }

    public void PlaySound(string clipName)
    {
        AudioClip clipToPlay = audioClips.Find(c => c.name == clipName);

        if (clipToPlay != null)
        {
            audioSource.PlayOneShot(clipToPlay);
            Debug.Log("Playing " + clipName);
        }
        else
        {
            Debug.LogWarning("Could not find sound: " + clipName + " in the Environmental Sounds list!");
        }
    }

    public void VehicleGoTo(float x, float y)
    {
        Vector2 position = new Vector2(x, y);

        // Now use 'position' normally
        if (targetPoint != null)
        {
            targetPoint.position = new Vector3(position.x, position.y, targetPoint.position.z);
        }
    }
    public Coroutine WaitCommand(float duration)
    {
        return StartCoroutine(DoWait(duration));
    }

    private IEnumerator DoWait(float duration)
    {
        Debug.Log($"Dialogue paused for {duration} seconds...");

        yield return new WaitForSeconds(duration);

        Debug.Log("Dialogue resuming!");
    }

    public void Alert(params string[] words)
    {
        string finalString = string.Join(" ", words);
        alertSystem.ShowAlert(finalString);
    }

    public void controlSwitch(bool decision)
    {
        carControl.InControl = decision;
        carControl.StartJittering();
        crashObj1.EnableCrashing();
        crashObj2.EnableCrashing();
    }

    public void SwapAndPop(string spriteName)
    {
        // 1. Find the new sprite
        Sprite newSprite = spriteList.Find(s => s.name == spriteName);

        if (newSprite == null)
        {
            Debug.LogWarning($"Sprite named {spriteName} not found!");
            return;
        }

        // 2. Setup initial state: New sprite, Alpha 0, Teleport down
        targetRenderer.sprite = newSprite;

        Color c = targetRenderer.color;
        c.a = 0f;
        targetRenderer.color = c;

        targetRenderer.transform.position = originalPosition + Vector3.down * dropDistance;

        // 3. Start combined Slide and Fade
        if (activeRoutine != null) StopCoroutine(activeRoutine);
        activeRoutine = StartCoroutine(SlideAndFadeRoutine());
    }

    private IEnumerator SlideAndFadeRoutine()
    {
        float currentTime = 0f;
        Vector3 startPos = targetRenderer.transform.position;

        while (currentTime < slideDuration)
        {
            currentTime += Time.deltaTime;
            float progress = currentTime / slideDuration;

            // --- Move Logic ---
            targetRenderer.transform.position = Vector3.Lerp(startPos, originalPosition, progress);

            // --- Fade Logic ---
            Color c = targetRenderer.color;
            c.a = Mathf.Lerp(0f, 1f, progress);
            targetRenderer.color = c;

            yield return null;
        }

        // Snap to final values
        targetRenderer.transform.position = originalPosition;
        Color finalColor = targetRenderer.color;
        finalColor.a = 1f;
        targetRenderer.color = finalColor;
    }
    public void SwapSpriteInstant(string spriteName)
    {
        // 1. Find the sprite in your list by name
        Sprite newSprite = spriteList.Find(s => s.name == spriteName);

        if (newSprite != null)
        {
            // 2. Apply it immediately
            targetRenderer.sprite = newSprite;

            // 3. Ensure alpha is fully visible in case a previous fade left it transparent
            Color c = targetRenderer.color;
            c.a = 1f;
            targetRenderer.color = c;
        }
        else
        {
            Debug.LogWarning($"Instant swap failed: {spriteName} not found!");
        }
    }

    private void Update()
    {
        if (!isPlaying || dialogueText == null || currentClip == null) return;

        // 1. Check how many letters are currently visible
        int currentCount = dialogueText.text.Length;

        // 2. If the count increased, add timePerLetter for every new letter
        if (currentCount > lastLetterCount)
        {
            int newLetters = currentCount - lastLetterCount;
            remainingTime += (newLetters * timePerLetter);
            lastLetterCount = currentCount;

            // NEW: Change the pitch every time a new letter appears (every 0.0185s effectively)
            voiceSource.pitch = Random.Range(0.9f, 1.1f);

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