using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Events
{
    public static Action<List<InventoryItems>> updateFilteredItems = delegate { };
}
