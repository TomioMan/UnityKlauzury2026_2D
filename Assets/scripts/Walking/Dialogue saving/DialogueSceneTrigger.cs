using UnityEngine;
using UnityEngine.SceneManagement;

public class DialogueSceneTrigger : MonoBehaviour
{
    public ProgressSave progressSave;
    public string targetSceneName;
    private Collider2D myCollider;
    private bool isReadyToTeleport = false;

    void Start()
    {
        myCollider = GetComponent<Collider2D>();
        CheckProgress();
    }

    // Run this in Update or call it after your dialogue ends
    void Update()
    {
        CheckProgress();
    }

    void CheckProgress()
    {
        if (progressSave.TylerDialogueIndex >= 3)
        {
            // Change from solid wall to a trigger
            if (myCollider != null && !myCollider.isTrigger)
            {
                myCollider.isTrigger = true;
                isReadyToTeleport = true;
                Debug.Log("Index is 3: Collider is now a Trigger!");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the thing hitting the trigger is the Player
        if (isReadyToTeleport && other.CompareTag("Player"))
        {
            SceneManager.LoadScene(targetSceneName);
        }
    }
}