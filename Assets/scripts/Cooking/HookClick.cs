using UnityEngine;
using UnityEngine.InputSystem;

public class HookClick : MonoBehaviour
{
    [Header("SETTINGS")]
    public Vector3 spawnOffset = new Vector3(0, -1.5f, 0);
    public float clickRadius = 0.2f;

    void Update()
    {
        // Detect Mouse Click
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            mousePos.z = 0;

            float distance = Vector2.Distance(mousePos, transform.position);

            if (distance <= clickRadius)
            {
                TeleportSpatula();
            }
        }
    }

    private void TeleportSpatula()
    {
        GameObject spatula = GameObject.FindGameObjectWithTag("MixingSpoon");

        if (spatula != null)
        {
            spatula.transform.position = transform.position + spawnOffset;

            spatula.transform.rotation = Quaternion.Euler(0, 0, 0);

            if (spatula.TryGetComponent<Rigidbody2D>(out var rb))
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;

                rb.rotation = 0f;

            }
        }
    }

    // Visualizes the click area in the editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, clickRadius);
    }
}