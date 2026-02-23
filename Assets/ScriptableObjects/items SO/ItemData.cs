using UnityEngine;


public enum ItemType
{
    Consumable,
    Equipment,
    Artifact
}
[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("Identity")]
    public string itemId;            
    public string itemName;
    [TextArea] public string description;
    public ItemType itemType;

    [Header("Visuals")]
    public Sprite icon;
    public GameObject worldPrefab;   

    [Header("Stacking")]
    public bool stackable = true;
    public int maxStack = 99;

    [Header("Use")]
    public bool isUsable = false;
    public bool consumeOnUse = true;

    public ItemEffect[] effects;
}
