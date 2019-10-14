using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Events
{
    public static Action<List<InventoryItems>> updateFilteredItems = delegate { };
    public static Action<string> onUIClick = delegate { };
    public static Action<int, Vector2> onItemHover = delegate { };
    public static Action onItemLeaveHover = delegate { };
}
