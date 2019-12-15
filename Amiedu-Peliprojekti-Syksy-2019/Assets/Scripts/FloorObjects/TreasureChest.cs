using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TreasureChest : InteractableObject
{
    bool chestOpened = false;
    GameObject chest;
    private List<Inventory> chestContent;
    
    private void OnEnable()
    {
        Events.inventoryKey += DisableThis;
    }

    private void OnDisable()
    {
        Events.inventoryKey -= DisableThis;
    }

    private void DisableThis()
    {
        if (!interacting) return;
        if (Events.onInventory)
            chest.SetActive(false);
        else
            chest.SetActive(true);
    }

    protected override void EnterRange()
    {
        base.EnterRange();
        io = obj.GetComponent<InteractableObjectText>();
        if (!chestOpened)
        {
            io.text.text = $"Press {TextColor.Green}{KeyboardConfig.ReturnKeyName(KeyboardConfig.action[0].ToString())} {TextColor.White} to open the Treasure Chest.";
        }
        else io.text.text = $"Press {TextColor.Green}{KeyboardConfig.ReturnKeyName(KeyboardConfig.action[0].ToString())} {TextColor.White} to search the contents of the Chest.";
        io.ToggleTextActive(true);
    }

    protected override void LeaveRange()
    {
        base.LeaveRange();
        chest?.SetActive(false);
    }
    public override void Interact()
    {
        if (interacting) return;
        base.Interact();
        if (!chestOpened)
        {
            Audio.PlaySound("ChestOpen", 0.95f, 1.5f);
            StartCoroutine(OpenChestWithDelay());
            chestOpened = true;
        }
        else if (chestOpened)
        {
            SpawnChest();
        }
        ps?.Play();
    }
    void SpawnChest()
    {
        chest = ObjectPooler.op.SpawnUI("ChestContent", transform.position, References.rf.uiOverlay);
        ChestContentUI contentUI = chest.GetComponent<ChestContentUI>();
        contentUI.UpdateContents(chestContent, this);
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
    }
    object GetItem(Type type, int level)
    {
        if (type == typeof(Weapon))
        {
            var temp = (from a in InventoryManager.im.weapons
                      where a.itemLevel <= level
                      select a).ToArray();
            if (temp.Length == 0) return null;
            object obj = temp[Random.Range(0, temp.Length)];
            return obj;
        }
        else if (type == typeof(Chestgear))
        {
            var temp = (from a in InventoryManager.im.chestgear
                        where a.itemLevel <= level
                        select a).ToArray();
            if (temp.Length == 0) return null;
            object obj = temp[Random.Range(0, temp.Length)];
            return obj;
        }
        else if (type == typeof(Headgear))
        {
            var temp = (from a in InventoryManager.im.headgear
                        where a.itemLevel <= level
                        select a).ToArray();
            if (temp.Length == 0) return null;
            object obj = temp[Random.Range(0, temp.Length)];
            return obj;
        }
        else if (type == typeof(Leggear))
        {
            var temp = (from a in InventoryManager.im.leggear
                        where a.itemLevel <= level
                        select a).ToArray();
            if (temp.Length == 0) return null;
            object obj = temp[Random.Range(0, temp.Length)];
            return obj;
        }
        else if (type == typeof(Lightsource))
        {
            var temp = (from a in InventoryManager.im.lightsources
                        where a.itemLevel <= level
                        select a).ToArray();
            if (temp.Length == 0) return null;
            object obj = temp[Random.Range(0, temp.Length)];
            return obj;
        }
        else if (type == typeof(Armgear))
        {
            var temp = (from a in InventoryManager.im.armgear
                        where a.itemLevel <= level
                        select a).ToArray();
            if (temp.Length == 0) return null;
            object obj = temp[Random.Range(0, temp.Length)];
            return obj;
        }
        else if (type == typeof(Consumable))
        {
            var temp = (from a in InventoryManager.im.consumables
                        where a.itemLevel <= level
                        select a).ToArray();
            if (temp.Length == 0) return null;
            object obj = temp[Random.Range(0, temp.Length)];
            return obj;
        }
        return null;
    }

    IEnumerator OpenChestWithDelay()
    {
        yield return new WaitForSeconds(0.35f);
        if (inRange) SpawnChest();
    }
    public void AllItemsRetrieved()
    {
        actionTrigger.gameObject.SetActive(false);
        ps.Stop();
    }
}
public class ChestContent 
{
    public bool random;
    public int level;
    public Type type;
    public int amount;

}
