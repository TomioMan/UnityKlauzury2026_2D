using UnityEngine;

public class MemoryPickup : MonoBehaviour
{
    public string GameObjectName;
    public ObjectSaving objectSaving; // Drag your SO here

    void Start()
    {
        // If we already interacted with this, disappear!
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