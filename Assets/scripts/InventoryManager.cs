using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [Serializable]
    public class InventorySlot
    {
        public ItemData item;
        public int quantity;

        public InventorySlot(ItemData item, int quantity)
        {
            this.item = item;
            this.quantity = quantity;
        }
    }

    [Header("Inventory Settings")]
    [SerializeField] private int maxSlots = 30;

    private List<InventorySlot> slots = new List<InventorySlot>();
    public IReadOnlyList<InventorySlot> Slots => slots;

    public event Action<ItemData, int> OnItemAdded; 
    public event Action OnInventoryChanged;
    public event Action<int, int> OnArtifactProgressChanged; 

    [Header("Artifacts Goal")]
    [SerializeField] private int totalArtifactsInLevel = 1;
    private int collectedArtifacts = 0;

    public int TotalArtifactsInLevel => totalArtifactsInLevel;
    public int CollectedArtifacts => collectedArtifacts;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
       
    }

    public void SetTotalArtifactsForLevel(int total)
    {
        totalArtifactsInLevel = Mathf.Max(0, total);
        collectedArtifacts = CountCollectedArtifacts();
        OnArtifactProgressChanged?.Invoke(collectedArtifacts, totalArtifactsInLevel);
    }

    public bool AddItem(ItemData item, int amount = 1)
    {
        if (item == null || amount <= 0) return false;

        // Safety
        if (maxSlots <= 0)
        {
            Debug.LogError("Inventory maxSlots <= 0. Cannot add items.");
            return false;
        }

        // Non-stackable items 
        if (!item.stackable)
        {
            int free = maxSlots - slots.Count;
            if (free < amount)
            {
                Debug.Log($"Inventory full for non-stackable item. free={free}, needed={amount}");
                OnInventoryChanged?.Invoke();
                return false;
            }

            for (int i = 0; i < amount; i++)
            {
                slots.Add(new InventorySlot(item, 1));
                OnItemAdded?.Invoke(item, 1);
            }

            HandleArtifactProgressIfNeeded(item);
            OnInventoryChanged?.Invoke();
            Debug.Log("supposed to return true ");
            return true;
        }

        // Stackable items: fill existing stacks first
        int remaining = amount;

        for (int i = 0; i < slots.Count && remaining > 0; i++)
        {
            if (slots[i].item == item && slots[i].quantity < item.maxStack)
            {
                int room = item.maxStack - slots[i].quantity;
                int toAdd = Mathf.Min(room, remaining);
                slots[i].quantity += toAdd;
                remaining -= toAdd;
                OnItemAdded?.Invoke(item, toAdd);
            }
        }

        // Create new stacks if needed
        while (remaining > 0)
        {
            if (slots.Count >= maxSlots)
            {
                Debug.Log($"Inventory full while creating new stack. remaining={remaining}");
                OnInventoryChanged?.Invoke();
                return false;
            }

            int toAdd = Mathf.Min(item.maxStack, remaining);
            slots.Add(new InventorySlot(item, toAdd));
            remaining -= toAdd;
            OnItemAdded?.Invoke(item, toAdd);
        }

        HandleArtifactProgressIfNeeded(item);
        OnInventoryChanged?.Invoke();
        return true;
    }

    public bool RemoveItem(ItemData item, int amount = 1)
    {
        if (item == null || amount <= 0) return false;

        for (int i = slots.Count - 1; i >= 0 && amount > 0; i--)
        {
            if (slots[i].item == item)
            {
                int remove = Mathf.Min(slots[i].quantity, amount);
                slots[i].quantity -= remove;
                amount -= remove;

                if (slots[i].quantity <= 0)
                    slots.RemoveAt(i);
            }
        }

        collectedArtifacts = CountCollectedArtifacts();
        OnArtifactProgressChanged?.Invoke(collectedArtifacts, totalArtifactsInLevel);
        OnInventoryChanged?.Invoke();
        return amount == 0;
    }

    private void HandleArtifactProgressIfNeeded(ItemData item)
    {
        if (item.itemType == ItemType.Artifact)
        {
            collectedArtifacts = CountCollectedArtifacts();
            OnArtifactProgressChanged?.Invoke(collectedArtifacts, totalArtifactsInLevel);
        }
    }

    private int CountCollectedArtifacts()
    {
        int count = 0;
        foreach (var s in slots)
        {
            if (s.item != null && s.item.itemType == ItemType.Artifact)
                count += s.quantity;
        }
        return count;
    }

    public void ClearInventory()
    {
        slots.Clear();
    }
}

