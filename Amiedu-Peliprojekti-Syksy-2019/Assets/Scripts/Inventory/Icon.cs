using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Icon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler
{
    private Image background;
    private Color oriColor;
    private Vector2 oriPos;
    private Transform oriParent;
    private TextMeshProUGUI amount;
    private Type type;
    [HideInInspector]
    public int index;
    private bool clicked = false;

    public void Awake()
    {
        background = GetComponent<Image>();
        oriColor = background.color;
        amount = transform.Find("Amount").GetComponent<TextMeshProUGUI>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (Events.onDialogueBox) return;
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
        if (Events.onDialogueBox) return;
        if (type != typeof(Consumable))
        {
            Vector2 pos = Info.camera.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector2(pos.x, pos.y);
        }
    }
    public void OnEnable()
    {
        background.color = oriColor;

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (Events.onDialogueBox) return;
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
        if (Events.onDrag || Events.onDialogueBox) return;
        var pos = transform.localPosition.y + Info.content.localPosition.y;
        if (pos >= -13f) Events.onItemHover(index, transform.position);
        else Events.onItemHover(index, new Vector2(transform.position.x, transform.position.y + 4f));
        background.color = new Color(oriColor.r, oriColor.g, oriColor.b, 0.75f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (Events.onDrag || Events.onDialogueBox) return;
        background.color = oriColor;
        Events.onItemLeaveHover();
    }

    public void CheckAmount()
    {
        background.color = oriColor;
        int amo = InventoryManager.im.filteredItems[index].amount;
        amount.text = amo > 1 ? amo.ToString() : "";
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (Events.onDrag || Events.onDialogueBox) return;

        switch (eventData.button)
        {
            case PointerEventData.InputButton.Left:
                if (!clicked)
                {
                    StartCoroutine(DoubleClickCheck());
                }
                else
                {
                    Inventory item = InventoryManager.im.filteredItems[index];
                    if (item.item.GetType() == typeof(Consumable))
                    {
                        Events.onDialogueBox = true;
                        GameObject obj = ObjectPooler.op.SpawnUI("EquipConsumable", transform.localPosition.x > 270 ? new Vector2(transform.position.x - 0.75f, transform.position.y) : (Vector2)transform.position, transform.parent.parent.parent);
                        Events.onEquipConsumable(item);
                    }
                    else
                    {
                        Events.onIconDoubleClick(index, InventoryManager.im.filteredItems[index]);
                     
                    }
                    Events.onItemLeaveHover();
                    clicked = false;
                }
                break;

            case PointerEventData.InputButton.Right:
                GameObject right = ObjectPooler.op.SpawnUI("RightClick", transform.position, transform.parent.parent.parent);
                RightClick rce = right.GetComponent<RightClick>();
                rce.itemIndex = index;
                rce.uitem[0].text.text = "Equip";
                rce.uitem[1].text.text = "Discard";
                break;
        }
    }

    private IEnumerator DoubleClickCheck()
    {
        clicked = true;
        yield return new WaitForSeconds(KeyboardConfig.doubleClickInterval);
        clicked = false;
    }
}
