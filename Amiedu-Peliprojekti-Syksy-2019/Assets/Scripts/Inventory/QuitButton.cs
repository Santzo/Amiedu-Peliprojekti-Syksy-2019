using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class QuitButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    private IUIHandler controller;
    private Image background;
    private Color oriColor;

    private void Awake()
    {
        controller = transform.parent.GetComponent<IUIHandler>();
        background = GetComponent<Image>();
        oriColor = background.color;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        controller.EntryClick(-1, eventData.button);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        background.color = new Color(0.2f, 0.06f, 0.03f, 0.95f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        background.color = oriColor;
    }
    private void OnEnable()
    {
        background.color = oriColor;
    }
}
