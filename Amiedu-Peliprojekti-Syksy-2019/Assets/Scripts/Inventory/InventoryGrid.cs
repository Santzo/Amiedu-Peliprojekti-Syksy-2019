using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryGrid : MonoBehaviour
{

    private GameObject[] itemsIcons;
    private GameObject hoverItem;
    private Vector2 placeHolder;
    private int iconWidth = 115;
    private int maxWidth = 6;
    private Transform mask;

    private void Awake()
    {
        placeHolder = transform.Find("PlaceHolder").localPosition;
        mask = transform.Find("Mask").GetChild(0);
    }

    private void OnEnable()
    {
        Events.updateFilteredItems += UpdateList;
        Events.onItemHover += ItemHover;
        Events.onItemLeaveHover += ItemLeave;
    }

   

    private void OnDisable()
    {
        Events.updateFilteredItems -= UpdateList;
        Events.onItemHover -= ItemHover;
        Events.onItemLeaveHover -= ItemLeave;
    }

    private void UpdateList(List<Inventory> items)
    {
        if (itemsIcons != null)
        {
            foreach (var icon in itemsIcons)
                ObjectPooler.op.DeSpawn(icon);
        }
        itemsIcons = new GameObject[items.Count];

        int row = 0;
        for (int i = 0; i < items.Count; i++)
        {
            row = i % maxWidth == 0 && i > 0 ? row + 1 : row;
            itemsIcons[i] = ObjectPooler.op.Spawn("InventoryItem");
            itemsIcons[i].transform.SetParent(mask, false);
            itemsIcons[i].transform.localPosition = new Vector2(placeHolder.x + ((i - (row * maxWidth)) * iconWidth), placeHolder.y + (-row * iconWidth));
            Icon icon = itemsIcons[i].GetComponent<Icon>();
            icon.index = i;
            icon.CheckAmount();

            Image itemImage = itemsIcons[i].transform.GetChild(0).GetComponent<Image>();
            itemImage.sprite = items[i].item.icon;
            itemImage.transform.localScale = Vector3.one * InventoryManager.im.filteredItems[i].item.iconScale;
        }

       
    }
    private void ItemHover(int index, Vector2 position)
    {
        hoverItem = ObjectPooler.op.Spawn("ItemDetails");
        hoverItem.transform.SetParent(transform, false);
        hoverItem.transform.position = position;
        hoverItem.GetComponent<ItemDetails>().DisplayDetails(InventoryManager.im.filteredItems[index].item);
  
    }
    private void ItemLeave()
    {
        ObjectPooler.op.DeSpawn(hoverItem);
    }

}
