using System.Collections;
using UnityEngine;

public class PunchBag : MonoBehaviour
{
    [Header("LINKIGN OBJECTS")]
    [SerializeField] Player_Controller Player;
    [SerializeField] CameraEffects camEffects;
    [SerializeField] PlayerFist_left leftFist;
    [SerializeField] PlayerFist_right rightFist;

    [SerializeField] Animator animator;

    public int health = 20;
    [SerializeField] float cycleDuration = 3.0f;
    private float currentTime;

    private bool wasPunchingL;
    private bool wasPunchingR;

    private float lastPlayerPunchTime;
    public bool canPunch;

    void Start()
    {
        currentTime = cycleDuration;
    }

    void Update()
    {
        UpdateCanPunchStatus();

        if (leftFist.PunchingL && !wasPunchingL)
        {
            StartCoroutine(HitLeft());
        }
        wasPunchingL = leftFist.PunchingL;

        if (rightFist.PunchingR && !wasPunchingR)
        {
            StartCoroutine(HitRight());
        }
        wasPunchingR = rightFist.PunchingR;


        currentTime -= Time.deltaTime;
        if (currentTime <= 0)
        {
            int randomNumber = Random.Range(1, 11);

            if (randomNumber <= 5)
            {
                StartCoroutine(PunchRight());
            }
            else
            {
                StartCoroutine(PunchLeft());
            }

            currentTime = cycleDuration;
        }
    }

    IEnumerator HitRight()
    {
        animator.SetBool("HitRight", true);
        health--;
        yield return new WaitForSeconds(0.1f);
        animator.SetBool("HitRight", false);
    }

    IEnumerator HitLeft()
    {
        animator.SetBool("HitLeft", true);
        health--;
        yield return new WaitForSeconds(0.1f);
        animator.SetBool("HitLeft", false);
    }

    IEnumerator PunchRight()
    {
        animator.SetBool("PunchRight", true);
        yield return new WaitForSeconds(0.2f);
        animator.SetBool("PunchRight", false);
        yield return new WaitForSeconds(0.5f);

        if (!rightFist.BlockingR && canPunch) //on punch
        {
            Player.doDamage(1);
            camEffects.PlayHitEffect(0.075f ,0.2f);
        }
        else if (rightFist.BlockingR && canPunch) // on block
        {
            Player.UseStamina(1);
            camEffects.PlayHitEffect(0.05f, 0.1f);
        }
    }

    IEnumerator PunchLeft()
    {
        yield return new WaitForSeconds(0.2f);
        animator.SetBool("PunchLeft", true);
        yield return new WaitForSeconds(0.2f);
        animator.SetBool("PunchLeft", false);
        yield return new WaitForSeconds(0.5f);

        if (!leftFist.BlockingL && canPunch) //on punch
        {
            Player.doDamage(1);
            camEffects.PlayHitEffect(0.075f, 0.2f);
        }
        else if (leftFist.BlockingL && canPunch) // on block
        {
            Player.UseStamina(1);
            camEffects.PlayHitEffect(0.05f, 0.1f);
        }
    }
    void UpdateCanPunchStatus()
    {
        if (leftFist.PunchingL || rightFist.PunchingR)
        {
            lastPlayerPunchTime = Time.time;
        }

        if (Time.time - lastPlayerPunchTime <= 0.8f) // ← time window to cancel a punch
        {
            canPunch = false;
        }
        else
        {
            canPunch = true;
        }
    }
}