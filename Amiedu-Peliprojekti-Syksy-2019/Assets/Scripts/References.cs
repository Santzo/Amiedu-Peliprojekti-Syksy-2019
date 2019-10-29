using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class References : MonoBehaviour
{
    [HideInInspector]
    public Bar healthBar;
    [HideInInspector]
    public Bar staminaBar;
    [HideInInspector]
    public PlayerEquipment playerEquipment;
    [HideInInspector]
    public StatsDetails statsDetails;
    [HideInInspector]
    public MainInventory mainInventory;

    public static References rf;
    private void Awake()
    {
        if (rf == null)
        {
            rf = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

       
    }

    void Start()
    {
        healthBar = GameObject.Find("HealthBar").GetComponent<Bar>();
        staminaBar = GameObject.Find("StaminaBar").GetComponent<Bar>();
        playerEquipment = FindObjectOfType<PlayerEquipment>();
        statsDetails = FindObjectOfType<StatsDetails>();
        mainInventory = FindObjectOfType<MainInventory>();
        mainInventory.gameObject.SetActive(false);
    }

}
