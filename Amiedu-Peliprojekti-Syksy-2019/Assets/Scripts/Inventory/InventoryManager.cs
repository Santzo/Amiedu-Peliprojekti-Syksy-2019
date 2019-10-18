﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager im;
    [HideInInspector]
    public List<Inventory> filteredItems = new List<Inventory>();
    public Weapon[] weapons;
    public Headgear[] headgear;
    public Lightsource[] lightsources;
    public Consumable[] consumables;
    public Leggear[] leggear;
    private string[] itemTypes = new string[] { "InventoryItems", "Weapon", "Headgear", "Chestgear", "Armgear", "Leggear", "Lightsource", "Consumable" };
    public static string[] itemsToShow = new string[] { "All items", "Weapons", "Head Gear", "Chest Guard", "Arm Guards", "Leg Guards", "Light sources", "Consumables" };
    public static string[] weaponSortBy = new string[] { "Weapon Level", "Damage", "Weapon Type" };


    void Awake()
    {
        im = this;
        weapons = Resources.LoadAll<Weapon>("Inventory/Weapons");
        headgear = Resources.LoadAll<Headgear>("Inventory/Headgear");
        consumables = Resources.LoadAll<Consumable>("Inventory/Consumables");
        leggear = Resources.LoadAll<Leggear>("Inventory/Leggear");
        lightsources = Resources.LoadAll<Lightsource>("Inventory/Lightsources");

        foreach (var weapon in weapons)
        {
            CharacterStats.inventoryItems.Add(new Inventory { amount = UnityEngine.Random.Range(1, 3), item = weapon});
        }
        foreach (var head in headgear)
        {
            CharacterStats.inventoryItems.Add(new Inventory { amount = 1, item = head });
        }
        foreach (var cons in consumables)
        {
            CharacterStats.inventoryItems.Add(new Inventory { amount = UnityEngine.Random.Range(2, 500), item = cons });
        }

        foreach (var lg in leggear)
        {
            CharacterStats.inventoryItems.Add(new Inventory { amount = 1, item = lg });
        }
        foreach (var ls in lightsources)
        {
            CharacterStats.inventoryItems.Add(new Inventory { amount = 1, item = ls });
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

}
