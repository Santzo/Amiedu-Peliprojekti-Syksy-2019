using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TreasureChest : InteractableObject
{
    bool chestOpened = false;
    private List<Inventory> chestContent;

    protected override void InRange()
    {
        base.InRange();
        io = obj.GetComponent<InteractableObjectText>();
        if (!chestOpened)
        {
            io.text.text = $"Press {TextColor.Green}{KeyboardConfig.ReturnKeyName(KeyboardConfig.action[0].ToString())} {TextColor.White} to open the Treasure Chest.";

        }
        else io.text.text = $"Press {TextColor.Green}{KeyboardConfig.ReturnKeyName(KeyboardConfig.action[0].ToString())} {TextColor.White} to search the content of the Chest.";
        io.ToggleTextActive(true);
    }

    public override void Interact()
    {
        if (!chestOpened)
        {
            Audio.PlaySound("ChestOpen", 0.95f, 1.5f);
          
            chestOpened = true;
        }
        base.Interact();
    }

    public void CreateChestContent(params ChestContent[] content)
    {
        chestContent = new List<Inventory>();
        foreach (var cont in content)
        {
            if (cont.random)
            {
                var temp = GetItem(cont.type, cont.level);
                if (temp is Weapon) chestContent.Add(new Inventory {amount = Mathf.Max(cont.amount, 1), item =  temp as Weapon });
                else if (temp is Lightsource) chestContent.Add(new Inventory { amount = Mathf.Max(cont.amount, 1), item = temp as Lightsource });
                else if (temp is Chestgear) chestContent.Add(new Inventory { amount = Mathf.Max(cont.amount, 1), item = temp as Chestgear });
                else if (temp is Headgear) chestContent.Add(new Inventory { amount = Mathf.Max(cont.amount, 1), item = temp as Headgear});
                else if (temp is Armgear) chestContent.Add(new Inventory { amount = Mathf.Max(cont.amount, 1), item = temp as Armgear});
                else if (temp is Leggear) chestContent.Add(new Inventory { amount = Mathf.Max(cont.amount, 1), item = temp as Leggear});
                else if (temp is Consumable) chestContent.Add(new Inventory { amount = Mathf.Max(cont.amount, 1), item = temp as Consumable });
            }
        }
        foreach (var a in chestContent)
            Debug.Log(a.item.name);
    }
    object GetItem(Type type, int level)
    {
        if (type == typeof(Weapon))
        {
            var temp = (from a in InventoryManager.im.weapons
                      where a.itemLevel == level
                      select a).ToArray();
            object obj = temp[Random.Range(0, temp.Length)];
            return obj;
        }
        else if (type == typeof(Chestgear))
        {
            var temp = (from a in InventoryManager.im.chestgear
                        where a.itemLevel == level
                        select a).ToArray();
            object obj = temp[Random.Range(0, temp.Length)];
            return obj;
        }
        else if (type == typeof(Headgear))
        {
            var temp = (from a in InventoryManager.im.headgear
                        where a.itemLevel == level
                        select a).ToArray();
            object obj = temp[Random.Range(0, temp.Length)];
            return obj;
        }
        else if (type == typeof(Leggear))
        {
            var temp = (from a in InventoryManager.im.leggear
                        where a.itemLevel == level
                        select a).ToArray();
            object obj = temp[Random.Range(0, temp.Length)];
            return obj;
        }
        else if (type == typeof(Lightsource))
        {
            var temp = (from a in InventoryManager.im.lightsources
                        where a.itemLevel == level
                        select a).ToArray();
            object obj = temp[Random.Range(0, temp.Length)];
            return obj;
        }
        else if (type == typeof(Armgear))
        {
            var temp = (from a in InventoryManager.im.armgear
                        where a.itemLevel == level
                        select a).ToArray();
            object obj = temp[Random.Range(0, temp.Length)];
            return obj;
        }
        else if (type == typeof(Consumable))
        {
            var temp = (from a in InventoryManager.im.consumables
                        where a.itemLevel == level
                        select a).ToArray();
            object obj = temp[Random.Range(0, temp.Length)];
            return obj;
        }
        return null;
    }
}
public struct ChestContent 
{
    public bool random;
    public int level;
    public Type type;
    public int amount;

}
