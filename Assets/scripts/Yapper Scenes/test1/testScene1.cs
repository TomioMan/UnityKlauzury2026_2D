using UnityEngine;
using UnityEngine.InputSystem;
using Yarn.Unity;

public class testScene1 : MonoBehaviour
{
    public DialogueRunner dialogueRunner;
    bool started = false;
    void Start()
    {
        
    }

    void Update()
    {
        if (Keyboard.current.eKey.isPressed && !started)
        {
            dialogueRunner.StartDialogue("Test1");
            started = true;
        }
    }
}
