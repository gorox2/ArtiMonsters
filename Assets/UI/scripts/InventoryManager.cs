using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public GameObject inventory;
    bool inventoryActive = false;
    [SerializeField] GameObject slot;
    public List<itemSlots> itemSlots;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.I) && inventoryActive)
        {
            Time.timeScale = 1;
            inventory.SetActive(false);
            inventoryActive = false;
        }
        else if(Input.GetKeyDown(KeyCode.I) && !inventoryActive)
        {
            Time.timeScale = 0.1f;
            inventory.SetActive(true); 
            inventoryActive = true;
        }
    }

    public void AddItem(string itemName, int itemQuantity, Sprite itemSprite)
    {
        for(int i = 0; i < itemSlots.Count; i++)
        {
            if (itemSlots[i].isFull == false)
            {
                itemSlots[i].AddItem(itemName, itemQuantity, itemSprite);
                return;
            }
        }
        
         
        return;
    }

    public void AddSlot()
    {
        
        
    }
}
