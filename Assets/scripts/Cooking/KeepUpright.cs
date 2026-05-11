using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class SelfRighting : MonoBehaviour
{
    public float uprightForce = 10f; // How hard it pulls back
    public float uprightDamping = 1f; // How much it resists "wobbling"

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        // 1. Get the current angle (-180 to 180)
        float currentAngle = transform.eulerAngles.z;
        if (currentAngle > 180f) currentAngle -= 360f;

        // 2. Calculate the "Correction" needed
        // We want to reach 0, so the error is just the negative angle
        float angleError = -currentAngle;

        // 3. Apply torque to move towards 0
        // We subtract the current angular velocity to "dampen" the movement (prevent infinite wobbling)
        float torque = (angleError * uprightForce) - (rb.angularVelocity * uprightDamping);

        rb.AddTorque(torque);
    }
}