using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainUI : MonoBehaviour
{
    GameObject inventory;
    GameObject overlay;
    GameObject underlay;

    private void Awake()
    {
        overlay = transform.Find("UIOverlay").gameObject;
        underlay = transform.Find("UIUnderlay").gameObject;
        inventory = GetComponentInChildren<MainInventory>().gameObject;
    }

    private void Start()
    {
        inventory.SetActive(false);
        overlay.SetActive(true);
        underlay.SetActive(false);
    }

    private void OnEnable()
    {
        Events.inventoryKey += ActivateInventory;
    }

    private void OnDisable()
    {
        Events.inventoryKey -= ActivateInventory;
    }

    private void ActivateInventory()
    {
        References.rf.statsDetails.UpdateStats();
        underlay.SetActive(!underlay.activeSelf);
        inventory.SetActive(!inventory.activeSelf);
    }
}
