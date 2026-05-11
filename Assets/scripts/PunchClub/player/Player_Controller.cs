using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

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
            staminaSource2.pitch = 1.1f;
            staminaSource1.Play();
            staminaSource2.Play();
        }
    }

    void Update()
    {
        UpdateBloodOverlay();
        UpdateStaminaAudio();

        /*
        // Optional: Slowly recover stamina over time
         * if (stamina < maxStamina && IsBlocking() == false)
        {
            stamina += Time.deltaTime;
            Debug.Log("Stamina+: " + stamina);
        }
        */
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
        if (stamina > 0)
        {
            stamina--;
            Debug.Log("Stamina: " + stamina);
        }
    }

    public void doDamage(float amount)
    {
        if (health > 0)
        {
            health--;
            Debug.Log($"Health: {health}");
        }

    }

    void UpdateStaminaAudio()
    {
        float staminaPercent = stamina / maxStamina; // 1.0 is full, 0 is empty
        float intensity = 1.0f - staminaPercent;    // 0 is full, 1.0 is empty

        if (staminaPercent > 0.25f)
        {
            staminaSource1.volume = intensity;
            staminaSource2.volume = 0;
        }
        else
        {
            staminaSource1.volume = 0;

            staminaSource2.volume = intensity / 2;
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