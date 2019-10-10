﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Icon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Image background;
    private Color oriColor;

    public void Awake()
    {
        background = GetComponent<Image>();
        oriColor = background.color;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
      
    }

    public void OnDrag(PointerEventData eventData)
    {
  
    }

    public void OnEndDrag(PointerEventData eventData)
    {
 
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("jee");
        background.color = new Color(oriColor.r, oriColor.g, oriColor.b, 0.58f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        background.color = oriColor;
    }

  
}
