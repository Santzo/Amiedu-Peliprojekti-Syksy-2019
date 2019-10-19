using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEquipment : MonoBehaviour
{
    public static Equipped[] equipment;
    public SpriteMask mask;

    private void Awake()
    {
        equipment = new Equipped[transform.childCount];
        equipment.Populate();
        for (int i = 0; i < transform.childCount; i++)
        {
            equipment[i].trans = transform.GetChild(i);
            equipment[i].item = null;
        }
        mask = transform.Find("LeftHand").Find("LOS").GetComponent<SpriteMask>();

    }

    public static void AddEquipment(GameObject obj, InventoryItems item)
    {
        int i = 0;

        if (item.GetType() == typeof(Headgear))
            i = 0;
        if (item.GetType() == typeof(Lightsource))
        {
            i = 1;
            Lightsource ls = item as Lightsource;
            Info.playerEquipment.mask.transform.localScale = new Vector3(ls.lightRadius, ls.lightRadius, ls.lightRadius);
        }
        else if (item.GetType() == typeof(Weapon))
            i = 2;

        if (equipment[i].item != null &&  equipment[i].item.Equals(item)) return;
        equipment[i].item = item;
        if (equipment[i].obj != null) Destroy(equipment[i].obj);
        equipment[i].obj = Instantiate(obj);
        equipment[i].obj.transform.SetParent(equipment[i].trans, false);

    }

}
public class Equipped
{
    public Transform trans;
    public GameObject obj;
    public InventoryItems item;
}
