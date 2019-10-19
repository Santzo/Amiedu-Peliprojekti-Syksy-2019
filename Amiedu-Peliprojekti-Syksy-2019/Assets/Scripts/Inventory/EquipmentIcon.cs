﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EquipmentIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
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
