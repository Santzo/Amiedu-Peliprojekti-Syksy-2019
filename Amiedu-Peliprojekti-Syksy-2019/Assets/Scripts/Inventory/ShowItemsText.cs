using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ShowItemsText : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, ISpawn
{
    private Image background;
    private ShowItems showItems;
    private Color oriColor;
    public TextMeshProUGUI text;
    public int index;

    private void Awake()
    {
        background = GetComponent<Image>();
        oriColor = background.color;
        text = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        showItems = transform.parent.GetComponent<ShowItems>();
    }



    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Events.onDrag || Events.onDiscard) return;
        background.color = new Color(oriColor.r + 0.2f, oriColor.g + 0.06f, oriColor.b, oriColor.a + 0.1f);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Events.onDrag || Events.onDiscard) return;
        showItems.onClick(text.text);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (Events.onDrag || Events.onDiscard) return;
        background.color = oriColor;
    }

    public void Spawn()
    {
        background.color = oriColor;
    }
}
