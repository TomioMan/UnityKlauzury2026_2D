using UnityEngine;

public class PunchBag : MonoBehaviour
{
    [SerializeField] PlayerFist_left leftFist;
    [SerializeField] PlayerFist_right rightFist;

    // We use these just to prevent the console from being flooded with 60 messages a second
    private bool wasPunchingL;
    private bool wasPunchingR;

    void Update()
    {
        if (leftFist == null || rightFist == null) return;

        // --- MONITORING LEFT FIST ---
        if (leftFist.PunchingL && !wasPunchingL)
        {
            Debug.Log("Bag hit by LEFT punch!");
        }
        wasPunchingL = leftFist.PunchingL;

        if (leftFist.BlockingL)
        {
            Debug.Log("Left fist is currently BLOCKING near the bag.");
        }


        // --- MONITORING RIGHT FIST ---
        if (rightFist.PunchingR && !wasPunchingR)
        {
            Debug.Log("Bag hit by RIGHT punch!");
        }
        wasPunchingR = rightFist.PunchingR;

        if (rightFist.BlockingR)
        {
            Debug.Log("Right fist is currently BLOCKING near the bag.");
        }
    }
}