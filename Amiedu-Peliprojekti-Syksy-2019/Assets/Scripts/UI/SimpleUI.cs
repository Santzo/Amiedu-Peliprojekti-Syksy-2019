using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SimpleUI : MonoBehaviour, IUIObject
{
    ISimpleUIHandler handler;
    void Awake()
    {
        handler = GetComponentInParent<ISimpleUIHandler>();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
       
        handler.SimpleClick(transform.GetSiblingIndex(), eventData.button);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
     
        handler.SimpleEnter(transform.GetSiblingIndex());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        handler.SimpleLeave(transform.GetSiblingIndex());
    }

}
