using UnityEngine;
using UnityEngine.InputSystem;

public class TopDownMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float rotationSpeed = 8f; // How fast the sprite turns
    [SerializeField] private Rigidbody2D rb;

    private Vector2 movementInput;

    void Update()
    {
        float moveX = 0;
        float moveY = 0;

        if (Keyboard.current.wKey.isPressed) moveY = 1;
        else if (Keyboard.current.sKey.isPressed) moveY = -1;

        if (Keyboard.current.aKey.isPressed) moveX = -1;
        else if (Keyboard.current.dKey.isPressed) moveX = 1;

        movementInput = new Vector2(moveX, moveY).normalized;

        // NEW: If we are moving, rotate to the direction
        if (movementInput != Vector2.zero)
        {
            // Calculate the angle in degrees
            float targetAngle = Mathf.Atan2(movementInput.y, movementInput.x) * Mathf.Rad2Deg;

            // Adjust by -90 if your sprite is drawn facing "Up"
            // If your sprite faces "Right" by default, you don't need the -90
            Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle - 90);

            // Smoothly rotate the object
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = movementInput * moveSpeed;
    }
}