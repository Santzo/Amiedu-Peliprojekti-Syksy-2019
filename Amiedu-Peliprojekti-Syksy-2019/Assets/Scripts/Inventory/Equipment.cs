using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Equipment : MonoBehaviour
{

    private Type current;
    private Child[] equipment;

    public class Child
    {
        public Image placeHolder;
        public Animator anim;
        public InventoryItems item;
        public Image itemIcon;
    }
    private void Awake()
    {
        equipment = new Child[transform.childCount];
        equipment.Populate();
        for (int i = 0; i < transform.childCount; i++)
        {
            equipment[i].anim = transform.GetChild(i).GetComponent<Animator>();
            equipment[i].placeHolder = transform.GetChild(i).Find("IconBackground").GetComponent<Image>();
            equipment[i].itemIcon = transform.GetChild(i).Find("ItemIcon").GetComponent<Image>();
        }
    }
    private void OnEnable()
    {
        Events.onItemHover += ItemHover;
        Events.onItemLeaveHover += ItemLeave;
        Events.onItemDragStop += ItemDragStop;
        foreach (var ani in equipment)
            ani.anim.SetBool("Hover", false);
    }

  

    private void OnDisable()
    {
        Events.onItemHover -= ItemHover;
        Events.onItemLeaveHover -= ItemLeave;
        Events.onItemDragStop -= ItemDragStop;

    }

    private void ItemDragStop(int index, Vector2 pos)
    {
        Vector2 childPos = transform.GetChild(ReturnType(current)).position;
         var distance = new Vector2(Math.Abs(pos.x - childPos.x), Math.Abs(pos.y - childPos.y)) / Info.CanvasScale;
        if (distance.x < 75 && distance.y < 75)
            EquipNewItem(index, InventoryManager.im.filteredItems[index]);

    }

    private void EquipNewItem(int ind, Inventory item)
    {
        var fields = CharacterStats.characterEquipment.GetType().GetFields();
        foreach (var field in fields)
        {
            if (field.FieldType == current)
            {
                var typeInfo = typeof(CharacterEquipment).GetField(field.Name);
                int index = ReturnType(current);

                var temp = item.item;

                if (equipment[index].item is null && item.amount == 1)
                {
                    CharacterStats.inventoryItems.Remove(item);
                    InventoryManager.im.filteredItems.Remove(item);
                    Events.updateFilteredItems(InventoryManager.im.filteredItems);
                }
                else if (!(equipment[index].item is null) && item.amount == 1)
                {
                    item.item = equipment[index].item;
                }
                typeInfo.SetValue(CharacterStats.characterEquipment, item.item);
                equipment[index].placeHolder.enabled = false;
                equipment[index].item = temp;
                equipment[index].itemIcon.sprite = temp.icon;
                equipment[index].itemIcon.color = new Color(1f, 1f, 1f, 1f);
                equipment[index].itemIcon.preserveAspect = true;


                break;
            }
        }
    }

    private void ItemHover(int index, Vector2 pos)
    {
        current = InventoryManager.im.filteredItems[index].item.GetType();
        if (current != typeof(Consumable)) equipment[ReturnType(current)].anim.SetBool("Hover", true);

    }
    private void ItemLeave()
    {
        if (current != typeof(Consumable)) equipment[ReturnType(current)].anim.SetBool("Hover", false);
    }

    private int ReturnType(Type type)
    {
        if (type == typeof(Headgear))
            return 0;
        if (type == typeof(Chestgear))
            return 1;
        if (type == typeof(Armgear))
            return 2;
        if (type == typeof(Leggear))
            return 3;
        if (type == typeof(Weapon))
            return 4;
        if (type == typeof(Lightsource))
            return 5;
        return 6;
    }

    


}
