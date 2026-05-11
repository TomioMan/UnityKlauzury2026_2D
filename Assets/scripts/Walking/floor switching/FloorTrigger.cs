using UnityEngine;

public class FloorTrigger : MonoBehaviour
{
    [Header("REFERENCES")]
    [SerializeField] private Collider2D playerTrigger; // Drag the Player's trigger HERE
    [SerializeField] private FloorFader floorA;
    [SerializeField] private FloorFader floorB;

    [Header("SETTINGS")]
    public float fadeSpeed = 0.75f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the thing that entered is EXACTLY the collider in our SerializedField
        if (other == playerTrigger)
        {
            SwitchFloors();
        }
    }

    private void SwitchFloors()
    {
        // We check Floor A's active state to decide the direction
        bool isAActive = floorA.gameObject.activeSelf;

        if (isAActive)
        {
            // Transition: A -> B
            floorB.gameObject.SetActive(true);
            StartCoroutine(floorA.FadeRoutine(0f, fadeSpeed));
            StartCoroutine(floorB.FadeRoutine(1f, fadeSpeed));
        }
        else
        {
            // Transition: B -> A
            floorA.gameObject.SetActive(true);
            StartCoroutine(floorB.FadeRoutine(0f, fadeSpeed));
            StartCoroutine(floorA.FadeRoutine(1f, fadeSpeed));
        }
    }
}