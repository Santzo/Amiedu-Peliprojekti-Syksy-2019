using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EquipmentIcon : MonoBehaviour, IUIObject
{
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
            Events.onEquipmentIconPress(transform.GetSiblingIndex());
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Events.onDrag || Events.onDialogueBox) return;
        Events.onEquipmentIconHover(transform.GetSiblingIndex());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (Events.onDrag || Events.onDialogueBox) return;
        Events.onEquipmentIconHoverLeave(transform.GetSiblingIndex());

    }
}
