﻿using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Icon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Image background;
    private Color oriColor;
    private Vector2 oriPos;
    private Transform oriParent;
    private TextMeshProUGUI amount;
    private Type type;
    [HideInInspector]
    public int index;

    public void Awake()
    {
        background = GetComponent<Image>();
        oriColor = background.color;
        amount = transform.Find("Amount").GetComponent<TextMeshProUGUI>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        type = InventoryManager.im.filteredItems[index].item.GetType();

        if (type != typeof(Consumable))
        {
            Events.onDrag = true;
            Events.onItemDragStart();
            oriPos = transform.position;
            oriParent = transform.parent;
            transform.SetParent(transform.parent.parent.parent.parent);
            background.color = oriColor;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (type != typeof(Consumable))
        {
            transform.position = Input.mousePosition;
        }
    }
    public void OnEnable()
    {
        background.color = oriColor;

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (type != typeof(Consumable))
        {
            Events.onDrag = false;
            Events.onItemDragStop(index, transform.position);
            transform.position = oriPos;
            transform.SetParent(oriParent);
            background.color = new Color(oriColor.r, oriColor.g, oriColor.b, 0.75f);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Events.onDrag) return;
        var pos = transform.position.y / Info.CanvasScale;
        if (pos >= 480) Events.onItemHover(index, transform.position);
        else Events.onItemHover(index, new Vector2(transform.position.x, transform.position.y + 450 * Info.CanvasScale));
        background.color = new Color(oriColor.r, oriColor.g, oriColor.b, 0.75f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (Events.onDrag) return;
        background.color = oriColor;
        Events.onItemLeaveHover();
    }

    public void CheckAmount()
    {
        background.color = oriColor;
        int amo = InventoryManager.im.filteredItems[index].amount;
        amount.text = amo > 1 ? amo.ToString() :  "";
    }
}
