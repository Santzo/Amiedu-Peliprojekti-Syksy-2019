using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager im;
    [HideInInspector]
    public List<InventoryItems> filteredItems = new List<InventoryItems>();
    public Weapon[] weapons;
    public Headgear[] headgear;
    private string[] itemTypes = new string[] { "InventoryItems", "Weapon", "Headgear", "Chestguard", "Armguard", "Legguard", "Consumable" };

    public static string[] itemsToShow = new string[] { "All items", "Weapons", "Head Gear", "Chest Guard", "Arm Guards", "Leg Guards", "Consumables" };
    public static string[] weaponSortBy = new string[] { "Weapon Level", "Damage", "Weapon Type" };


    void Awake()
    {
        im = this;
        weapons = Resources.LoadAll<Weapon>("Inventory/Weapons");
        headgear = Resources.LoadAll<Headgear>("Inventory/Headgear");

        foreach (var weapon in weapons)
        {
            CharacterStats.inventoryItems.Add(weapon);
        }
        foreach (var head in headgear)
        {
            CharacterStats.inventoryItems.Add(head);
        }
    }


    public void FilterItems(string name)
    {
        filteredItems = new List<InventoryItems>();
        int index = 0;
        for (int i = 0; i < itemsToShow.Length; i++)
        {
            if (name == itemsToShow[i])
            {
                index = i;
                break;
            }
        }
        filteredItems = index != 0 ? CharacterStats.inventoryItems.FindAll(item => item.GetType().ToString() == itemTypes[index]) : CharacterStats.inventoryItems;
        Events.updateFilteredItems(filteredItems);

    }

}
