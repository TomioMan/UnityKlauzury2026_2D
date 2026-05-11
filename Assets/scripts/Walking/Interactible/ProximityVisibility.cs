using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events; // Required for the UnityEvent dropdown
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement; // Using the New Input System like your other scripts

public class ProximityVisibility : MonoBehaviour
{
    private Transform playerTransform;
    private SpriteRenderer spriteRenderer;
    public AlertSystem alertSystem;

    [Header("DISTANCE SETTINGS")]
    public float noOpacityDistance = 3f;
    public float transitionOpacityDistance = 2f;
    private bool canBeViewed = true;

    [Header("INTERACTION")]
    public UnityEvent onInteract;

    [Header("INVENTORY STUFF")]
    public InventoryObject inventoryObject;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;
    }

    void Update()
    {
        if (playerTransform == null) return;

        if (canBeViewed)
        {
            float distance = Vector2.Distance(transform.position, playerTransform.position);
            float alpha = Mathf.InverseLerp(noOpacityDistance, transitionOpacityDistance, distance);

            Color newColor = spriteRenderer.color;
            newColor.a = alpha;
            spriteRenderer.color = newColor;

            if (alpha >= 0.5f)
            {
                if (Keyboard.current.eKey.wasPressedThisFrame)
                {
                    onInteract.Invoke();
                }
            }
        }
    }

    bool canPickUp = true;
    public ObjectSaving objectSaving; // Link the new SO
    public void AddToInvetory(ItemData item)
    {
        if (canPickUp)
        {
            // 1. Check Inventory
            if (inventoryObject == null)
            {
                Debug.LogError($"InventoryObject missing on {gameObject.name}");
                return;
            }

            // 2. Check Memory System
            if (TryGetComponent<MemoryPickup>(out var memory))
            {
                if (objectSaving != null)
                {
                    objectSaving.SaveInteraction(memory.GameObjectName, true);
                }
                else
                {
                    Debug.LogError($"ObjectSaving SO missing on {gameObject.name}!");
                }
            }

            // 3. Check Alert System
            if (alertSystem != null)
            {
                alertSystem.ShowAlert(item.itemName + " added to inventory!");
            }
            else
            {
                Debug.LogWarning($"AlertSystem missing on {gameObject.name}. Item added silently.");
            }

            // Proceed with logic
            inventoryObject.AddItem(item);
            canPickUp = false;
            canBeViewed = false;

            Color newColor = spriteRenderer.color;
            newColor.a = 0;
            spriteRenderer.color = newColor;
        }
    }

    private bool isAlerting = false;

    public void Alert(string commaSeparatedAlerts)
    {
        if (isAlerting) return;

        List<string> alertList = new List<string>(commaSeparatedAlerts.Split(','));

        StartCoroutine(AlertRoutine(alertList));
    }

    private IEnumerator AlertRoutine(List<string> alerts)
    {
        isAlerting = true;

        foreach (string message in alerts)
        {
            if (alertSystem != null)
            {
                alertSystem.ShowAlert(message);
            }

            yield return new WaitForSeconds(3.5f);
        }

        isAlerting = false;
    }

    public PositionSaving positionSaving;
    public void GoToScene(string sceneName)
    {
        Vector3 playerPos = GameObject.FindWithTag("Player").transform.position;
        string currentScene = SceneManager.GetActiveScene().name;

        positionSaving.SavePosition(currentScene, playerPos);
        SceneManager.LoadScene(sceneName);
    }
}