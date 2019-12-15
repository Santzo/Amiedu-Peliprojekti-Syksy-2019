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
    [HideInInspector]
    public Transform uiUnderLay;
    [HideInInspector]
    public PlayerMovement playerMovement;
    [HideInInspector]
    public WeaponSlot weaponSlot;
    [HideInInspector]
    public InventoryScreenCharacter inventoryScreenCharacter;
    [HideInInspector]
    public DialogueBox currentDialogueBox;
    [HideInInspector]
    public Camera mainCamera;
    [HideInInspector]
    public InteractableObject currentInteractableObject;
    public LevelGenerator levelGenerator;
    public Transform uiOverlay;
    public EnemyManager enemyManager;


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
        healthBar = GameObject.Find("HealthBar").GetComponent<Bar>();
        staminaBar = GameObject.Find("StaminaBar").GetComponent<Bar>();
        playerEquipment = FindObjectOfType<PlayerEquipment>();
        playerMovement = FindObjectOfType<PlayerMovement>();
        statsDetails = FindObjectOfType<StatsDetails>();
        mainInventory = FindObjectOfType<MainInventory>();
        weaponSlot = FindObjectOfType<WeaponSlot>();
        enemyManager = FindObjectOfType<EnemyManager>();
        inventoryScreenCharacter = FindObjectOfType<InventoryScreenCharacter>();
        mainInventory.gameObject.SetActive(false);
        mainCamera = Camera.main;
    }
 

}
