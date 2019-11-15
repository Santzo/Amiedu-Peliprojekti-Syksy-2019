﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Events
{
    public static Action<List<Inventory>> updateFilteredItems = delegate { };
    public static Action<string> onUIClick = delegate { };
    public static Action<int, Inventory> onIconDoubleClick = delegate { };
    public static Action<int, Vector2> onItemHover = delegate { };
    public static Action onItemLeaveHover = delegate { };
    public static Action<int, Vector2> onItemDragStop = delegate { };
    public static Action onItemDragStart = delegate { };
    public static Action inventoryKey = delegate { };
    public static Action<int> onEquipmentIconHover = delegate { };
    public static Action<int> onEquipmentIconHoverLeave = delegate { };
    public static Action<int> onEquipmentIconPress = delegate { };
    public static Action onInventoryChange = delegate { };
    public static Action<Type, int> onUnEquip = delegate { };
    public static Action<Inventory> onEquipConsumable = delegate { };
    public static Action<LevelManager.AllRooms> onFieldInitialized = delegate { };
    public static Action<GameObject, InventoryItems> onAddPlayerEquipment = delegate { };
    public static Action onGameFieldCreated = delegate { };
    public static Action<List<Collider2D>> onEnemyHitboxesUpdated = delegate { };

    public static bool onDrag = false;
    public static bool onDialogueBox = false;
   
}
