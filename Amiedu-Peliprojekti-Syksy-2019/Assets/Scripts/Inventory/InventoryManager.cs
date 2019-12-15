using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager im;
    [HideInInspector]
    public List<Inventory> filteredItems = new List<Inventory>();
    public Weapon[] weapons;
    public Headgear[] headgear;
    public Chestgear[] chestgear;
    public Lightsource[] lightsources;
    public Armgear[] armgear;
    public Consumable[] consumables;
    public Leggear[] leggear;
    public Ammo[] ammo;
    private string[] itemTypes = new string[] { "InventoryItems", "Weapon", "Headgear", "Chestgear", "Armgear", "Leggear", "Lightsource", "Consumable" };
    public static string[] itemsToShow = new string[] { "All items", "Weapons", "Head Gear", "Chest Guard", "Arm Guards", "Leg Guards", "Light sources", "Consumables" };
    public static string[] weaponSortBy = new string[] { "Weapon Level", "Damage", "Weapon Type" };


    void Awake()
    {
        im = this;

        //weapons = weapons.LoadAssets("equipment/weapons"); // When building the final product, replace Resources.LoadAll calls with LoadAssets calls

        weapons = Resources.LoadAll<Weapon>("Inventory/Weapons");
        headgear = Resources.LoadAll<Headgear>("Inventory/Headgear");
        chestgear = Resources.LoadAll<Chestgear>("Inventory/Chestgear");
        armgear = Resources.LoadAll<Armgear>("Inventory/Armgear");
        consumables = Resources.LoadAll<Consumable>("Inventory/Consumables");
        leggear = Resources.LoadAll<Leggear>("Inventory/Leggear");
        lightsources = Resources.LoadAll<Lightsource>("Inventory/Lightsources");
        ammo = Resources.LoadAll<Ammo>("Inventory/Ammo");
        foreach (var a in weapons)
        {
            SetMaterialProperties smp = a.obj.GetComponent<SetMaterialProperties>();
            smp?.SetProps();
        }
        foreach (var a in headgear)
        {
            SetMaterialProperties smp = a.obj.GetComponent<SetMaterialProperties>();
            smp?.SetProps();
        }
        foreach (var a in chestgear)
        {
            SetMaterialProperties smp = a.obj.GetComponent<SetMaterialProperties>();
            smp?.SetProps();
        }
        foreach (var a in armgear)
        {
            SetMaterialProperties smp = a.obj.GetComponent<SetMaterialProperties>();
            smp?.SetProps();
        }
        foreach (var a in consumables)
        {
            SetMaterialProperties smp = a.obj.GetComponent<SetMaterialProperties>();
            smp?.SetProps();
        }
        foreach (var a in leggear)
        {
            SetMaterialProperties smp = a.obj.GetComponent<SetMaterialProperties>();
            smp?.SetProps();
        }
        foreach (var a in lightsources)
        {
            SetMaterialProperties smp = a.obj.GetComponent<SetMaterialProperties>();
            smp?.SetProps();
        }
    }


    public void FilterItems(string name)
    {
        filteredItems = new List<Inventory>();
        int index = 0;
        for (int i = 0; i < itemsToShow.Length; i++)
        {
            if (name == itemsToShow[i])
            {
                index = i;
                break;
            }
        }
        filteredItems = index != 0 ? CharacterStats.inventoryItems.FindAll(item => item.item.GetType().ToString() == itemTypes[index]) : CharacterStats.inventoryItems;
        Events.updateFilteredItems(filteredItems);

    }
    public void RemoveItems(int index, int amount)
    {
        Inventory item = filteredItems[index];
        if (amount == item.amount)
        {
            CharacterStats.inventoryItems.Remove(item);
            filteredItems.Remove(item);
        }
        else
        {
            item.amount -= amount;
        }
        Events.updateFilteredItems(filteredItems);
        Events.onInventoryChange();
    }
    public void AddToInventory(Inventory inv)
    {
        if (inv.item.GetType() != typeof(Ammo))
        {
            var tempItem = CharacterStats.inventoryItems.Find(_item => _item.item == inv.item);
            if (tempItem != null) tempItem.amount += inv.amount;
            else
            {
                CharacterStats.inventoryItems.Add(new Inventory { amount = inv.amount, item = inv.item });
            }
            Events.updateFilteredItems(filteredItems);
            Events.onInventoryChange();
        }
        else
        {
            Ammo ammo = inv.item as Ammo;
            if (ammo.ammoType == Ammo.AmmoType.Gasoline) CharacterStats.gasAmmo += inv.amount;
            else if (ammo.ammoType == Ammo.AmmoType.Pistol) CharacterStats.pistolAmmo += inv.amount;
            else if (ammo.ammoType == Ammo.AmmoType.Rifle) CharacterStats.rifleAmmo += inv.amount;
        }
    }
    public void AddSingleItem(InventoryItems itemToAdd)
    {
        var tempItem = CharacterStats.inventoryItems.Find(_item => _item.item == itemToAdd);
        if (tempItem != null) tempItem.amount++;
        else
        {
            CharacterStats.inventoryItems.Add(new Inventory { amount = 1, item = itemToAdd });
        }
        Events.updateFilteredItems(filteredItems);
        Events.onInventoryChange();
    }
}


