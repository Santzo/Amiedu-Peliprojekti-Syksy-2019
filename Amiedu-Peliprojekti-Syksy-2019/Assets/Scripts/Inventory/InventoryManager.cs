using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager im;
    public static Weapon[] weapons;

    void Awake()
    {
        im = this;
        weapons = Resources.LoadAll<Weapon>("Inventory/Weapons");
    }
    // Start is called before the first frame update
    void Start()
    {
        foreach (var weapon in weapons)
        {
            CharacterStats.inventoryItems.Add(weapon);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
