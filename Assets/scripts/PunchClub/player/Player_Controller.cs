using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player_Controller : MonoBehaviour
{
    [Header("PLAYER STATS")]
    [SerializeField] public int health = 20;
    [SerializeField] private int maxHealth = 20;

    [SerializeField] public float stamina = 20f; // Changed to float for smoother depletion
    [SerializeField] public float maxStamina = 20f;

    [Header("AUDIO")]
    public AudioSource staminaSource1; // Breath 1
    public AudioSource staminaSource2; // Breath 2 (Heavier)
    public List<AudioClip> staminaClips; // Index 0 = Normal, Index 1 = Heavy

    [SerializeField] private GameObject bloodSplatterObject;
    [SerializeField] PlayerFist_right fistR;
    [SerializeField] PlayerFist_left fistL;
    private SpriteRenderer spriteRend;
    private Image uiImage;

    void Start()
    {
        // Get blood components
        if (bloodSplatterObject != null)
        {
            spriteRend = bloodSplatterObject.GetComponent<SpriteRenderer>();
            uiImage = bloodSplatterObject.GetComponent<Image>();
        }

        // Setup Audio Sources to loop forever
        SetupStaminaAudio();
    }

    void SetupStaminaAudio()
    {
        if (staminaClips.Count >= 2)
        {
            staminaSource1.clip = staminaClips[0];
            staminaSource2.clip = staminaClips[1];
            staminaSource1.loop = true;
            staminaSource2.loop = true;
            staminaSource1.volume = 0;
            staminaSource2.volume = 0;
            staminaSource1.Play();
            staminaSource2.Play();
        }
    }

    void Update()
    {
        UpdateBloodOverlay();
        UpdateStaminaAudio();

        // Optional: Slowly recover stamina over time
        if (stamina < maxStamina && IsBlocking() == false)
        {
            stamina += Time.deltaTime;
            Debug.Log("Stamina+: " + stamina);
        }
    }

    bool IsBlocking()
    {
        if (fistR.BlockingR != true) return false;
        else if (fistL.BlockingL != true) return false;
        else return true;
    }

    // playerController.UseStamina();
    public void UseStamina(float amount)
    {
        stamina -= amount;
        if (stamina < 0) stamina = 0;
    }

    void UpdateStaminaAudio()
    {
        float staminaPercent = stamina / maxStamina; // 1.0 is full, 0 is empty
        float intensity = 1.0f - staminaPercent;    // 0 is full, 1.0 is empty

        if (staminaPercent > 0.5f)
        {
            // Phase 1: Fade in Sound 1, Sound 2 stays quiet
            staminaSource1.volume = intensity;
            staminaSource2.volume = 0;
        }
        else
        {
            // Phase 2: Fade out Sound 1, Fade in Sound 2
            staminaSource1.volume = staminaPercent;
            staminaSource2.volume = intensity;
        }
    }

    void UpdateBloodOverlay()
    {
        if (bloodSplatterObject == null) return;

        float healthPercent = (float)health / maxHealth;
        float targetAlpha = 1.0f - healthPercent;

        if (spriteRend != null)
        {
            Color c = spriteRend.color;
            c.a = targetAlpha;
            spriteRend.color = c;
        }

        if (uiImage != null)
        {
            Color c = uiImage.color;
            c.a = targetAlpha;
            uiImage.color = c;
        }

        bloodSplatterObject.SetActive(health < maxHealth);
    }
}