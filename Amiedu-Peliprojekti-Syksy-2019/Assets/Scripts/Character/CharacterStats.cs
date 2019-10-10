using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CharacterStats
{
    public static int strength;
    public static int constitution;
    public static int dexterity;
    public static int luck;
    public static List<InventoryItems> inventoryItems = new List<InventoryItems>();

    public static void ResetStats()
    {
        strength = 10;
        constitution = 10;
        dexterity = 10;
        luck = 10;
    }
}
