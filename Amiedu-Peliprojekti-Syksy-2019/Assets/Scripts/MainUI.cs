using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainUI : MonoBehaviour
{
    GameObject inventory;
    private void Awake()
    {
        inventory = transform.Find("Inventory").gameObject;
        //inventory.SetActive(false);
    }
    private void OnEnable()
    {
        Events.inventoryKey += () => inventory.SetActive(!inventory.activeSelf);
    }
    private void OnDisable()
    {
        Events.inventoryKey -= () => inventory.SetActive(!inventory.activeSelf);
    }
}
