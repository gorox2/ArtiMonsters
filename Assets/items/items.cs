using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class items : MonoBehaviour
{
    [SerializeField]
    string itemName;
    [SerializeField]
    int itemQuantity;
    [SerializeField]
    Sprite itemSprite;

    InventoryManager inventoryManager;
    bool canCollect = false;
    // Start is called before the first frame update
    void Start()
    {
        inventoryManager = GameObject.Find("Canvas").GetComponent<InventoryManager>();
        itemName = gameObject.name;
        itemQuantity = 1;
        itemSprite = gameObject.GetComponentInChildren<SpriteRenderer>().sprite; 
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && canCollect)
        {
            inventoryManager.AddItem(itemName, itemQuantity, itemSprite);
            inventoryManager.AddSlot();
            Destroy(gameObject);    
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" ) 
        {
            canCollect = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            canCollect = false;
        }
    }
}
