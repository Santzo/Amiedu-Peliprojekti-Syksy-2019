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
    public static List<Inventory> inventoryItems = new List<Inventory>();

   
    public static void ResetStats()
    {
        strength = 10;
        constitution = 10;
        dexterity = 10;
        adaptability = 10;
        luck = 10;
    }
}

public class Inventory
{
    public InventoryItems item;
    public int amount;
}
