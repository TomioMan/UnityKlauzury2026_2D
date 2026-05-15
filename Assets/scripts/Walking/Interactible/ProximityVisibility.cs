using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Yarn.Unity;

public class ProximityVisibility : MonoBehaviour
{
    private Transform playerTransform;
    private SpriteRenderer spriteRenderer;
    public AlertSystem alertSystem;

    [Header("YARN SPINNER")]
    public DialogueRunner dialogueRunner;
    public ProgressSave progressSave;

    [Header("DISTANCE SETTINGS")]
    public float noOpacityDistance = 3f;
    public float transitionOpacityDistance = 2f;
    private bool canBeViewed = true;

    [Header("INTERACTION")]
    public UnityEvent onInteract;

    [Header("INVENTORY STUFF")]
    public InventoryObject inventoryObject;

    [Header("WIRE CUTTER SETTINGS")]
    public ItemData wireCuttersItem;
    public GameObject objectToActivate;
    [SerializeField] private bool deleteWholeObject = true;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;

        if (dialogueRunner == null) dialogueRunner = FindFirstObjectByType<DialogueRunner>();
    }

    public void StartYarnDialogue(YarnProject project)
    {
        if (dialogueRunner == null) return;
        if (dialogueRunner.IsDialogueRunning) return;

        string targetNode = "Node_" + progressSave.TylerDialogueIndex.ToString();

        dialogueRunner.SetProject(project);

        bool nodeExists = false;
        foreach (var nodeName in project.NodeNames)
        {
            if (nodeName == targetNode)
            {
                nodeExists = true;
                break;
            }
        }

        if (nodeExists)
        {
            dialogueRunner.StartDialogue(targetNode);
        }
        else
        {
            Debug.LogError($"Yarn Error: No node named '{targetNode}' found in {project.name}!");
        }
    }

    public void AdvanceDialogueProgress()
    {
        progressSave.TylerDialogueIndex++;
    }

    public void TryUseWireCutters()
    {
        if (inventoryObject == null || wireCuttersItem == null)
        {
            Debug.LogError("Inventory or Required Item missing on " + gameObject.name);
            return;
        }

        if (inventoryObject.HasItem(wireCuttersItem, 1))
        {
            if (objectToActivate != null)
            {
                objectToActivate.SetActive(true);
            }

            inventoryObject.RemoveMultipleItems(wireCuttersItem, 1);

            if (deleteWholeObject)
            {
                Destroy(gameObject);
            }
            else
            {
                Destroy(this);
            }

            Debug.Log("You cut the fence. Cutters lost in process.");
        }
        else
        {
            if (alertSystem != null)
            {
                alertSystem.ShowAlert("You don't have wire cutters. Try the northern part of the beauty clinic.");
            }
        }
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
    public ObjectSaving objectSaving;
    public void AddToInvetory(ItemData item)
    {
        if (canPickUp)
        {
            if (inventoryObject == null)
            {
                Debug.LogError($"InventoryObject missing on {gameObject.name}");
                return;
            }

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

            if (alertSystem != null)
            {
                alertSystem.ShowAlert(item.itemName + " added to inventory!");
            }
            else
            {
                Debug.LogWarning($"AlertSystem missing on {gameObject.name}. Item added silently.");
            }

            inventoryObject.AddItem(item);
            canPickUp = false;
            canBeViewed = false;

            Color newColor = spriteRenderer.color;
            newColor.a = 0;
            spriteRenderer.color = newColor;
        }
    }

    public void RemoveItems(ItemData requiredItem, int requiredAmount)
    {
        if (inventoryObject == null) return;

        if (inventoryObject.HasItem(requiredItem, requiredAmount))
        {
            inventoryObject.RemoveMultipleItems(requiredItem, requiredAmount);

            if (alertSystem != null)
            {
                alertSystem.ShowAlert($"-{requiredAmount} {requiredItem.itemName}");
            }

            onInteract.Invoke();
        }
        else
        {
            if (alertSystem != null)
            {
                alertSystem.ShowAlert($"Missing: {requiredAmount} {requiredItem.itemName}");
            }
            Debug.Log("Interaction blocked: Not enough items.");
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