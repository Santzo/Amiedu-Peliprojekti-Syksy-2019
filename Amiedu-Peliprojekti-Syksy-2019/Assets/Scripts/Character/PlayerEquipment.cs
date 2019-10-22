using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEquipment : MonoBehaviour
{
    public static Dictionary<string, Equipped> equipment = new Dictionary<string, Equipped>();
    public static Transform LOSCircle;


    private void Awake()
    {
  

        var gear = CharacterStats.characterEquipment.GetType().GetFields();
        foreach (var g in gear)
        {
            Transform trans = transform.parent.GetFromAllChildren(g.FieldType.ToString());
            equipment.Add(trans.name, new Equipped { trans = trans, item = null, obj = null });

        }



    }
    private void Start()
    {
        LOSCircle = transform.parent.Find("MainFogCircle");
    }

    public static void AddEquipment(GameObject obj, InventoryItems item)
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

    }

}
public class Equipped
{
    public Transform trans;
    public GameObject obj;
    public InventoryItems item;
}
