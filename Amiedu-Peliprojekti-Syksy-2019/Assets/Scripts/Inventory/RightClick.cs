using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class RightClick : MonoBehaviour, IPointerExitHandler, IUIHandler
{
    public List<UIItem> uitem = new List<UIItem>();
    public InventoryItems unEquip;
    public int itemIndex;

    public void Awake()
    {
        uitem.UItemInitialize(transform);
    }

    public void EntryEnter(int index)
    {
        uitem[index].anim.SetBool("Hover", true);
    }

    public void EntryClick(int index, PointerEventData.InputButton button)
    {
        if (button == PointerEventData.InputButton.Right)
            ObjectPooler.op.DeSpawn(gameObject);
        if (button == PointerEventData.InputButton.Left)
        {
            string buttonType = uitem[index].text.text;
            switch (buttonType)
            {
                case "Equip":
                    Inventory item = InventoryManager.im.filteredItems[itemIndex];
                    if (item.item.GetType() == typeof(Consumable))
                    {
                        SpawnDialogue("EquipConsumable");
                        Events.onDiscard = true;
                        Events.onEquipConsumable(item);
                     
                    }
                    else
                    {
                        Events.onIconDoubleClick(itemIndex, item);
               
                    }
                    break;
                case "Unequip":
                    InventoryManager.im.AddSingleItem(unEquip);
                    Events.onUnEquip(unEquip, itemIndex);
                    unEquip = null;
                    break;
                case "Discard":
                    Events.onDiscard = true;
                    SpawnDialogue("DiscardItem");

                    break;
            }
            ObjectPooler.op.DeSpawn(gameObject);
        }
    }
    public void EntryLeave(int index)
    {
        uitem[index].anim.SetBool("Hover", false);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ObjectPooler.op.DeSpawn(gameObject);
    }

    private void SpawnDialogue(string objName)
    {
        Debug.Log(objName);
        GameObject obj = ObjectPooler.op.SpawnUI(objName, transform.localPosition.x > 270 ? new Vector2(transform.position.x - 0.75f, transform.position.y) : (Vector2)transform.position, transform.parent);
        if (objName == "DiscardItem")
        {
            DiscardItems ditem = obj.GetComponent<DiscardItems>();
            if (unEquip == null) ditem.Spawn(itemIndex);
            else ditem.Spawn(unEquip);
        }
    }

}
