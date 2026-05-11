using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))] // Ensures the object has physics
public class DraggableItem : MonoBehaviour
{
    private bool isDragging = false;
    private Vector3 offset;
    private Camera mainCamera;
    private Rigidbody2D rb;

    // Velocity Tracking
    private Vector2 lastMousePos;
    private Vector2 currentVelocity;

    void Awake()
    {
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector3 worldMousePos = GetMouseWorldPos(mousePos);

        // 1. START DRAG
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            RaycastHit2D hit = Physics2D.Raycast(worldMousePos, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                isDragging = true;
                offset = transform.position - worldMousePos;
            }
        }

        // 2. WHILE DRAGGING
        if (isDragging)
        {
            transform.position = worldMousePos + offset;

            // Calculate how much the mouse moved since last frame
            // This is the "Throwing" force
            currentVelocity = (worldMousePos - (Vector3)lastMousePos) / Time.deltaTime;
            lastMousePos = worldMousePos;

            // 3. RELEASE AND THROW
            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                isDragging = false;

                // Apply the "Flick" speed to the object
                rb.linearVelocity = currentVelocity / 4;
             
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