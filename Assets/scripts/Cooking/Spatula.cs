using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class Spatula : MonoBehaviour
{
    private bool isDragging = false;
    private Vector3 offset;
    private Camera mainCamera;
    private Rigidbody2D rb;

    [Header("Dragging & Throwing")]
    private Vector2 lastMousePos;
    private Vector2 currentVelocity;
    public float throwForceMultiplier = 0.25f;
    public float dragFollowSpeed = 15f;

    [Header("Hook Magnet Settings")]
    public Transform pinPoint;
    public float attractionRange = 3.0f;
    public float magnetStrength = 25f;

    void Awake()
    {
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
    }

    void Update()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector3 worldMousePos = GetMouseWorldPos(mousePos);

        // 1. PICK UP
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            RaycastHit2D hit = Physics2D.Raycast(worldMousePos, Vector2.zero);
            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                isDragging = true;
                offset = transform.position - worldMousePos;
            }
        }

        // 2. TRACK VELOCITY
        if (isDragging)
        {
            currentVelocity = (worldMousePos - (Vector3)lastMousePos) / Time.deltaTime;
            lastMousePos = worldMousePos;

            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                isDragging = false;
                rb.linearVelocity = currentVelocity * throwForceMultiplier;
            }
        }
    }

    void FixedUpdate()
    {
        if (isDragging)
        {
            Vector3 worldMousePos = GetMouseWorldPos(Mouse.current.position.ReadValue());
            Vector2 targetPos = (Vector2)worldMousePos + (Vector2)offset;
            Vector2 moveDirection = targetPos - rb.position;

            rb.linearVelocity = moveDirection * dragFollowSpeed;
        }

        GameObject[] hooks = GameObject.FindGameObjectsWithTag("Hook");
        foreach (GameObject hook in hooks)
        {
            Vector2 direction = (Vector2)hook.transform.position - (Vector2)pinPoint.position;
            float distance = direction.magnitude;

            if (distance < attractionRange)
            {
                float forceMagnitude = (1f / Mathf.Max(distance, 0.05f)) * magnetStrength;
                Vector2 force = direction.normalized * forceMagnitude;

                rb.AddForceAtPosition(force, pinPoint.position);
            }
        }
    }

    private Vector3 GetMouseWorldPos(Vector2 screenPos)
    {
        Vector3 pos = screenPos;
        pos.z = -mainCamera.transform.position.z;
        return mainCamera.ScreenToWorldPoint(pos);
    }
}