using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIEvents : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    [HideInInspector]
    public IUIHandler mouseController;
    [HideInInspector]
    public int index;

    private void Awake()
    {
        mouseController = GetComponentInParent<IUIHandler>();
        index = transform.GetSiblingIndex();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        mouseController.EntryClick(index, eventData.button);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseController.EntryEnter(index);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseController.EntryLeave(index);
    }
}
