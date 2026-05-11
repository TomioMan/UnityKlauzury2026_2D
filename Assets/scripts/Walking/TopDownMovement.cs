using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TopDownMovement : MonoBehaviour
{
    [Header("PLAYER MOVEMENT VARIABLES")]
    [SerializeField] private float moveSpeed = 3f;
    private float originalMoveSpeed;
    [SerializeField] private float rotationSpeed = 8f;
    [SerializeField] private float sprintMultiplier = 2f;

    [Header("OTHER THINGS")]
    [SerializeField] private Rigidbody2D rb; 
    [SerializeField] private Animator animator;
    public PositionSaving positionSaving;

    private Vector2 movementInput;

    private void Awake()
    {
        ApplySavedPosition();
        originalMoveSpeed = moveSpeed; 
    }

    void Update()
    {
        float moveX = 0;
        float moveY = 0;

        if (Keyboard.current.wKey.isPressed)
        {
            moveY = 1;
        }
        else if (Keyboard.current.sKey.isPressed)
        {
            moveY = -1;
        }

        if (Keyboard.current.aKey.isPressed)
        {
            moveX = -1;
        }
        else if (Keyboard.current.dKey.isPressed)
        {
            moveX = 1;
        }

        // animation
        if (Keyboard.current.wKey.isPressed ||
            Keyboard.current.aKey.isPressed ||
            Keyboard.current.sKey.isPressed ||
            Keyboard.current.dKey.isPressed)
        {
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }

        // player rotating to when theyre moving
        movementInput = new Vector2(moveX, moveY).normalized;

        if (movementInput != Vector2.zero){

            float targetAngle = Mathf.Atan2(movementInput.y, movementInput.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle - 90);

            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // Sprinting
        if (Keyboard.current.leftShiftKey.isPressed)
        {
            moveSpeed = originalMoveSpeed * sprintMultiplier;
            animator.speed = sprintMultiplier;
        }
        else
        {
            moveSpeed = originalMoveSpeed;
            animator.speed = 1f;
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = movementInput * moveSpeed;

        rb.angularVelocity = 0f;
    }

    public void ApplySavedPosition()
    {
        if (positionSaving == null)
        {
            Debug.LogWarning("No PositionSaving asset linked to Player!");
            return;
        }

        string currentScene = SceneManager.GetActiveScene().name;

        foreach (var entry in positionSaving.saveEntries)
        {
            if (entry.sceneName == currentScene)
            {
                // 1. Teleport the Player
                transform.position = entry.playerPosition;

                // 2. Teleport the Camera
                GameObject mainCam = GameObject.FindGameObjectWithTag("MainCamera");
                if (mainCam != null)
                {
                    // We set X and Y to match the player, but keep the Camera's original Z
                    Vector3 newCamPos = entry.playerPosition;
                    newCamPos.z = mainCam.transform.position.z;

                    mainCam.transform.position = newCamPos;
                }

                Debug.Log($"Teleported Player and Camera to saved position in {currentScene}");

                // Reset physics if necessary
                if (TryGetComponent<Rigidbody2D>(out var rb))
                {
                    rb.linearVelocity = Vector2.zero;
                }

                break;
            }
        }
    }
}