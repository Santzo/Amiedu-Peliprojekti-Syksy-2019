using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEquipment : MonoBehaviour
{
    public static Dictionary<string, Equipped> equipment = new Dictionary<string, Equipped>();
    public static Transform LOSCircle;
    public Animator anim;
    private float lightRadius = 1.75f;


    private void Awake()
    {
        var gear = CharacterStats.characterEquipment.GetType().GetFields();
        foreach (var g in gear)
        {
            Transform trans = transform.parent.GetFromAllChildren(g.FieldType.ToString());
            equipment.Add(trans.name, new Equipped { trans = trans, item = null, obj = null });

        }
        anim = transform.parent.GetComponent<Animator>();
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
            if (temp.hands == Hands.Two_handed) anim.SetLayerWeight(1, 1f);
            else anim.SetLayerWeight(1, 0f);
            
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
            anim.SetLayerWeight(1, 0f);
        }
    }

}
public class Equipped
{
    public Transform trans;
    public GameObject obj;
    public InventoryItems item;
}
