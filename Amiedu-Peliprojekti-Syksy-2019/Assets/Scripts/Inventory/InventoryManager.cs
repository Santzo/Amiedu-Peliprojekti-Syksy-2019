using System;
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
    public Consumable[] consumables;
    private string[] itemTypes = new string[] { "InventoryItems", "Weapon", "Headgear", "Chestgear", "Armgear", "Leggear", "Lightsource", "Consumable" };
    public static string[] itemsToShow = new string[] { "All items", "Weapons", "Head Gear", "Chest Guard", "Arm Guards", "Leg Guards", "Light sources", "Consumables" };
    public static string[] weaponSortBy = new string[] { "Weapon Level", "Damage", "Weapon Type" };


    void Awake()
    {
        im = this;
        weapons = Resources.LoadAll<Weapon>("Inventory/Weapons");
        headgear = Resources.LoadAll<Headgear>("Inventory/Headgear");
        consumables = Resources.LoadAll<Consumable>("Inventory/Consumables");

        foreach (var weapon in weapons)
        {
            CharacterStats.inventoryItems.Add(new Inventory { amount = 1, item = weapon});
        }
        foreach (var head in headgear)
        {
            CharacterStats.inventoryItems.Add(new Inventory { amount = 1, item = head });
        }
        foreach (var cons in consumables)
        {
            CharacterStats.inventoryItems.Add(new Inventory { amount = UnityEngine.Random.Range(2, 500), item = cons });
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
