using UnityEngine;

public class InventoryPickup : MonoBehaviour
{
    [Header("REFERENCES")]
    public InventoryObject playerInventory; // Drag your Inventory asset here
    public InventoryUIManager uiManager;
    public AlertSystem alertSystem;

    [Header("AUDIO")]
    public AudioSource ZipperSource;
    public AudioClip Zipper;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. Check if the object has the Ingredient script
        if (other.TryGetComponent<Ingredient>(out var ingredientScript))
        {
            // 2. Check if it actually has data assigned
            if (ingredientScript.ingredientData != null)
            {
                playerInventory.AddItem(ingredientScript.ingredientData);
                alertSystem.ShowAlert(ingredientScript.ingredientName + " added to inventory!");
                uiManager.RefreshUI();

                Destroy(other.gameObject);
                ZipperSource.pitch = Random.Range(0.75f, 1.25f);
                ZipperSource.PlayOneShot(Zipper);

                Debug.Log($"Picked up: {ingredientScript.ingredientData.itemName}");
            }
            else
            {
                Debug.LogWarning($"{other.name} has an Ingredient script but no ItemData assigned!");
            }
        }
    }
}