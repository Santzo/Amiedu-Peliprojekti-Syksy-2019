using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RightClickEntry : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    [HideInInspector]
    public RightClick rightClick;

    public void OnPointerClick(PointerEventData eventData)
    {
        rightClick.EntryClick(transform.GetSiblingIndex(), eventData.button);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        rightClick.EntryEnter(transform.GetSiblingIndex());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        rightClick.EntryLeave(transform.GetSiblingIndex());
    }
}
