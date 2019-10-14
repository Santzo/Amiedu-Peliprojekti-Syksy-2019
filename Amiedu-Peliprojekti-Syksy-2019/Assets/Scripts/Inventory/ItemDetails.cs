using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class ItemDetails : MonoBehaviour
{
    private TextMeshProUGUI itemName;
    private TextMeshProUGUI itemType;
    private TextMeshProUGUI itemDescription;
    private Transform info;
    private TextMeshProUGUI[] details;
    private Animator anim;

    public void Awake()
    {
        itemName = transform.Find("ItemName").GetComponent<TextMeshProUGUI>();
        itemType = transform.Find("ItemType").GetComponent<TextMeshProUGUI>();
        itemDescription = transform.Find("ItemDescription").GetComponent<TextMeshProUGUI>();
        anim = GetComponent<Animator>();
        info = transform.Find("Info");

        details = new TextMeshProUGUI[info.childCount];
        for (int i = 0; i < info.childCount; i++)
        {
            details[i] = info.GetChild(i).GetComponent<TextMeshProUGUI>();
            details[i].text = "";
        }
    }



    public void DisplayDetails(InventoryItems item)
    {
        itemName.text = item.name;
        itemDescription.text = item.description;

        if (item is Weapon)  // WEAPONS HERE
        {
            Weapon _item = item as Weapon;
            itemType.text = _item.hands.ToString().Replace("_", " ") + " " + _item.weaponType + " - Level " + item.itemLevel;
            details[0].text = "Damage <color=yellow>" + _item.minDamage + " - " + _item.maxDamage;
            details[2].text = "Critical hit chance <color=yellow>" + _item.criticalHitChance + "<color=white>%";
            if (_item.weaponType == WeaponType.Melee)
            {
                details[1].text = "Weapon swings <color=yellow>" + _item.fireRate + "<color=white> times per second";

            }
            else
            {
                details[1].text = "Weapon fires <color=yellow>" + _item.fireRate + "<color=white> times per second";
                details[3].text = "Clip size <color=yellow>" + _item.clipSize;
                details[4].text = "Reload time <color=yellow>" + _item.reloadTime + "<color=white> seconds";
                details[5].text = "Weapon shoots <color=yellow>" + _item.bulletPerShot + "<color=white> bullets per shot";
            }
        }


        else if (item is Headgear) // HEADGEAR HERE
        {
            itemType.text = item.itemType.ToString() + " - Level " + item.itemLevel; ;
        }

        else if (item is Consumable) // CONSUMABLES HERE
        {
            Consumable _item = item as Consumable;
            for (int i = 0; i < _item.itemEffect.Length; i++)
            {
                details[i].text = _item.itemEffect[i].effect.ToString().Replace("_", " ") + "<color=yellow> " + _item.itemEffect[i].amount + "<color=white> points in <color=yellow>" + _item.itemEffect[i].duration + "<color=white> seconds";
            }
        }
    }

    private void OnEnable()
    {
        details.EmptyTextArray();
    }


}
