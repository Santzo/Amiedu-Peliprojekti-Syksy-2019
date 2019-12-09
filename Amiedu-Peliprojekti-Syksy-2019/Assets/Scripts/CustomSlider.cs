using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomSlider : Slider
{
    private Color oriColor;
    private Image fillColor;
    protected override void Awake()
    {
        fillColor = transform.GetFromAllChildren("Fill").GetComponent<Image>();
        oriColor = fillColor.color;
    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
            fillColor.color = new Color(oriColor.r + 0.1f, oriColor.g + 0.2f, oriColor.b + 0.25f);
        Audio.PlaySound("Select", 0.8f, 0.9f);
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        fillColor.color = oriColor;
    }
    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        Audio.PlaySound("Click", 1f, 0.8f);
    }
}
