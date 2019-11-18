using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Info
{
    private static Canvas canvas = GameObject.Find("UIOverlay").GetComponent<Canvas>();
    public static Camera camera = Camera.main;
    public static GameObject player = GameObject.Find("Player");
    public static float CanvasScale { get { return canvas.scaleFactor; } }
    public static Transform content;
    public static PlayerEquipment playerEquipment = GameObject.Find("PlayerEquipment").GetComponent<PlayerEquipment>();
    public static float minDamage { get; private set; }
    public static float maxDamage { get; private set; }
    public static float totalCriticalHitChance { get; private set; }
    public static float totalAttackSpeed { get; private set; }
    public static float attackInterval { get; private set; }
    public static List<Collider2D> enemyHitboxes = new List<Collider2D>();


    public static int SortingOrder(float yPos)
    {
        yPos *= 100;
        return Mathf.RoundToInt(-yPos);
    }

    public static void AddEnemyHitbox(Collider2D hitbox)
    {
        enemyHitboxes.Add(hitbox);
    }
    public static void RemoveEnemyHitbox(Collider2D hitbox)
    {
        enemyHitboxes.Remove(hitbox);

    }

    public static float BaseHealth
    {
        get
        {
            int temp = CharacterStats.constitution;
            float _value = 0f;
            for (int i = 50; temp > 0; i -= 5)
            {
                int add = temp >= 10 ? 10 : temp;
                float addValue = Mathf.Clamp(i * 0.2f, 2f, 10f);
                _value += addValue * add;
                temp -= 10;
            }
            temp = CharacterStats.strength;
            for (int i = 50; temp > 10; i -= 10)
            {
                int add = temp >= 10 ? 10 : temp;
                float addValue = Mathf.Clamp(i * 0.05f, 0.05f, 5f);
                _value += addValue * add;
                temp -= 10;
            }
            return _value;
        }
    }
    public static float BaseStamina
    {
        get
        {
            int temp = CharacterStats.constitution;
            float _value = 0f;
            for (int i = 50; temp > 0; i -= 5)
            {
                int add = temp >= 10 ? 10 : temp;
                float addValue = Mathf.Clamp(i * 0.175f, 1.75f, 10f);
                _value += addValue * add;
                temp -= 10;
            }
            temp = CharacterStats.strength;
            for (int i = 50; temp > 10; i -= 10)
            {
                int add = temp >= 10 ? 10 : temp;
                float addValue = Mathf.Clamp(i * 0.03f, 0.03f, 5f);
                _value += addValue * add;
                temp -= 10;
            }
            return _value;
        }
    }

    public static float BaseSight
    {
        get { return 1.25f + CharacterStats.perception * 0.05f; }
    }

    public static float SightRadius
    {
        get
        {
            if (References.rf.playerEquipment != null)
                return References.rf.playerEquipment.LOSCircle.transform.localScale.x;
            return 0f;
        }
    }

    public static void CalculateDefenses()
    {
        CharacterStats.totalPhysicalDefense = CharacterStats.physicalDefenseFromItems + CharacterStats.physicalDefensePercentage;
        CharacterStats.totalFireDefense = CharacterStats.fireDefenseFromItems + CharacterStats.fireDefensePercentage;
        CharacterStats.totalSpectralDefense = CharacterStats.spectralDefenseFromItems + CharacterStats.spectralDefensePercentage;
    }

    public static float BaseCritical
    {
        get
        {
            int temp = CharacterStats.perception;
            float _value = 0f;
            for (int i = 50; temp > 10; i -= 10)
            {
                int add = temp >= 10 ? 10 : temp;
                float addValue = Mathf.Clamp(i * 0.02f, 0.2f, 1f);
                _value += addValue * add;
                temp -= 10;
            }
            temp = CharacterStats.luck;
            for (int i = 50; temp > 10; i -= 10)
            {
                int add = temp >= 10 ? 10 : temp;
                float addValue = Mathf.Clamp(i * 0.005f, 0.05f, 1f);
                _value += addValue * add;
                temp -= 10;
            }
            return _value;
        }
    }
    public static float BaseAttackSpeed
    {
        get
        {
            int temp = CharacterStats.dexterity;
            float _value = 0f;
            for (int i = 50; temp > 10; i -= 10)
            {
                int add = temp >= 10 ? 10 : temp;
                float addValue = Mathf.Clamp(i * 0.00025f, 0.002f, 1f);
                _value += addValue * add;
                temp -= 10;
            }
            return 1f + _value;
        }
    }
    public static int StatsMinDamage
    {
        get
        {
            float dmg = CharacterStats.characterEquipment.weapon != null ? (int)CharacterStats.characterEquipment.weapon.minDamage : 0;
            float statBonus = CharacterStats.characterEquipment.weapon.weaponType == WeaponType.Melee ? CharacterStats.strength * 0.05f * (dmg * 0.1f) : CharacterStats.dexterity * 0.05f * (dmg * 0.1f);
            dmg += statBonus;
            minDamage = dmg;
            return Mathf.RoundToInt(dmg);
        }
    }

    public static int StatsMaxDamage
    {
        get
        {
            float dmg = CharacterStats.characterEquipment.weapon != null ? (int)CharacterStats.characterEquipment.weapon.maxDamage : 0;
            float statBonus = CharacterStats.characterEquipment.weapon.weaponType == WeaponType.Melee ? CharacterStats.strength * 0.05f * (dmg * 0.1f) : CharacterStats.dexterity * 0.05f * (dmg * 0.1f);
            dmg += statBonus;
            maxDamage = dmg;
            return Mathf.RoundToInt(dmg);
        }
    }

    public static int CalculateDamage(EnemyStats enemyStats)
    {
        float dmg = Random.Range(minDamage, maxDamage);
        float defense = enemyStats.defense;
        int totalDmg = dmg - defense > 1f ? Mathf.RoundToInt(dmg - defense) : 1;
        return totalDmg;
    }

    internal static void AddDefenses(Armor armor, bool unEquip = false)
    {
        CharacterStats.physicalDefenseFromItems += !unEquip ? armor.defense : -armor.defense;
        CharacterStats.fireDefenseFromItems += !unEquip ? armor.fireDefense : -armor.fireDefense;
        CharacterStats.spectralDefenseFromItems += !unEquip ? armor.spectralDefense : -armor.spectralDefense;
    }

    public static void CalculateCriticalHitChance()
    {
        float weaponCrit = CharacterStats.characterEquipment.weapon != null ? CharacterStats.characterEquipment.weapon.criticalHitChance : 0f;
        float percent = 1f + (CharacterStats.criticalBonusPercentage / 100f);
        totalCriticalHitChance = (weaponCrit + BaseCritical) * percent;
    }

    public static void CalculateAttackSpeed()
    {
        float weaponSpeed = CharacterStats.characterEquipment.weapon != null ? CharacterStats.characterEquipment.weapon.fireRate : 0f;
        totalAttackSpeed = weaponSpeed * BaseAttackSpeed;

        attackInterval = totalAttackSpeed > 0f ? 1f / totalAttackSpeed : 0f;
    }

    public static void CalculateSightRange()
    {
        float percent = 1f + (CharacterStats.sightBonusPercentage / 100f);
        float sight = (BaseSight + CharacterStats.sightBonusFromItems) * percent;
        References.rf.playerEquipment.LOSCircle.transform.localScale = Vector3.one * sight;
        References.rf.playerMovement.mask.localScale = Vector3.one * References.rf.playerEquipment.LOSCircle.transform.localScale.x * References.rf.playerMovement.transform.localScale.y;
    }
    public static void CalculateHealthAndStamina()
    {
        CharacterStats.maxHealth = Mathf.Ceil(BaseHealth + CharacterStats.healthBonusFromItems + CharacterStats.healthBonusPercentage);
        CharacterStats.maxStamina = Mathf.Ceil(12.5f + BaseStamina + CharacterStats.staminaBonusFromItems + CharacterStats.staminaBonusPercentage);
        if (CharacterStats.health > CharacterStats.maxHealth) CharacterStats.health = CharacterStats.maxHealth;
        if (CharacterStats.stamina > CharacterStats.maxStamina) CharacterStats.stamina = CharacterStats.maxStamina;
        References.rf.healthBar.ChangeValues(CharacterStats.health, CharacterStats.maxHealth);
        References.rf.staminaBar.ChangeValues(CharacterStats.stamina, CharacterStats.maxStamina);
    }
}
