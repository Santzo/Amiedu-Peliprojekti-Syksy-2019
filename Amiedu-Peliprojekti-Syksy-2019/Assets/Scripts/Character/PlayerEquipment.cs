using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEquipment : MonoBehaviour
{
    public static Equipped[] equipment;

    private void Awake()
    {
        equipment = new Equipped[transform.childCount];
        equipment.Populate();
        for (int i = 0; i < transform.childCount; i++)
        {
            equipment[i].obj = transform.GetChild(i);
            equipment[i].item = null;
        }
    }

    public static void AddEquipment(GameObject obj, InventoryItems item)
    {
        int i = 0;

        if (item.GetType() == typeof(Headgear))
            i = 0;
        if (item.GetType() == typeof(Lightsource))
            i = 1;
        else if (item.GetType() == typeof(Weapon))
            i = 2;
       

        equipment[i].item = item;
        GameObject instance = Instantiate(obj);
        instance.transform.SetParent(equipment[i].obj, false);
    }

}
public class Equipped
{
    public Transform obj;
    public InventoryItems item;
}
