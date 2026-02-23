using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.tvOS;
using UnityEngine.UI;

public class InventoryUIController : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private KeyCode toggleKey = KeyCode.I;

    [Header("Slots")]
    [SerializeField] private Transform slotsParent;
    [SerializeField] private InventorySlotUI slotPrefab;

    [Header("Details")]
    [SerializeField] private Image detailIcon;
    [SerializeField] private TMP_Text detailName;
    [SerializeField] private TMP_Text detailDescription;
    [SerializeField] private TMP_Text detailType;
    [SerializeField] private Button useButton;
    [SerializeField] private Button discardButton;

    private readonly List<InventorySlotUI> spawned = new();
    private ItemData selectedItem;
    private int selectedQty;

    private void Start()
    {
        inventoryPanel.SetActive(false);
        ClearDetails();

        InventoryManager.Instance.OnInventoryChanged += Refresh;
        Refresh();

        useButton.onClick.AddListener(OnUseClicked);
        discardButton.onClick.AddListener(OnDiscardClicked);
    }

    private void OnDestroy()
    {
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.OnInventoryChanged -= Refresh;
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            bool open = !inventoryPanel.activeSelf;
            inventoryPanel.SetActive(open);

            if (open) Refresh();
        }
    }

    private void Refresh()
    {
        foreach (var s in spawned) Destroy(s.gameObject);
        spawned.Clear();

        foreach (var slot in InventoryManager.Instance.Slots)
        {
            var ui = Instantiate(slotPrefab, slotsParent);
            ui.Setup(slot.item, slot.quantity, OnSlotClicked);
            spawned.Add(ui);
        }

        ValidateSelection();
    }

    private void ValidateSelection()
    {
        if (selectedItem == null)
        {
            ClearDetails();
            return;
        }

        
        int quantity = 0;
        foreach (var s in InventoryManager.Instance.Slots)
        {
            if (s.item == selectedItem)
                quantity += s.quantity;
        }

        if (quantity <= 0)
        {
            // item no longer exists -> clear UI
            selectedItem = null;
            selectedQty = 0;
            ClearDetails();
            return;
        }

        // Update displayed quantity 
        selectedQty = quantity;

        // Buttons remain enabled appropriately
        useButton.interactable = selectedItem.isUsable;
        discardButton.interactable = true; 
    }

    private void OnSlotClicked(ItemData item, int qty)
    {
        selectedItem = item;
        selectedQty = qty;

        detailIcon.sprite = item.icon;
        detailIcon.enabled = item.icon != null;
        detailName.text = item.itemName;
        detailDescription.text = item.description;
        detailType.text = item.itemType.ToString();

        useButton.interactable = true;      
        discardButton.interactable = true;
    }

    private void OnUseClicked()
    {
        if (selectedItem == null) return;

        
        if (selectedItem.itemType == ItemType.Artifact) return;

        if (!HasAtLeast(selectedItem, 1))
        {
            ValidateSelection();
            return;
        }

        bool used = TryUseItem(selectedItem);
        if (!used) return;

        if (selectedItem.consumeOnUse)
            InventoryManager.Instance.RemoveItem(selectedItem, 1);

        Refresh();
    }

    private void OnDiscardClicked()
    {
        if (selectedItem == null) return;

        DropItem(selectedItem, 1);
        bool removed =  InventoryManager.Instance.RemoveItem(selectedItem, 1);
        if (!removed)
        {
            ValidateSelection();
            return;
        }

        
        Refresh();
    }

    private void DropItem(ItemData item, int amount)
    {
        if (item.worldPrefab == null)
        {
            Debug.LogWarning($"No worldPrefab set for {item.itemName}. Can't drop.");
            return;
        }

        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("No Player found with tag 'Player'. Can't drop.");
            return;
        }

        Vector3 spawnPos = player.transform.position + Vector3.right * 0.5f; 
        var go = Instantiate(item.worldPrefab, spawnPos, Quaternion.identity);

        
        var pickup = go.GetComponent<ItemPickUp>();
        if (pickup != null)
            pickup.Init(item, amount);
    }

    private bool TryUseItem(ItemData item)
    {

        if (item == null) return false;
        if (!item.isUsable) return false;

        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("TryUseItem: No Player with tag 'Player'.");
            return false;
        }

        if (item.effects == null || item.effects.Length == 0)
        {
            Debug.LogWarning($"TryUseItem: {item.itemName} is usable but has no effects.");
            return false;
        }

        bool anyApplied = false;

        foreach (var effect in item.effects)
        {
            if (effect == null) continue;

            bool applied = effect.Apply(player);
            anyApplied |= applied;
        }

        return anyApplied;
    }

    private bool HasAtLeast(ItemData item, int amount)
    {
        int qty = 0;
        foreach (var s in InventoryManager.Instance.Slots)
        {
            if (s.item == item) qty += s.quantity;
        }
        return qty >= amount;
    }
    private void ClearDetails()
    {
        detailIcon.sprite = null;
        detailIcon.enabled = false;
        detailName.text = "";
        detailDescription.text = "";
        detailType.text = "";
        useButton.interactable = false;
        discardButton.interactable = false;
    }
}
