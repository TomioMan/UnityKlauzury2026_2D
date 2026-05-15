using UnityEngine;

public class MemoryPickup : MonoBehaviour
{
    public string GameObjectName;
    public ObjectSaving objectSaving; // Drag your SO here

    void Start()
    {
        // Check if the reference is missing before trying to use it
        if (objectSaving == null)
        {
            Debug.LogError($"ObjectSaving is MISSING on {gameObject.name}! Drag the SO into the inspector.");
            return;
        }

        if (objectSaving.CheckInteraction(GameObjectName))
        {
            Destroy(this.gameObject);
        }
    }

    // Call this function when the player interacts/picks it up
    public void OnInteracted()
    {
        objectSaving.SaveInteraction(GameObjectName, true);

        // Add your inventory logic here...

        Destroy(this.gameObject);
    }
}