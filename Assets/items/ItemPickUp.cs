using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    [Header("Item")]
    public ItemData itemData;
    public int amount = 1;

    [Header("Prompt")]
    public InteractPromptUI PromptUI; 
    public string playerTag = "Player";
    public KeyCode pickupKey = KeyCode.E;

    [Header("Audio")]
    public AudioClip pickupSfx;
    [Range(0f, 1f)] public float volume = 1f;

    private bool playerInRange = false;
    int promptToken;

    private void Start()
    {
        if (PromptUI != null) PromptUI.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!playerInRange) return;

        if (Input.GetKeyDown(pickupKey))
        {
            Debug.Log("E pressed while in range -> TryPickup");
            TryPickup();
        }
    }

    private void TryPickup()
    {
        if (itemData == null)
        {
            Debug.LogError($"[{name}] itemData is NULL");
            return;
        }
        if (InventoryManager.Instance == null)
        {
            Debug.LogError("InventoryManager.Instance is NULL");
            return;
        }
        Debug.Log($"[{name}] Before AddItem. Slots count = {InventoryManager.Instance.Slots.Count}");

        bool success = InventoryManager.Instance.AddItem(itemData, amount);
        Debug.Log($"[{name}] AddItem returned: {success}");
        Debug.Log($"[{name}] After AddItem. Slots count = {InventoryManager.Instance.Slots.Count}");

        if (!success) return; // optional: show "Inventory Full"

        
        if (pickupSfx != null)
            AudioSource.PlayClipAtPoint(pickupSfx, transform.position, volume);

        
        if (PromptUI != null) PromptUI.Hide(promptToken);

        
        PickupFeedUI.Instance?.Show(itemData, amount);
        Debug.Log($"[{name}] Destroying pickup object now");
       
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        playerInRange = true;
        if (PromptUI != null)
            promptToken = PromptUI.Show($"Pick Up [E]");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        playerInRange = false;
        if (PromptUI != null)
            PromptUI.Hide(promptToken);
    }

    public void Init(ItemData data, int amt)
    {
        itemData = data;
        amount = amt;

        
        var sr = GetComponent<SpriteRenderer>();
        if (sr != null && data != null && data.icon != null)
            sr.sprite = data.icon;
    }
}
