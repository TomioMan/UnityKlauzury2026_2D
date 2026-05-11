using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class InventorySlotUI : MonoBehaviour
{
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private Button dropButton;

    private ItemData currentData;
    private InventoryUIManager manager;
    private bool isProcessing = false; // Prevents double-clicks

    public void Setup(ItemData data, InventoryUIManager uiManager)
    {
        if (data == null) return;

        currentData = data;
        manager = uiManager;
        isProcessing = false;

        gameObject.SetActive(true);

        Image buttonImage = dropButton.GetComponent<Image>();
        if (buttonImage != null) buttonImage.enabled = true;

        if (itemNameText != null)
        {
            itemNameText.enabled = true;
            itemNameText.text = data.itemName;
        }

        if (dropButton != null)
        {
            dropButton.enabled = true;
            dropButton.onClick.RemoveAllListeners();
            dropButton.onClick.AddListener(DropItem);
        }
    }

    public void DropItem()
    {
        if (currentData == null || manager == null || isProcessing) return;

        isProcessing = true; // Lock the button

        // Remove from inventory
        manager.playerInventory.RemoveItem(currentData);

        // Spawn the physical item
        if (currentData.itemPrefab != null)
        {
            SpawnAtCursor(currentData.itemPrefab);
        }

        // Refresh UI
        manager.RefreshUI();
    }

    private void SpawnAtCursor(GameObject prefabToSpawn)
    {
        Vector2 mousePos2D = Mouse.current.position.ReadValue();
        Vector3 mousePos = new Vector3(mousePos2D.x, mousePos2D.y, 10f);

        Vector3 worldSpawnPos = Camera.main.ScreenToWorldPoint(mousePos);
        Instantiate(prefabToSpawn, worldSpawnPos, Quaternion.identity);
    }
}