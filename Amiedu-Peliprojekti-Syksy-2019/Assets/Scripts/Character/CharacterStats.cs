using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CharacterStats
{
    private static int essence;

    public static string name = "Player";
    public static int strength;
    public static int constitution;
    public static int dexterity;
    public static int perception;
    public static int luck;
    public static float weightLimit;
    public static List<Inventory> inventoryItems = new List<Inventory>();
    public static CharacterEquipment characterEquipment = new CharacterEquipment();
    public static Inventory[] hotbar = new Inventory[4];
    private static float health, maxHealth, stamina, maxStamina;
    public static float Health { get { return health; } set {if (value == health) return; health = value; References.rf.healthBar.UpdateValue(health); } }
    public static float MaxHealth { get { return maxHealth; } set { if (value == maxHealth) return; maxHealth = value; References.rf.healthBar.ChangeValues(health, maxHealth); } }
    public static float Stamina { get { return stamina; } set { if (value == stamina) return; stamina = value; References.rf.staminaBar.UpdateValue(stamina); } }
    public static float MaxStamina { get { return maxStamina; } set { if (value == maxStamina) return; maxStamina = value; References.rf.staminaBar.ChangeValues(stamina, maxStamina); } }
    public static float staminaRegenerationRate, movementSpeedMultiplier, attackSpeed, moveSpeed, animationBaseMoveSpeed, animationSprintMoveSpeed;
    public static float sightBonusFromItems, sightBonusPercentage, criticalBonusFromItems, criticalBonusPercentage;
    public static float healthBonusFromItems, healthBonusPercentage, staminaBonusFromItems, staminaBonusPercentage;
    public static int gasAmmo, pistolAmmo, rifleAmmo;
    public static float totalPhysicalDefense, physicalDefenseFromItems, physicalDefensePercentage;
    public static float totalSpectralDefense, spectralDefenseFromItems, spectralDefensePercentage;
    public static float totalFireDefense, fireDefenseFromItems, fireDefensePercentage;
    public static int Essence { get { return essence; } set { if (value <= 0) return; essence = value; Events.onEssenceChanged(Essence);  } }


    public static void ResetStats()
    {
        hotbar.Populate();
        hotbar[0] = new Inventory{ amount = 1, item = InventoryManager.im.consumables[0] };
        strength = 10;
        constitution = 10;
        dexterity = 10;
        perception = 10;
        luck = 10;
        weightLimit = Mathf.Round(((strength * 10f) + (constitution * 1.43f)) * 10f) / 10f;
        Info.CalculateHealthAndStamina();
        Health = MaxHealth;
        Stamina = MaxStamina;
        staminaRegenerationRate = 3f;
        movementSpeedMultiplier = 1.7f;
        attackSpeed = 1f;
        moveSpeed = 5f;
        Info.CalculateAnimationSpeeds();
        sightBonusFromItems = sightBonusPercentage = criticalBonusFromItems = criticalBonusPercentage = 0f;
        healthBonusFromItems = healthBonusPercentage = staminaBonusFromItems = staminaBonusPercentage = 0f;
        gasAmmo = 1000;
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
