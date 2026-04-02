using UnityEngine;
using UnityEngine.UI; // Still needed if your GameObject uses an Image component

public class Player_Controller : MonoBehaviour
{
    [SerializeField] public int health = 20;
    [SerializeField] private int maxHealth = 20;

    // Drag your BloodSplatter GameObject here
    [SerializeField] private GameObject bloodSplatterObject;

    private SpriteRenderer spriteRend;
    private Image uiImage;

    void Start()
    {

    }

    void Update()
    {
        UpdateBloodOverlay();
    }

    void UpdateBloodOverlay()
    {
        if (bloodSplatterObject == null) return;

        // 1. Calculate health percentage
        float healthPercent = (float)health / maxHealth;
        float targetAlpha = 1.0f - healthPercent;

        // 2. Apply Alpha to a SpriteRenderer (World Object)
        if (spriteRend != null)
        {
            Color c = spriteRend.color;
            c.a = targetAlpha;
            spriteRend.color = c;
        }

        // 3. Apply Alpha to a UI Image (Canvas Object)
        if (uiImage != null)
        {
            Color c = uiImage.color;
            c.a = targetAlpha;
            uiImage.color = c;
        }

        // Optional: Turn the whole object off if health is full to save performance
        bloodSplatterObject.SetActive(health < maxHealth);
    }
}