using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class KeyboardConfig 
{
    public static KeyCode inventory = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("inventoryKey", "I"));

}
