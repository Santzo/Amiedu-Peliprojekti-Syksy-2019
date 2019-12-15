using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChestContentUI : MonoBehaviour, IUIHandler, ISimpleUIHandler
{
    List<UIItem> buttons = new List<UIItem>();
    ChestItem[] items;
    Transform detailPos;
    TextMeshProUGUI currentWeight;
    GameObject currentDetail;
    List<Inventory> currentContent;
    TreasureChest bindedChest;
    bool clicked;

    public void Awake()
    {
        buttons.UItemInitialize(transform);
        var info = transform.Find("Info");
        items = new ChestItem[info.childCount];
        for (int i = 0; i < items.Length; i++)
        {
            items[i].img = info.GetChild(i).GetComponent<Image>();
            items[i].icon = info.GetChild(i).GetChild(1).GetComponent<Image>();
            items[i].text = info.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>();
        }
        currentWeight = transform.Find("Weight").GetComponent<TextMeshProUGUI>();
        detailPos = transform.Find("DetailPos");
    }

    void OnEnable()
    {
        currentWeight.text = $"Current weight {CharacterStats.totalWeight}/{CharacterStats.weightLimit}";
    }

    public void EntryClick(int index, PointerEventData.InputButton button)
    {
        for (int i = 0; i < currentContent.Count; i++)
        {
            InventoryManager.im.AddToInventory(currentContent[i]);
        }
        ObjectPooler.op.DeSpawn(gameObject);
        bindedChest.AllItemsRetrieved();
        if (currentDetail != null) ObjectPooler.op.DeSpawn(currentDetail);
    }
    public void EntryEnter(int index)
    {
        buttons[index]?.anim?.SetBool("Hover", true);
    }

    public void EntryLeave(int index)
    {
        buttons[index]?.anim?.SetBool("Hover", false);
    }
    public void UpdateContents(List<Inventory> content, TreasureChest chest)
    {
        bindedChest = chest;
        currentContent = content;
        for (int i = 0; i < items.Length; i++)
        {
            if (i < content.Count)
            {
                items[i].text.text = content[i].item.name;
                items[i].icon.sprite = content[i].item.icon == null ? content[i].item.obj.GetComponent<SpriteRenderer>().sprite : content[i].item.icon;
                if (content[i].item.material != null)
                {
                    Material mat = new Material(content[i].item.material);
                    items[i].icon.material = mat;
                    content[i].item.modifiedMat = mat;
                    SetMaterialProperties custom = content[i].item.obj.GetComponent<SetMaterialProperties>();
                    if (custom != null) items[i].icon.material.SetUIPropertyBlock(content[i].item.obj.GetComponent<Renderer>());
                }
                else items[i].icon.material = null;
                if (content[i].amount > 1)
                    items[i].text.text += $" x{content[i].amount}";
            }
            else
            {
                items[i].text.text = "";
                items[i].icon.enabled = false;
            }
            items[i].img.enabled = false;
        }
    }

    public void SimpleEnter(int index)
    {
        if (index >= currentContent.Count) return;
        items[index].img.enabled = true;
        currentDetail = InventoryGrid.ShowItemDetails(currentContent[index].item, detailPos.position, transform.parent);
    }

    public void SimpleClick(int index, PointerEventData.InputButton button)
    {
        if (button != PointerEventData.InputButton.Left) return;
        if (!clicked) StartCoroutine(DoubleClickCheck());
        else if (clicked)
        {
            InventoryManager.im.AddToInventory(currentContent[index]);
            currentContent.Remove(currentContent[index]);
            if (currentContent.Count == 0)
            {
                ObjectPooler.op.DeSpawn(gameObject);
                bindedChest.AllItemsRetrieved();
            }
            if (currentDetail != null) ObjectPooler.op.DeSpawn(currentDetail);
            UpdateContents(currentContent, bindedChest);
        }
    }

    public void SimpleLeave(int index)
    {
        if (index >= currentContent.Count) return;
        items[index].img.enabled = false;
        currentDetail?.SetActive(false);
    }

    struct ChestItem
    {
        public Image img;
        public Image icon;
        public TextMeshProUGUI text;
    }
    private IEnumerator DoubleClickCheck()
    {
        clicked = true;
        yield return new WaitForSeconds(KeyboardConfig.doubleClickInterval);
        clicked = false;
    }
}
