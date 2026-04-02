using UnityEngine;
using UnityEngine.InputSystem;

public class TopDownMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float rotationSpeed = 8f;
    [SerializeField] private Rigidbody2D rb;

    private Vector2 movementInput;

    void Update()
    {
        float moveX = 0;
        float moveY = 0;

        if (Keyboard.current.wKey.isPressed){
            moveY = 1;
        }
        else if (Keyboard.current.sKey.isPressed){
            moveY = -1;
        }

        if (Keyboard.current.aKey.isPressed){
            moveX = -1; 
        }
        else if (Keyboard.current.dKey.isPressed)
        {
            moveX = 1; 
        }

        movementInput = new Vector2(moveX, moveY).normalized;

        if (movementInput != Vector2.zero){

            float targetAngle = Mathf.Atan2(movementInput.y, movementInput.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle - 90);

            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = movementInput * moveSpeed;

        rb.angularVelocity = 0f;
    }
}