using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEquipment : MonoBehaviour
{
    public static Equipped[] equipment;
    public static Transform LOSCircle;


    private void Awake()
    {
        equipment = new Equipped[transform.childCount];
        equipment.Populate();
        for (int i = 0; i < transform.childCount; i++)
        {
            equipment[i].trans = transform.GetChild(i);
            equipment[i].item = null;
        }
        
    }
    private void Start()
    {
        LOSCircle = transform.parent.Find("MainFogCircle");
    }

    public static void AddEquipment(GameObject obj, InventoryItems item)
    {
        var equip = Array.Find(equipment, a => item.GetType().ToString() == a.trans.name);

        if (equip.item != null && equip.item.Equals(item)) return;
        equip.item = item;
        if (equip.obj != null) Destroy(equip.obj);
        equip.obj = Instantiate(obj);
        equip.obj.transform.SetParent(equip.trans, false);

        if (equip.item.GetType() == typeof(Lightsource))
        {
            Debug.Log("JEEJEE");
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
