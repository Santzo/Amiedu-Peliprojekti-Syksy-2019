using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Equipment : MonoBehaviour
{
    private GameObject _obj;
    private Type current;
    private Child[] equipment;
    private string[] types = new string[] { "head", "chest", "arms", "legs", "weapon", "lightSource" };

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
        Events.onEquipmentIconHover += EquipmentHover;
        Events.onEquipmentIconHoverLeave += EquipmentHoverLeave;
        Events.onIconDoubleClick += EquipNewItem;
        foreach (var ani in equipment)
            ani.anim.SetBool("Hover", false);
    }

  
    private void OnDisable()
    {
        Events.onItemHover -= ItemHover;
        Events.onItemLeaveHover -= ItemLeave;
        Events.onItemDragStop -= ItemDragStop;
        Events.onEquipmentIconHover -= EquipmentHover;
        Events.onEquipmentIconHoverLeave -= EquipmentHoverLeave;
        Events.onIconDoubleClick -= EquipNewItem;
        if (_obj != null) ObjectPooler.op.DeSpawn(_obj);

    }

    private void EquipmentHover(int obj)
    {
        equipment[obj].anim.SetBool("Hover", true);
        EquipmentItemDetails(obj);
    }

    private void EquipmentHoverLeave(int obj)
    {
        equipment[obj].anim.SetBool("Hover", false);
        if (_obj != null) ObjectPooler.op.DeSpawn(_obj);
    }


    private void EquipmentItemDetails(int obj)
    {
        var info = typeof(CharacterEquipment).GetField(types[obj]);
        if (info.GetValue(CharacterStats.characterEquipment) != null)
        {
            InventoryItems item = info.GetValue(CharacterStats.characterEquipment) as InventoryItems;
            Vector2 pos = transform.GetChild(obj).transform.position;
            pos = obj < 4 ? pos : new Vector2(pos.x - 2f * Info.CanvasScale, pos.y);
            _obj = InventoryGrid.ShowItemDetails(item, pos, transform, true);
        }
    }

    private void ItemDragStop(int index, Vector2 pos)
    {
        Vector2 childPos = transform.GetChild(ReturnType(current)).position;
        var distance = new Vector2(Math.Abs(pos.x - childPos.x), Math.Abs(pos.y - childPos.y)) / Info.CanvasScale;
        if (distance.x < 75 && distance.y < 75)
            EquipNewItem(index, InventoryManager.im.filteredItems[index]);
        ItemLeave();

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
                var _item = CharacterStats.inventoryItems.Find(inv => inv.item == equipment[index].item);

                switch (item.amount)
                {
                    case 1:

                        if (equipment[index].item is null)
                        {
                            CharacterStats.inventoryItems.Remove(item);
                            InventoryManager.im.filteredItems.Remove(item);
                        }

                        else if (!(equipment[index].item is null))
                        {
                            if (equipment[index].item.Equals(item.item))
                            {
                                break;
                            }
                            else if (_item is null)
                            {
                                item.item = equipment[index].item;
                            }
                            else
                            {
                                _item.amount++;
                                CharacterStats.inventoryItems.Remove(item);
                                InventoryManager.im.filteredItems.Remove(item);
                            }
                        }
                        break;
                    default:
                        if (equipment[index].item is null)
                        {
                            item.amount--;
                        }
                        else if (!(equipment[index].item is null) && equipment[index].item.Equals(item.item))
                        {
                            break;
                        }
                        else
                        {

                            if (_item is null)
                            {
                                CharacterStats.inventoryItems.Add(new Inventory { amount = 1, item = equipment[index].item });
                                item.amount--;
                            }
                            else
                            {
                                _item.amount++;
                                item.amount--;
                            }
                        }
                        break;
                }

                typeInfo.SetValue(CharacterStats.characterEquipment, item.item);
                equipment[index].placeHolder.enabled = false;
                equipment[index].item = temp;
                equipment[index].itemIcon.sprite = temp.icon == null ? temp.obj.GetComponent<SpriteRenderer>().sprite : temp.icon;
                equipment[index].itemIcon.color = new Color(1f, 1f, 1f, 1f);
                equipment[index].itemIcon.preserveAspect = true;
                GameObject obj = equipment[index].item.obj;
                if (obj != null) PlayerEquipment.AddEquipment(obj, equipment[index].item);
                Events.updateFilteredItems(InventoryManager.im.filteredItems);


                break;
            }
        }
    }

    private void ItemHover(int index, Vector2 pos)
    {
        current = InventoryManager.im.filteredItems[index].item.GetType();
        if (current != typeof(Consumable)) equipment[ReturnType(current)].anim.SetBool("Hover", true);
        if (ReturnType(current) < 6) EquipmentItemDetails(ReturnType(current));

    }
    private void ItemLeave()
    {
        if (current != typeof(Consumable)) equipment[ReturnType(current)].anim.SetBool("Hover", false);
        if (_obj != null) ObjectPooler.op.DeSpawn(_obj);
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
