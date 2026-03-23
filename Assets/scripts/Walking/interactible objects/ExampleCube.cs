using UnityEngine;

public class ExampleCube : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    private Color originalColor;

    void Start()
    {
        // Store the starting color so we can change it back later
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    // This runs when something enters the Trigger area
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the thing that entered has the "Player" tag
        if (other.CompareTag("Player"))
        {
            spriteRenderer.color = Color.red;
            Debug.Log("Player entered! Object is now RED.");
        }
    }

    // This runs when the player walks away
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            spriteRenderer.color = originalColor;
            Debug.Log("Player left! Object returned to original color.");
        }
    }
}
