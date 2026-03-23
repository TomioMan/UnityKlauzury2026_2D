using UnityEngine;

public class TopDownCamera : MonoBehaviour
{
    [SerializeField] private Transform playerTarget;
    [SerializeField] private float followSpeed = 2f;
    [SerializeField] private float lookAheadDistance = 2f;
    [SerializeField] private Rigidbody2D playerRb;

    private Vector3 offset;

    void Start()
    {
        // Calculate the initial height of the camera (Z axis)
        offset = transform.position - playerTarget.position;
    }

    void LateUpdate()
    {
        if (playerTarget == null) return;

        // 1. Calculate the Look-Ahead offset based on player velocity
        Vector3 lookAheadOffset = (Vector3)playerRb.linearVelocity.normalized * lookAheadDistance;

        // 2. Define the target position (Player Pos + Look-Ahead + Original Height)
        Vector3 targetPosition = playerTarget.position + offset + lookAheadOffset;

        // 3. Smoothly move the camera to that target
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);

        // 4. Force the rotation to stay flat (facing down at the world)
        // This ensures even if the script is on a rotating object, the camera stays level.
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }
}