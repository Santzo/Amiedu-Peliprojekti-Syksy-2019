using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CharacterStats
{
    public static int strength;
    public static int constitution;
    public static int dexterity;
    public static int adaptability;
    public static int luck;
    public static float weightLimit;
    public static List<Inventory> inventoryItems = new List<Inventory>();
    public static CharacterEquipment characterEquipment = new CharacterEquipment();
    public static Inventory[] hotbar = new Inventory[4];


    public static void ResetStats()
    {
        hotbar.Populate();
        hotbar[0] = new Inventory{ amount = 1, item = InventoryManager.im.consumables[0] };
        strength = 10;
        constitution = 11;
        dexterity = 10;
        adaptability = 10;
        luck = 10;
        weightLimit = Mathf.Round(((strength * 10f) + (constitution * 1.43f)) * 10f) / 10f;

    }
}

public class CharacterEquipment
{
    public Headgear head;
    public Chestgear chest;
    public Leggear legs;
    public Armgear arms;
    public Weapon weapon;
    public Lightsource lightSource;
}



public class Inventory
{
    public InventoryItems item;
    public int amount;
}
