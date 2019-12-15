using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIEvents : MonoBehaviour,  IUIObject
{
    [HideInInspector]
    public IUIHandler mouseController;
    [HideInInspector]
    public int index;

    public void OnPointerClick(PointerEventData eventData)
    {
        Audio.PlaySound("Click");
        mouseController.EntryClick(index, eventData.button);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Audio.PlaySound("Select");
        mouseController.EntryEnter(index);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseController.EntryLeave(index);
    }
}
