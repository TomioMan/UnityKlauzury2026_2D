using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class InventoryUIManager : MonoBehaviour
{
    [Header("REFERENCES")]
    public InventoryObject playerInventory;
    public GameObject slotPrefab;

    [Header("LAYOUT")]
    public Transform contentParent;

    private void OnEnable()
    {
        RefreshUI();
    }

    public void RefreshUI()
    {
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(UpdateInventoryDisplay());
        }
    }

    private IEnumerator UpdateInventoryDisplay()
    {
        // 1. Clear old slots safely
        foreach (Transform child in contentParent)
        {
            if (child != null) Destroy(child.gameObject);
        }

        // 2. Wait for end of frame to ensure destruction is finished
        yield return new WaitForEndOfFrame();

        // 3. Safety check for the prefab and inventory
        if (slotPrefab == null)
        {
            Debug.LogError("InventoryUIManager: Slot Prefab is missing! Drag it into the inspector.");
            yield break;
        }

        if (playerInventory == null)
        {
            Debug.LogError("InventoryUIManager: Player Inventory ScriptableObject is missing!");
            yield break;
        }

        // 4. Create new slots
        foreach (ItemData item in playerInventory.items)
        {
            // Skip if the item entry itself is null
            if (item == null) continue;

            GameObject newSlot = Instantiate(slotPrefab, contentParent);
            newSlot.SetActive(true);

            InventorySlotUI slotScript = newSlot.GetComponent<InventorySlotUI>();
            if (slotScript != null)
            {
                slotScript.Setup(item, this);
            }
        }

        // 5. Final Layout Polish
        Canvas.ForceUpdateCanvases();
        RectTransform rect = contentParent.GetComponent<RectTransform>();
        if (rect != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
        }
    }
}