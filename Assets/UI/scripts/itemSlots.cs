using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class itemSlots : MonoBehaviour 
{
    #region Item Data
    public string itemName;
    public int itemQuantity;
    public Sprite itemSprite;
    public bool isFull = false;
    #endregion

    #region item Slot
    [SerializeField]
    TMP_Text quantityText;
    [SerializeField]
    Image itemImage;
    #endregion


    
    public void AddItem(string itemName, int itemQuantity, Sprite itemSprite)
    {
        this.itemName = itemName;
        this.itemQuantity = itemQuantity;
        this.itemSprite = itemSprite;

        quantityText.text = itemQuantity.ToString();
        quantityText.enabled = true;
        itemImage.sprite = itemSprite;
        isFull = true;
    }
}
