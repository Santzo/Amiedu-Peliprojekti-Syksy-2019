using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class KeyboardConfig 
{
    // Delay time to check for a double click
    public static float doubleClickInterval = 0.25f;

    // Up, Down, Left & Right keys
    public static KeyCode[] up = new KeyCode[] { (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("upKey", "W")),
                                                        (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("upKeyAlt", "UpArrow")) };

    public static KeyCode[] down = new KeyCode[] { (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("downKey", "S")),
                                                        (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("downKeyAlt", "DownArrow")) };

    public static KeyCode[] left = new KeyCode[] { (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("leftKey", "A")),
                                                        (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("leftKeyAlt", "LeftArrow")) };

    public static KeyCode[] right = new KeyCode[] { (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("rightKey", "D")),
                                                        (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("rightKeyAlt", "RightArrow")) };

    // Inventory Key
    public static KeyCode[] inventory =     new KeyCode[] { (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("inventoryKey", "I")),
                                                        (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("inventoryKeyAlt", "O")) };
    public static KeyCode[] sprint =        new KeyCode[] { (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("sprintKey", "LeftShift")),
                                                        (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("sprintKeyAlt", "RightShift")) };
    public static KeyCode[] action =        new KeyCode[] { (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("actionKey", "E")),
                                                        (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("actionKeyAlt", "F")) };
    public static KeyCode[] attack =        new KeyCode[] { (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("attackKey", "Mouse0")),
                                                        (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("attackKeyAlt", "Space")) };



   

    public static KeyCode[] hotbar =        new KeyCode[] { (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("hotbar1", "1")),
                                                            (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("hotbar2", "2")),
                                                            (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("hotbar3", "3")),
                                                            (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("hotbar4", "4")) };

    public static string ReturnKeyName(string key)
    {
        if (key == "Mouse0") return "mouse left";
        if (key == "Mouse1") return "mouse middle";
        if (key == "Mouse2") return "mouse right";
        string result = "";
        for (int i = 0; i < key.Length; i++)
        {
            bool isUpper = Char.IsUpper(key[i]);
            result += isUpper && i > 0 ? " " + key[i].ToString() : key[i].ToString();
        }
        return result;
    }

}
