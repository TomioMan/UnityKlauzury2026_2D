using UnityEngine;
using UnityEngine.InputSystem;

public class CarMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float accelerationX = 10f; // Changed to acceleration for "draggy" feel
    [SerializeField] private float accelerationY = 10f;
    [SerializeField] private float returnForce = 1f;
    [SerializeField] private float yLimit = 3f;      // Now limiting X drift

    [Header("Visual Settings")]
    [SerializeField] private float leanAmount = 15f;
    [SerializeField] private float leanSpeed = 5f;

    private Rigidbody2D rb;
    private float moveX;
    private float moveY;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.linearDamping = 2f; // Creates the "slow down" effect
        rb.gravityScale = 0f;
    }

    void Update()
    {
        moveX = 0;
        moveY = 0;

        if (Keyboard.current.wKey.isPressed) moveY = 1;
        else if (Keyboard.current.sKey.isPressed) moveY = -1;

        if (Keyboard.current.aKey.isPressed) moveX = -1;
        else if (Keyboard.current.dKey.isPressed) moveX = 1;

        Leaning();
        ClampPosition();
    }

    void FixedUpdate()
    {
        rb.AddForce(new Vector2(moveX * accelerationX, 0));

        rb.AddForce(new Vector2(0, moveY * accelerationY));

        // nudge 
        if (moveX == 0)
        {
            float distFromCenterX = transform.position.x;
            rb.AddForce(new Vector2( (-distFromCenterX / 8) * returnForce, 0));
        }
        if (moveY == 0)
        {
            float distFromCenterY = transform.position.y;
            rb.AddForce(new Vector2(0, -distFromCenterY * returnForce));
        }
    }

    void Leaning()
    {
        float targetRotation = -rb.linearVelocity.x * leanAmount / 5f;

        // mmmm... math I dont understand...
        Quaternion targetRot = Quaternion.Euler(0, 0, targetRotation);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, Time.deltaTime * leanSpeed);
    }

    void ClampPosition()
    {
        float clampedY = Mathf.Clamp(transform.position.y, -yLimit, yLimit);
        transform.position = new Vector3(transform.position.x , clampedY, transform.position.z);
    }
}