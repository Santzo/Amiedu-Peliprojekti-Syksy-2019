using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class KeyboardConfig 
{
    // Delay time to check for a double click
    public static float doubleClickInterval = 0.25f; 

    // Inventory Key
    public static KeyCode[] inventory =     new KeyCode[] { (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("inventoryKey", "I")),
                                                        (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("inventoryKeyAlt", "O")) };
    public static KeyCode[] sprint =    new KeyCode[] { (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("sprintKey", "LeftShift")),
                                                        (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("sprintKeyAlt", "RightShift")) };

    

    // Up, Down, Left & Right keys
    public static KeyCode[] up =            new KeyCode[] { (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("upKey", "W")),
                                                        (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("upKeyAlt", "UpArrow")) };

    public static KeyCode[] down =          new KeyCode[] { (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("downKey", "S")),
                                                        (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("downKeyAlt", "DownArrow")) };

    public static KeyCode[] left =          new KeyCode[] { (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("leftKey", "A")),
                                                        (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("leftKeyAlt", "LeftArrow")) };

    public static KeyCode[] right =         new KeyCode[] { (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("rightKey", "D")),
                                                        (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("rightKeyAlt", "RightArrow")) };

    public static KeyCode[] hotbar =        new KeyCode[] { (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("hotbar1", "1")),
                                                            (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("hotbar2", "2")),
                                                            (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("hotbar3", "3")),
                                                            (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("hotbar4", "4")) };



}
