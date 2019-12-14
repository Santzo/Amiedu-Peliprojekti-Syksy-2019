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
    public static float minPhys { get; private set; }
    public static float maxPhys { get; private set; }
    public static float minFire { get; private set; }
    public static float maxFire { get; private set; }
    public static float minSpectral { get; private set; }
    public static float maxSpectral { get; private set; }
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
                float addValue = Mathf.Clamp(i * 0.0175f, 0.2f, 1f);
                _value += addValue * add;
                temp -= 10;
            }
            temp = CharacterStats.luck;
            for (int i = 50; temp > 10; i -= 10)
            {
                int add = temp >= 10 ? 10 : temp;
                float addValue = Mathf.Clamp(i * 0.004f, 0.05f, 1f);
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
            if (CharacterStats.characterEquipment.weapon == null) return 0;
            Weapon wep = CharacterStats.characterEquipment.weapon;
            float physBonus = CharacterStats.characterEquipment.weapon.weaponType == WeaponType.Melee ? CharacterStats.strength * 0.05f * (wep.physicalMin * 0.1f) : CharacterStats.dexterity * 0.05f * (wep.physicalMin * 0.1f);
            float fireBonus = CharacterStats.characterEquipment.weapon.weaponType == WeaponType.Melee ? CharacterStats.strength * 0.05f * (wep.fireMin * 0.1f) : CharacterStats.dexterity * 0.05f * (wep.fireMin * 0.1f);
            float specBonus = CharacterStats.characterEquipment.weapon.weaponType == WeaponType.Melee ? CharacterStats.strength * 0.05f * (wep.spectralMin * 0.1f) : CharacterStats.dexterity * 0.05f * (wep.spectralMin * 0.1f);
            float luckmulti = 1f + (CharacterStats.luck / 1500f);
            minPhys = (wep.physicalMin + physBonus) * luckmulti;
            minFire = (wep.fireMin + fireBonus) * luckmulti;
            minSpectral = (wep.spectralMin + specBonus) * luckmulti;
            return Mathf.RoundToInt(minPhys + minFire + minSpectral);
        }
    }

    public static int StatsMaxDamage
    {
        get
        {
            if (CharacterStats.characterEquipment.weapon == null) return 0;
            Weapon wep = CharacterStats.characterEquipment.weapon;
            float physBonus = CharacterStats.characterEquipment.weapon.weaponType == WeaponType.Melee ? CharacterStats.strength * 0.05f * (wep.physicalMax * 0.1f) : CharacterStats.dexterity * 0.05f * (wep.physicalMax * 0.1f);
            float fireBonus = CharacterStats.characterEquipment.weapon.weaponType == WeaponType.Melee ? CharacterStats.strength * 0.05f * (wep.fireMax * 0.1f) : CharacterStats.dexterity * 0.05f * (wep.fireMax * 0.1f);
            float specBonus = CharacterStats.characterEquipment.weapon.weaponType == WeaponType.Melee ? CharacterStats.strength * 0.05f * (wep.spectralMax * 0.1f) : CharacterStats.dexterity * 0.05f * (wep.spectralMax * 0.1f);
            float luckmulti = 1f + (CharacterStats.luck / 1500f);
            maxPhys = (wep.physicalMax + physBonus) * luckmulti;
            maxFire = (wep.fireMax + fireBonus) * luckmulti;
            maxSpectral = (wep.spectralMax + specBonus) * luckmulti;
            return Mathf.RoundToInt(maxPhys + maxFire + maxSpectral);
        }
    }

    public static int CalculateDamage(EnemyStats enemyStats, out bool crit)
    {
        crit = Random.Range(0f, 100f) < totalCriticalHitChance;
        float physDmg = Mathf.Max(Random.Range(minPhys, maxPhys) - enemyStats.physicalDefense,0);
        float specDmg = Mathf.Max(Random.Range(minSpectral, maxSpectral) - enemyStats.spectralDefense,0);
        float fireDmg = Mathf.Max(Random.Range(minFire, maxFire) - enemyStats.fireDefense,0);
        float totalDmg = (physDmg + specDmg + fireDmg) * (crit ? 1.5f : 1f);
        int returnDmg = totalDmg > 1 ? Mathf.RoundToInt(totalDmg) : Random.Range(0, 2);
        return returnDmg;
    }
    public static int CalculateEnemyDamage(EnemyAttack enemyAttack)
    {
        float physDmg = Mathf.Max(Random.Range(enemyAttack.minPhysical, enemyAttack.maxPhysical) - CharacterStats.physicalDefenseFromItems, 0);
        float specDmg = Mathf.Max(Random.Range(enemyAttack.minSpectral, enemyAttack.maxSpectral) - CharacterStats.spectralDefenseFromItems, 0);
        float fireDmg = Mathf.Max(Random.Range(enemyAttack.minFire, enemyAttack.maxFire) - CharacterStats.fireDefenseFromItems, 0);

        float totalDmg = physDmg + specDmg + fireDmg;
        int returnDmg = totalDmg > 1 ? Mathf.RoundToInt(totalDmg) : Random.Range(0, 2);
        return returnDmg;
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
        float weaponSpeed = CharacterStats.characterEquipment.weapon != null ? CharacterStats.characterEquipment.weapon.attackRate : 0f;
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
        CharacterStats.MaxHealth = Mathf.Ceil(BaseHealth + CharacterStats.healthBonusFromItems + CharacterStats.healthBonusPercentage);
        CharacterStats.MaxStamina = Mathf.Ceil(12.5f + BaseStamina + CharacterStats.staminaBonusFromItems + CharacterStats.staminaBonusPercentage);
        if (CharacterStats.Health > CharacterStats.MaxHealth) CharacterStats.Health = CharacterStats.MaxHealth;
        if (CharacterStats.Stamina > CharacterStats.MaxStamina) CharacterStats.Stamina = CharacterStats.MaxStamina;
    }
    public static void CalculateAnimationSpeeds()
    {
        CharacterStats.animationBaseMoveSpeed = CharacterStats.moveSpeed / 5f;
        CharacterStats.animationSprintMoveSpeed = CharacterStats.animationBaseMoveSpeed * CharacterStats.movementSpeedMultiplier;
    }
  
}
