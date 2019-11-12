using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEquipment : MonoBehaviour
{
    public static Dictionary<string, Equipped> equipment = new Dictionary<string, Equipped>();
    private Dictionary<string, SpriteRenderer> chestgearEquipment = new Dictionary<string, SpriteRenderer>();
    private Dictionary<string, SpriteRenderer> leggearEquipment = new Dictionary<string, SpriteRenderer>();
    [HideInInspector]
    public Transform LOSCircle;
    [HideInInspector]
    public Animator anim;
    private float lightRadius = 1.75f;
    public AnimationClip oneHandedMelee, oneHandedRanged, oneHandedIdle, oneHandedWalk;
    public AnimationClip twoHandedMelee, twoHandedRanged, twoHandedIdle, twoHandedWalk;
    private AnimatorOverrideController overrider;

    private void Awake()
    {
        var gear = CharacterStats.characterEquipment.GetType().GetFields();
        foreach (var g in gear)
        {
            Transform trans = transform.parent.GetFromAllChildren(g.FieldType.ToString());
            equipment.Add(trans.name, new Equipped { trans = trans, item = null, obj = null });
        }
        anim = transform.parent.GetComponent<Animator>();
        overrider = new AnimatorOverrideController();
        overrider.runtimeAnimatorController = anim.runtimeAnimatorController;
    }

    private void Start()
    {
        Events.onAddPlayerEquipment += AddEquipment;
        LOSCircle = transform.parent.Find("MainFogCircle");
        References.rf.playerMovement.mask = transform.parent.Find("VisionMask");
        References.rf.playerMovement.mask.SetParent(null);
        LOSCircle.transform.localScale = Vector3.one * lightRadius;
        References.rf.playerMovement.mask.localScale = References.rf.playerMovement.transform.localScale * lightRadius;
        chestgearEquipment.Add("ChestgearEquip", transform.parent.GetFromAllChildren("ChestgearEquip").GetComponent<SpriteRenderer>());
        chestgearEquipment.Add("Mid_SectionEquip", transform.parent.GetFromAllChildren("Mid_SectionEquip").GetComponent<SpriteRenderer>());
        chestgearEquipment.Add("R_Upper_ArmEquip", transform.parent.GetFromAllChildren("R_Upper_ArmEquip").GetComponent<SpriteRenderer>());
        chestgearEquipment.Add("R_Lower_ArmEquip", transform.parent.GetFromAllChildren("R_Lower_ArmEquip").GetComponent<SpriteRenderer>());
        chestgearEquipment.Add("L_Upper_ArmEquip", transform.parent.GetFromAllChildren("L_Upper_ArmEquip").GetComponent<SpriteRenderer>());
        chestgearEquipment.Add("L_Lower_ArmEquip", transform.parent.GetFromAllChildren("L_Lower_ArmEquip").GetComponent<SpriteRenderer>());
    }

    private void OnDisable()
    {
        Events.onAddPlayerEquipment -= AddEquipment;
    }

    public void AddEquipment(GameObject obj, InventoryItems item)
    {
        var equip = equipment[item.GetType().ToString()];
        if (equip.item != null && equip.item.Equals(item)) return;
        equip.item = item;

        if (item.GetType() != typeof(Chestgear))
        {
            if (equip.obj != null) Destroy(equip.obj);
            equip.obj = Instantiate(obj);
            equip.obj.transform.SetParent(equip.trans, false);
        }

        if (equip.item.GetType() == typeof(Lightsource))
        {
            Lightsource temp = equip.item as Lightsource;
            LOSCircle.transform.localScale = Vector3.one * temp.lightRadius;
            References.rf.playerMovement.mask.localScale = Vector3.one * References.rf.playerMovement.transform.localScale.y * temp.lightRadius;
        }

        else if (equip.item.GetType() == typeof(Weapon))
        {
            Weapon temp = equip.item as Weapon;
            string defWep = temp.weaponType == WeaponType.Melee ? "Melee" : "Ranged";
            Equipped ls = equipment["Lightsource"];
            if (ls.obj != null)
            {
                Debug.Log("Lightsource");
                ls.obj.SetActive(temp.hands == Hands.One_handed);
            }

            AnimationClip attackClip = temp.attackAnimation == null ? DefaultAttackClip(temp.hands + defWep) : temp.attackAnimation;
            AnimationClip idleClip = DefaultIdleClip(temp.hands + defWep);
            AnimationClip walkClip = DefaultWalkClip(temp.hands + defWep);
            overrider["BaseAttack"] = attackClip;
            overrider["BaseIdle"] = idleClip;
            overrider["BaseWalk"] = walkClip;
            anim.runtimeAnimatorController = overrider;
            anim.SetLayerWeight(1, 1f);
            anim.SetFloat("AttackSpeed", temp.fireRate);
            ParticleSystem trail = equip.obj.GetComponentInChildren<ParticleSystem>();
            if (trail != null)
            {
                Debug.Log("Trail has been set");
                References.rf.playerMovement.weaponTrailRenderer = trail;
                trail.Stop();
            }
            if (temp.weaponType == WeaponType.Melee)
                References.rf.playerMovement.meleeWeapon = equip.obj.GetComponent<MeleeWeaponHit>();
        }

        else if (equip.item.GetType() == typeof(Chestgear))
        {
            foreach (Transform trans in obj.transform)
            {
                chestgearEquipment[trans.name].sprite = trans.GetComponent<SpriteRenderer>().sprite;
            }
        }
    }

    public void RemoveEquipment(Type item)
    {
        var equip = equipment[item.ToString()];
        if (equip.item != null && equip.item.Equals(item)) return;
        equip.item = null;
        if (equip.obj != null) Destroy(equip.obj);

        if (item == typeof(Lightsource))
        {
            LOSCircle.transform.localScale = Vector3.one * lightRadius;
        }
        else if (item == typeof(Weapon))
        {
            //References.rf.playerMovement.meleeWeapon = null;
        }
        else if (item == typeof(Chestgear))
        {
            foreach (var obj in chestgearEquipment)
            {
                obj.Value.sprite = null;
            }
        }
    }

    private AnimationClip DefaultAttackClip(string weapon)
    {
        switch (weapon)
        {
            case "One_handedMelee":
                return oneHandedMelee;
            case "Two_handedMelee":
                return twoHandedMelee;
            case "One_handedRanged":
                return oneHandedRanged;
            case "Two_handedRanged":
                return twoHandedRanged;
        }
        return null;
    }
    private AnimationClip DefaultIdleClip(string weapon)
    {
        switch (weapon)
        {
            case "One_handedMelee":
                return oneHandedIdle;
            case "Two_handedMelee":
                return twoHandedIdle;
            case "One_handedRanged":
                return oneHandedIdle;
            case "Two_handedRanged":
                return twoHandedIdle;
        }
        return null;
    }
    private AnimationClip DefaultWalkClip(string weapon)
    {
        switch (weapon)
        {
            case "One_handedMelee":
                return oneHandedWalk;
            case "Two_handedMelee":
                return twoHandedWalk;
            case "One_handedRanged":
                return oneHandedWalk;
            case "Two_handedRanged":
                return twoHandedWalk;
        }
        return null;
    }
}

public class Equipped
{
    public Transform trans;
    public GameObject obj;
    public InventoryItems item;
}
