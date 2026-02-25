using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerFist : MonoBehaviour
{
    [SerializeField] Animator animator;


    void Update()
    {
        if (animator == null) return;

        if (Keyboard.current.spaceKey.isPressed)
        {
            animator.SetBool("punching", true);
        }
        else
        {
            animator.SetBool("punching", false);
        }
    }
}