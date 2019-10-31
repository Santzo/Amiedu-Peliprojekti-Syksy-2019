using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEquipment : MonoBehaviour
{
    public static Dictionary<string, Equipped> equipment = new Dictionary<string, Equipped>();
    private Transform LOSCircle;
    private int animationLayerIndex = 0;
    private int attackLayerIndex = 0;
    [HideInInspector]
    public Animator anim;
    private int weaponLayerIndex = 0;
    private float lightRadius = 1.75f;
    public AnimationClip oneHandedMelee;
    public AnimationClip twoHandedMelee;
    public AnimationClip oneHandedRanged;
    public AnimationClip twoHandedRanged;


    private void Awake()
    {
        var gear = CharacterStats.characterEquipment.GetType().GetFields();
        foreach (var g in gear)
        {
            Transform trans = transform.parent.GetFromAllChildren(g.FieldType.ToString());
            equipment.Add(trans.name, new Equipped { trans = trans, item = null, obj = null });

        }
        anim = transform.parent.GetComponent<Animator>();
        for (int i = 0; i < anim.runtimeAnimatorController.animationClips.Length; i++)
        {
            if (anim.runtimeAnimatorController.animationClips[i].name == "BaseAttack")
            {
                weaponLayerIndex = i;
                break;
            }
        }
    }
    private void Start()
    {
        Events.onAddPlayerEquipment += AddEquipment;
        LOSCircle = transform.parent.Find("MainFogCircle");
        LOSCircle.transform.localScale = Vector3.one * lightRadius;
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
        if (equip.obj != null) Destroy(equip.obj);
        equip.obj = Instantiate(obj);
        equip.obj.transform.SetParent(equip.trans, false);

        if (equip.item.GetType() == typeof(Lightsource))
        {
            Lightsource temp = equip.item as Lightsource;
            LOSCircle.transform.localScale = Vector3.one * temp.lightRadius;
        }
        else if (equip.item.GetType() == typeof(Weapon))
        {
            Weapon temp = equip.item as Weapon;
            AnimationClip attackClip = temp.attackAnimation == null ? DefaultClip(temp.hands + temp.weaponType.ToString()) : temp.attackAnimation;
            AnimatorOverrideController _override = new AnimatorOverrideController();
            _override.runtimeAnimatorController = anim.runtimeAnimatorController;
            Debug.Log(_override.animationClips[weaponLayerIndex]);
            _override[_override.animationClips[weaponLayerIndex]] = attackClip;
            Debug.Log(_override.animationClips[weaponLayerIndex]);
            anim.runtimeAnimatorController = _override;     
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
        }
    }

    private AnimationClip DefaultClip(string weapon)
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

}
public class Equipped
{
    public Transform trans;
    public GameObject obj;
    public InventoryItems item;
}
