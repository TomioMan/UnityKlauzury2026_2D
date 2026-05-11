using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarControl : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float accelerationX = 10f;
    [SerializeField] private float accelerationY = 10f;
    [SerializeField] private float returnForce = 5f;
    [SerializeField] private float yLimit = 3f;

    [Header("Return Target")]
    [SerializeField] private Transform targetPoint; // Drag an empty GameObject here!

    [Header("Visual Settings")]
    [SerializeField] private float leanAmount = 15f;
    [SerializeField] private float leanSpeed = 5f;

    private Rigidbody2D rb;
    private float moveX;
    private float moveY;
    public bool InControl = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.linearDamping = 2f;
        rb.gravityScale = 0f;

        // Fallback: if you forget to assign a target, use the starting position
        if (targetPoint == null)
        {
            GameObject tempTarget = new GameObject("Auto_TargetPoint");
            tempTarget.transform.position = Vector3.zero;
            targetPoint = tempTarget.transform;
            Debug.LogWarning("No Target Point assigned to Car! Defaulting to (0,0)");
        }
    }

    void Update()
    {
        moveX = 0;
        moveY = 0;

        if (InControl)
        {
            if (Keyboard.current.wKey.isPressed) moveY = 1;
            else if (Keyboard.current.sKey.isPressed) moveY = -1;

            if (Keyboard.current.aKey.isPressed) moveX = -1;
            else if (Keyboard.current.dKey.isPressed) moveX = 1;
        }


        Leaning();
        ClampPosition();
    }

    void FixedUpdate()
    {
        rb.AddForce(new Vector2(moveX * accelerationX, moveY * accelerationY));

        // Return logic based on the targetPoint's position
        if (moveX == 0)
        {
            // Calculate distance from target instead of from 0
            float distFromTargetX = transform.position.x - targetPoint.position.x;
            rb.AddForce(new Vector2((-distFromTargetX / 8) * returnForce, 0));
        }

        if (moveY == 0)
        {
            // Calculate distance from target instead of from 0
            float distFromTargetY = transform.position.y - targetPoint.position.y;
            rb.AddForce(new Vector2(0, -distFromTargetY * returnForce));
        }
    }

    void Leaning()
    {
        float targetRotation = -rb.linearVelocity.x * leanAmount / 5f;
        Quaternion targetRot = Quaternion.Euler(0, 0, targetRotation);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, Time.deltaTime * leanSpeed);
    }

    void ClampPosition()
    {
        // Adjust the clamp to be relative to the target's Y position
        float minY = targetPoint.position.y - yLimit;
        float maxY = targetPoint.position.y + yLimit;

        float clampedY = Mathf.Clamp(transform.position.y, minY, maxY);
        transform.position = new Vector3(transform.position.x, clampedY, transform.position.z);
    }

    // This makes it easy to see the center in the Scene View
    private void OnDrawGizmos()
    {
        if (targetPoint != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(targetPoint.position, new Vector3(1f, yLimit * 2, 0.1f));
        }
    }

    public void VehicleGoTo(float x, float y)
    {
        Vector2 position = new Vector2(x, y);

        // Now use 'position' normally
        if (targetPoint != null)
        {
            targetPoint.position = new Vector3(position.x, position.y, targetPoint.position.z);
        }
    }



    public bool isJittering = false;
    private float baseJitterRangeX = 2f;
    private float intensityMultiplier = 2f; // How much the range grows every 5 rounds

    private int roundCounter = 0;
    private float currentScale = 1.0f;

    public void StartJittering()
    {
        if (!isJittering)
        {
            isJittering = true;
            roundCounter = 0;
            currentScale = 1.0f;
            StartCoroutine(JitterRoutine());
        }
    }

    private IEnumerator JitterRoutine()
    {
        while (isJittering)
        {
            // 1. Calculate the scaled range
            float scaledX = baseJitterRangeX * currentScale;

            // 2. Pick a random spot within the NEW scaled range
            float randomX = Random.Range(-scaledX, scaledX);

            // 3. Move the target
            VehicleGoTo(randomX, 0);

            // 4. Update the round counter
            roundCounter++;

            // 5. Every 5 rounds, increase the randomness
            if (roundCounter % 5 == 0)
            {
                currentScale *= intensityMultiplier;
                Debug.Log($"Chaos Level Up! Current Scale: {currentScale}");
            }

            yield return new WaitForSeconds(2f);
        }
    }
}