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
    private Slider slider;
    private int iconWidth = 115;
    private int maxWidth = 6;
    private Transform mask;
    private Transform content;

    private void Awake()
    {
        placeHolder = transform.Find("PlaceHolder").localPosition;
        mask = transform.Find("Mask").GetChild(0);
        slider = transform.Find("Slider").GetComponent<Slider>();
        slider.gameObject.SetActive(false);
        content = transform.Find("Mask").GetChild(0);
    
    }

    private void Update()
    {
        if (Input.mouseScrollDelta.y != 0)
        {
            slider.value -= Input.mouseScrollDelta.y * (slider.maxValue * 0.05f);
        }
    }

    private void OnEnable()
    {
        Events.updateFilteredItems += UpdateList;
        Events.onItemHover += ItemHover;
        Events.onItemLeaveHover += ItemLeave;
        slider.onValueChanged.AddListener(sliderChange);
        if (hoverItem != null) ObjectPooler.op.DeSpawn(hoverItem);
    }



    private void OnDisable()
    {
        Events.updateFilteredItems -= UpdateList;
        Events.onItemHover -= ItemHover;
        Events.onItemLeaveHover -= ItemLeave;
        slider.onValueChanged.RemoveAllListeners();
        if (hoverItem != null) ObjectPooler.op.DeSpawn(hoverItem);
    }


    private void sliderChange(float sliderValue)
    {
        content.localPosition = new Vector2(0f, slider.value);
    }
    private void UpdateList(List<Inventory> items)
    {
        if (itemsIcons != null)
        {
            foreach (var icon in itemsIcons)
                ObjectPooler.op.DeSpawn(icon);
        }
        itemsIcons = new GameObject[items.Count];
        slider.gameObject.SetActive(items.Count > 30 ? true : false);
        if (slider)
        {
            int itemsOver = items.Count - 30;
            int scrollableRows = Mathf.CeilToInt((float) itemsOver / (float) maxWidth);
            slider.value = 0;
            slider.maxValue = scrollableRows * iconWidth + 5;
        }


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
            itemImage.sprite = items[i].item.icon == null ? items[i].item.obj.GetComponent<SpriteRenderer>().sprite : items[i].item.icon;
            itemImage.transform.localScale = Vector3.one * InventoryManager.im.filteredItems[i].item.iconScale;
        }
        content.localPosition = new Vector2(0f, 0f);

    }
    private void ItemHover(int index, Vector2 position)
    {
        hoverItem = ShowItemDetails(index, position, transform.parent);
    }

    private void ItemLeave()
    {
        ObjectPooler.op.DeSpawn(hoverItem);
    }

    public static GameObject ShowItemDetails(int index, Vector2 position, Transform parent, bool equipped = false)
    {
        GameObject obj = ObjectPooler.op.Spawn("ItemDetails");
  
        obj.transform.SetParent(parent, false);
        obj.transform.SetAsLastSibling();
        obj.transform.position = position;
        obj.GetComponent<ItemDetails>().DisplayDetails(InventoryManager.im.filteredItems[index].item);
        return obj;
    }

    public static GameObject ShowItemDetails(InventoryItems item, Vector2 position, Transform parent, bool equipped = false)
    {
        GameObject obj = ObjectPooler.op.Spawn("ItemDetails");
        obj.transform.SetParent(parent, false);
        obj.transform.SetAsLastSibling();
        obj.transform.position = position;
        obj.GetComponent<ItemDetails>().DisplayDetails(item, equipped);
        return obj;
    }

}
