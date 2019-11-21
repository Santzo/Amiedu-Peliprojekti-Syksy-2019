using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipConsumable : MonoBehaviour, IUIHandler, IPointerClickHandler
{
    List<UIItem> uitem = new List<UIItem>();
    Inventory currentItem;
    TextMeshProUGUI text;
    Image[] icon;
    public void Awake()
    {
        uitem.UItemInitialize(transform);
        text = transform.Find("TitleText").GetComponent<TextMeshProUGUI>();
        icon = new Image[uitem.Count];
        for (int i = 0; i < uitem.Count; i++)
        {
            icon[i] = uitem[i].trans.GetChild(1).GetComponent<Image>();
        }
        SetKeys();
    }

    private void OnEnable()
    {
        Events.onEquipConsumable += Equip;
    }

    private void OnDisable()
    {
        Events.onEquipConsumable -= Equip;
    }

    private void Equip(Inventory item)
    {
        currentItem = item;
        SetKeys();
        ResetIcons();
        text.text = "Select quickslot for " + TextColor.Return("green") + item.item.name;
    }

    public void EntryClick(int index, PointerEventData.InputButton button)
    {
        if (button == PointerEventData.InputButton.Left && index > -1)
        {
            CharacterStats.hotbar[index] = currentItem;
        }
        Events.onDiscard = false;
        ObjectPooler.op.DeSpawn(gameObject);
    }

    public void EntryEnter(int index)
    {
        icon[index].enabled = true;
        icon[index].sprite = currentItem.item.icon == null ? currentItem.item.obj.GetComponent<SpriteRenderer>().sprite : currentItem.item.icon;

        icon[index].transform.localScale = Vector3.one * currentItem.item.iconScale;
        uitem[index].anim.SetBool("Hover", true);
    }

    public void EntryLeave(int index)
    {

        uitem[index].anim.SetBool("Hover", false);
        icon[index].enabled = CharacterStats.hotbar[index].item != null;
        if (icon[index].enabled)
        {
            Sprite img = CharacterStats.hotbar[index].item.icon ?? null;
            icon[index].sprite = img != null ? img : CharacterStats.hotbar[index].item.obj.GetComponent<SpriteRenderer>().sprite;
            icon[index].transform.localScale = Vector3.one * CharacterStats.hotbar[index].item.iconScale;
        
        }
    }

    private void SetKeys()
    {
        for (int i = 0; i < uitem.Count; i++)
        {
            uitem[i].text.text = KeyboardConfig.hotbar[i].ToString();
        }
    }
    private void ResetIcons()
    {
       
        for (int i = 0; i < icon.Length; i++)
        {
            var hot = CharacterStats.hotbar[i].item;
            var tempIcon = hot != null ? hot.icon != null ? hot.icon : hot.obj.GetComponent<SpriteRenderer>().sprite : null;
            icon[i].sprite = tempIcon ?? icon[i].sprite;
            icon[i].color = hot != null ? hot.colorTint : Color.white;
            if (CharacterStats.hotbar[i].item != null) icon[i].transform.localScale = Vector3.one * CharacterStats.hotbar[i].item.iconScale;

            icon[i].enabled = tempIcon;
            icon[i].preserveAspect = true;
        
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            Events.onDiscard = false;
            ObjectPooler.op.DeSpawn(gameObject);
        }
    }
}

