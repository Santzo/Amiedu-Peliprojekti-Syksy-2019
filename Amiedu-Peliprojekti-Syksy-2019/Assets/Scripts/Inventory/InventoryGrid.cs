using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryGrid : MonoBehaviour
{

    private GameObject[] itemsIcons;
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
    }
    private void OnDisable()
    {
        Events.updateFilteredItems -= UpdateList;
    }

    private void UpdateList(List<InventoryItems> items)
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
            itemsIcons[i].transform.SetParent(mask);
            itemsIcons[i].transform.localPosition = new Vector2(placeHolder.x + ((i - (row * maxWidth)) * iconWidth), placeHolder.y + (-row * iconWidth));

            Image itemImage = itemsIcons[i].transform.GetChild(0).GetComponent<Image>();
            itemImage.sprite = items[i].icon;
        }
        

    }
}
