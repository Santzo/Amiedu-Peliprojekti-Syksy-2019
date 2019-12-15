using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEquipment : MonoBehaviour
{
    public Dictionary<string, Equipped> equipment = new Dictionary<string, Equipped>();
    private Dictionary<string, SpriteRenderer> chestgearEquipment = new Dictionary<string, SpriteRenderer>();
    private Dictionary<string, SpriteRenderer> leggearEquipment = new Dictionary<string, SpriteRenderer>();
    private Material defaultMat;
    private Lightsource curLight;
    [HideInInspector]
    public Transform LOSCircle;
    private Transform twoHandedLightSource;
    private Transform oneHandedLightSource;
    [HideInInspector]
    public Animator anim;
    public AnimatorOverrideController overrider;
    private Animations animations;

    private void Awake()
    {
        var gear = CharacterStats.characterEquipment.GetType().GetFields();
        foreach (var g in gear)
        {
            Transform trans = transform.parent.GetFromAllChildren(g.FieldType.ToString());
            equipment.Add(trans.name, new Equipped { trans = trans, item = null, obj = null });
        }
        anim = transform.parent.GetComponent<Animator>();
        animations = transform.parent.GetComponent<Animations>();
        overrider = new AnimatorOverrideController();
        defaultMat = new Material(Shader.Find("Sprites/Default"));
        overrider.runtimeAnimatorController = anim.runtimeAnimatorController;
    }

    private void Start()
    {
        Events.onAddPlayerEquipment += AddEquipment;
        LOSCircle = transform.parent.Find("MainFogCircle");
        twoHandedLightSource = transform.parent.GetFromAllChildren("TwoHandedLightSource");
        oneHandedLightSource = transform.parent.GetFromAllChildren("Lightsource");
        References.rf.playerMovement.mask = transform.parent.Find("VisionMask");
        References.rf.playerMovement.mask.SetParent(null);
        CharacterStats.ResetStats();
        CalculateStats();
        chestgearEquipment.Add("ChestgearEquip", transform.parent.GetFromAllChildren("ChestgearEquip").GetComponent<SpriteRenderer>());
        chestgearEquipment.Add("Mid_SectionEquip", transform.parent.GetFromAllChildren("Mid_SectionEquip").GetComponent<SpriteRenderer>());
        chestgearEquipment.Add("R_Upper_ArmEquip", transform.parent.GetFromAllChildren("R_Upper_ArmEquip").GetComponent<SpriteRenderer>());
        chestgearEquipment.Add("R_Lower_ArmEquip", transform.parent.GetFromAllChildren("R_Lower_ArmEquip").GetComponent<SpriteRenderer>());
        chestgearEquipment.Add("L_Upper_ArmEquip", transform.parent.GetFromAllChildren("L_Upper_ArmEquip").GetComponent<SpriteRenderer>());
        chestgearEquipment.Add("L_Lower_ArmEquip", transform.parent.GetFromAllChildren("L_Lower_ArmEquip").GetComponent<SpriteRenderer>());
    }

    private IEnumerator UpdateInventoryGear()
    {
        yield return null;
        References.rf.inventoryScreenCharacter.UpdateEquipment(transform.parent.Find("Chestgear").gameObject);
    }

    void CalculateStats()
    {
        Info.CalculateSightRange();
        Info.CalculateCriticalHitChance();
        Info.CalculateAttackSpeed();
        Info.CalculateHealthAndStamina();
        Info.CalculateDefenses();
        if (CharacterStats.characterEquipment.weapon != null)
        {
            switch (CharacterStats.characterEquipment.weapon.weaponType)
            {
                case WeaponType.Flamethrower:
                    anim.SetFloat("AttackSpeed", 1f);
                    break;
                default:
                    anim.SetFloat("AttackSpeed", Info.totalAttackSpeed);
                    break;
            }
        }
        References.rf.weaponSlot.UpdateWeaponSlot();
        Info.CalculateAnimationSpeeds();
        Info.UpdateWeightInfo();
    }

    private void OnDisable()
    {
        Events.onAddPlayerEquipment -= AddEquipment;
    }

    public void AddEquipment(GameObject obj, InventoryItems item)
    {
        var equip = equipment[item.GetType().ToString()];
        if (equip.item != null && equip.item.Equals(item)) return;
        if (equip.item != null) RemoveEquipment(equip.item, true);
        equip.item = item;
      
        if (item.GetType() != typeof(Chestgear))
        {
            var srs = obj.GetComponentsInChildren<SpriteRenderer>();
            foreach (var sr in srs)
            {
                sr.material = item.material == null ? defaultMat : item.material;
            }
            if (equip.obj != null) Destroy(equip.obj);
            equip.obj = Instantiate(obj);
            if (equip.item.modifiedMat != null) equip.obj.GetComponent<SpriteRenderer>().material = equip.item.modifiedMat;
            equip.obj.transform.SetParent(equip.trans, false);
        }

        if (equip.item.GetType() == typeof(Lightsource))
        {
            Lightsource temp = equip.item as Lightsource;
            Debug.Log(equip.obj.transform.rotation);
            Weapon wep = CharacterStats.characterEquipment.weapon;
            if (wep != null)
            {
                if (wep.hands == Hands.One_handed) equip.obj.transform.SetParent(oneHandedLightSource, false);
                else equip.obj.transform.SetParent(twoHandedLightSource, false);
            }
            else equip.obj.transform.SetParent(oneHandedLightSource);
        
            CharacterStats.sightBonusFromItems += temp.lightRadius;
        }

        else if (equip.item.GetType() == typeof(Weapon))
        {
            Weapon temp = equip.item as Weapon;
            string defWep = temp.weaponType == WeaponType.Melee ? "Melee" : "Ranged";
            Equipped ls = equipment["Lightsource"];
            if (ls.obj != null)
            {
                if (temp.hands == Hands.One_handed) ls.obj.transform.SetParent(oneHandedLightSource, false);
                else ls.obj.transform.SetParent(twoHandedLightSource, false);
            }

            AnimationClip attackClip = temp.attackAnimation == null ? animations.DefaultAttackClip(temp.hands + defWep) : temp.attackAnimation;
            AnimationClip idleClip = animations.DefaultIdleClip(temp.hands + defWep);
            AnimationClip walkClip = animations.DefaultWalkClip(temp.hands + defWep);
            overrider["BaseAttack"] = attackClip;
            overrider["BaseIdle"] = idleClip;
            overrider["BaseWalk"] = walkClip;
            anim.runtimeAnimatorController = overrider;
            anim.SetLayerWeight(1, 1f);
            ParticleSystem trail = equip.obj.GetComponentInChildren<ParticleSystem>();
            if (trail != null)
            {
                References.rf.playerMovement.weaponTrailRenderer = trail;
                if (temp.weaponType == WeaponType.Melee || temp.weaponType == WeaponType.Flamethrower) trail.Stop();
            }
            if (temp.weaponType == WeaponType.Melee)
                References.rf.playerMovement.meleeWeapon = equip.obj.GetComponent<MeleeWeaponHit>();
        }

        else
        {
            Armor armor = equip.item as Armor;
            if (equip.item.GetType() == typeof(Chestgear))
            {
                foreach (Transform trans in obj.transform)
                {
                    chestgearEquipment[trans.name].sprite = trans.GetComponent<SpriteRenderer>().sprite;
                    chestgearEquipment[trans.name].material = item.material == null ? defaultMat : item.material;
                }
            }
            Info.AddDefenses(armor);
        }

        ApplyGearEffects(equip.item, false);
        CalculateStats();
        StartCoroutine("UpdateInventoryGear");

    }

    private void ApplyGearEffects(InventoryItems item, bool unequip)
    {
        GearEffect[] gearEffects = (GearEffect[])item.GetType().GetField("gearEffects").GetValue(item);
        if (gearEffects == null || gearEffects.Length == 0)
            return;
        foreach (var effect in gearEffects)
        {
            switch (effect.effect)
            {
                case _GearEffect.Increases_Strength:
                    CharacterStats.strength += !unequip ? (int)effect.amount : (int)-effect.amount;
                    break;
                case _GearEffect.Increases_Dexterity:
                    CharacterStats.dexterity += !unequip ? (int)effect.amount : (int)-effect.amount;
                    break;
                case _GearEffect.Increases_Perception:
                    CharacterStats.perception += !unequip ? (int)effect.amount : (int)-effect.amount;
                    break;
                case _GearEffect.Increases_Constitution:
                    CharacterStats.constitution += !unequip ? (int)effect.amount : (int)-effect.amount;
                    break;
                case _GearEffect.Increases_Luck:
                    CharacterStats.luck += !unequip ? (int)effect.amount : (int)-effect.amount;
                    break;
                case _GearEffect.Light_Radius:
                    CharacterStats.sightBonusPercentage += !unequip ? effect.amount : -effect.amount;
                    break;
                case _GearEffect.Increases_Health:
                    CharacterStats.healthBonusFromItems += !unequip ? effect.amount : -effect.amount;
                    break;
                case _GearEffect.Increases_Stamina:
                    CharacterStats.staminaBonusFromItems += !unequip ? effect.amount : -effect.amount;
                    break;
                case _GearEffect.Movement_Speed:
                    CharacterStats.moveSpeed = !unequip ? CharacterStats.moveSpeed *= 1f + effect.amount / 100f : CharacterStats.moveSpeed /= 1f + effect.amount / 100f;
                    break;
                case _GearEffect.Increases_Critical_Hit_Chance:
                    CharacterStats.criticalBonusPercentage += !unequip ? effect.amount : -effect.amount;
                    break;
                case _GearEffect.Increases_PhysicalDamagePoints:
                    Info.extraPhys += !unequip ? effect.amount : -effect.amount;
                    break;
                case _GearEffect.Increases_FireDamagePoints:
                    Info.extraFire += !unequip ? effect.amount : -effect.amount;
                    break;
                case _GearEffect.Increases_SpectralDamagePoints:
                    Info.extraSpec += !unequip ? effect.amount : -effect.amount;
                    break;
                case _GearEffect.Increases_PhysicalDamagePercent:
                    Info.extraPhysPercent += !unequip ? effect.amount : -effect.amount;
                    break;
                case _GearEffect.Increases_FireDamagePercent:
                    Info.extraFirePercent += !unequip ? effect.amount : -effect.amount;
                    break;
                case _GearEffect.Increases_SpectralDamagePercent:
                    Info.extraSpecPercent += !unequip ? effect.amount : -effect.amount;
                    break;

            }
        }
    }

    public void RemoveEquipment(InventoryItems item, bool willEquip = false)
    {
        var equip = equipment[item.GetType().ToString()];
        if (equip == null) return;
        if (item.GetType() == typeof(Lightsource))
        {
            Lightsource temp = equip.item as Lightsource;
            CharacterStats.sightBonusFromItems -= temp.lightRadius;
        }
        else if (item.GetType() == typeof(Weapon))
        {
            References.rf.playerMovement.meleeWeapon = null;
            AnimationClip idleClip = animations.DefaultIdleClip("One_handedMelee");
            AnimationClip walkClip = animations.DefaultWalkClip("One_handedMelee");
            overrider["BaseIdle"] = idleClip;
            overrider["BaseWalk"] = walkClip;
            anim.runtimeAnimatorController = overrider;
            anim.SetLayerWeight(1, 1f);
        }
        else
        {
            Armor armor = item as Armor;
            if (item.GetType() == typeof(Chestgear))
            {
                foreach (var obj in chestgearEquipment)
                {
                    obj.Value.sprite = null;
                }
            }
            Info.AddDefenses(armor, true);
        }
        if (equip.obj != null) Destroy(equip.obj);
        ApplyGearEffects(equip.item, true);
        if (!willEquip)
        {
            CalculateStats();
            StartCoroutine("UpdateInventoryGear");
        }
        equip.item = null;
    }
}

public class Equipped
{
    public Transform trans;
    public GameObject obj;
    public InventoryItems item;
}
