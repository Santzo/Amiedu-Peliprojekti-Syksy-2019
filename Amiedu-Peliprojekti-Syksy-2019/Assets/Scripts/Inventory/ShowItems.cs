using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System;

public class ShowItems : MonoBehaviour, IResetUI
{
    private RectTransform showItemsBackground;
    private RectTransform border;


    private float itemHeight, borderHeight;
    private int howManyItemsShown = 1;
    private Image background;
    private Color oriColor;
    private bool expanded = false;
    private string oldText;

    public GameObject showItemsEntry;
    private GameObject[] itemsEntry;


    //private string[] items = new string[] { "All items", "Weapons", "Head Gear" };
    private string[] items = new string[] { "All items", "Weapons", "Head Gear", "Chest Guard", "Arm Guards", "Leg Guards", "Consumables" };


    private void Awake()
    {
        itemsEntry = new GameObject[items.Length];

        border = transform.Find("Border").GetComponent<RectTransform>();
        borderHeight = border.sizeDelta.y;

        itemHeight = showItemsEntry.GetComponent<RectTransform>().sizeDelta.y;

    }
    private void Start()
    {
        CreateItemEntry(0, items[5], new Vector2(0f, 0f));
        oldText = items[5];
    }

    public void onClick(string text = "")
    {
        expanded = text != "" ? !expanded : false;
        text = text != "" ? text : oldText;
        Array.ForEach(itemsEntry, item => { if (item != null) ObjectPooler.op.DeSpawn(item); });
       
        if (expanded)
        {
            CreateItemEntry(0, text, new Vector2(0f, 0f));
            int x = 0;
            for (int i = 0; i < items.Length; i++)
            {
                if (text != items[i])
                {
                    x++;
                    CreateItemEntry(x, items[i], new Vector2(0f, -x * itemHeight));
                }
            }
            border.sizeDelta = new Vector2(border.sizeDelta.x, borderHeight + itemHeight * (items.Length - 1));
        }
        else
        {
            CreateItemEntry(0, text, new Vector2(0f, 0f));
            border.sizeDelta = new Vector2(border.sizeDelta.x, borderHeight);
        }
        oldText = text;

    }

    private void CreateItemEntry(int index, string text, Vector2 position)
    {
        itemsEntry[index] = ObjectPooler.op.Spawn("ShowItemsEntry", new Vector2(0f, 0f), null, transform);
        itemsEntry[index].transform.localPosition = position;
        itemsEntry[index].transform.SetAsFirstSibling();
        itemsEntry[index].GetComponent<ShowItemsText>().text.text = text;
    }

    public void Reset()
    {
        onClick();
    }
}
