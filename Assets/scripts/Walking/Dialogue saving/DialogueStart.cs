using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;

public class DialogueStart : MonoBehaviour
{
    public ProgressSave progressSave;
    public DialogueRunner dialogueRunner; 
    public YarnProject autoDialogueProject;

    private void OnEnable()
    {
        StartCoroutine(DelayedStart());
    }

    private IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(0.5f);


        if (SceneManager.GetActiveScene().name == "Walking - House" && progressSave.TylerDialogueIndex == 1)
        {
            TriggerAutomaticDialogue(autoDialogueProject, "Node_1");
            Debug.Log("Conditions for dialogue autostart met.");
        }
    }

    private void TriggerAutomaticDialogue(YarnProject project, string nodeName)
    {
        if (dialogueRunner == null)
        {
            dialogueRunner = FindFirstObjectByType<DialogueRunner>();
        }

        if (dialogueRunner != null && project != null)
        {
            // 1. Tell the runner to use the 'Test1' project
            dialogueRunner.SetProject(project);

            // 2. Only start if it's not already talking
            if (!dialogueRunner.IsDialogueRunning)
            {
                dialogueRunner.StartDialogue(nodeName);
            }
        }
    }
}
