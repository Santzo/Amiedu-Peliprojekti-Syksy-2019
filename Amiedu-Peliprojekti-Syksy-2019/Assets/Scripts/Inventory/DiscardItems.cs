using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DiscardItems : MonoBehaviour, IUIHandler, IPointerExitHandler, IPointerClickHandler
{
    List<UIItem> uitem = new List<UIItem>();
    TextMeshProUGUI text;
    TextMeshProUGUI sliderText;
    public Slider slider;
    [HideInInspector]
    private int amount = 1;
    [HideInInspector]
    private int itemIndex;

    public void Awake()
    {
        uitem.UItemInitialize(transform);
        slider = transform.Find("Slider").GetComponent<Slider>();
        sliderText = slider.transform.Find("Amount").GetComponent<TextMeshProUGUI>();
        text = transform.Find("TitleText").GetComponent<TextMeshProUGUI>();
        itemIndex = -1;
        slider.onValueChanged.AddListener(amo => sliderText.text = amo.ToString());

    }


    public void EntryClick(int index, PointerEventData.InputButton button)
    {
        if (button == PointerEventData.InputButton.Right || button == PointerEventData.InputButton.Left && uitem[index].name == "Cancel")
            Close();
    }

    public void EntryEnter(int index)
    {
        uitem[index].anim.SetBool("Hover", true);
    }

    public void EntryLeave(int index)
    {
        uitem[index].anim.SetBool("Hover", false);
    }

    public void Spawn(int index)
    {
        itemIndex = index;
        Inventory item = InventoryManager.im.filteredItems[itemIndex];
        if (item != null)
        {
            amount = item.amount;
            slider.gameObject.SetActive(amount > 1 ? true : false);
            if (amount > 1)
            {
                slider.maxValue = amount;
                slider.value = 0;
            }
            text.text = "Discard " + TextColor.Return("yellow") + item.item.name + TextColor.Return("defaultTitle") + " ?";
        }
    }

    private void Close()
    {
        Events.onDiscard = false;
        ObjectPooler.op.DeSpawn(gameObject);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Close();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
            Close();
    }
}



