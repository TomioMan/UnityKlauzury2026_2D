using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerFist_right : MonoBehaviour
{
    [SerializeField] Animator animator;

    public bool PunchingR = false;
    public bool BlockingR = false;

    private float lastPunchTime;
    [SerializeField] float punchCooldown = 0.5f;

    private Coroutine blockRoutine;

    void Update()
    {
        if (animator == null) return;

        if (Keyboard.current.kKey.wasPressedThisFrame &&
            Time.time >= lastPunchTime + punchCooldown)
        {
            lastPunchTime = Time.time;
            StartCoroutine(DelayedPunch());
        }

        if (Keyboard.current.jKey.wasPressedThisFrame)
        {
            blockRoutine = StartCoroutine(DelayedBlock());
        }

        if (Keyboard.current.jKey.wasReleasedThisFrame)
        {
            StopBlocking();
        }
    }

    IEnumerator DelayedBlock()
    {
        animator.SetBool("blockingR", true);
        yield return new WaitForSeconds(0.075f);
        BlockingR = true;
    }

    void StopBlocking()
    {
        if (blockRoutine != null) StopCoroutine(blockRoutine);
        BlockingR = false;
        animator.SetBool("blockingR", false);
    }

    IEnumerator DelayedPunch()
    {
        animator.SetBool("punchingR", true);
        yield return new WaitForSeconds(0.2f);
        PunchingR = true;
        yield return new WaitForSeconds(0.1f);
        animator.SetBool("punchingR", false);
        yield return new WaitForSeconds(0.2f);
        PunchingR = false;
    }
}