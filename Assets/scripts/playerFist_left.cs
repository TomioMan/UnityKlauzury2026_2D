using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerFist_left : MonoBehaviour
{
    [SerializeField] Animator animator;
    public bool PunchingL = false;
    public bool BlockingL = false;

    private float lastPunchTime;
    [SerializeField] float punchCooldown = 0.5f;

    private Coroutine blockRoutine;

    void Update()
    {
        if (animator == null) return;
        if (Keyboard.current.dKey.wasPressedThisFrame &&
            Time.time >= lastPunchTime + punchCooldown)
        {
            lastPunchTime = Time.time;
            StartCoroutine(DelayedPunch());
        }

        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            blockRoutine = StartCoroutine(DelayedBlock());
        }

        if (Keyboard.current.fKey.wasReleasedThisFrame)
        {
            StopBlocking();
        }
    }

    IEnumerator DelayedBlock()
    {
        animator.SetBool("blockingL", true);
        yield return new WaitForSeconds(0.1f);
        BlockingL = true;
    }

    void StopBlocking()
    {
        if (blockRoutine != null) StopCoroutine(blockRoutine);

        BlockingL = false;
        animator.SetBool("blockingL", false);
    }

    IEnumerator DelayedPunch()
    {
        animator.SetBool("punchingL", true);
        yield return new WaitForSeconds(0.2f);
        PunchingL = true;
        yield return new WaitForSeconds(0.1f);
        PunchingL = false;
        animator.SetBool("punchingL", false);
    }
}