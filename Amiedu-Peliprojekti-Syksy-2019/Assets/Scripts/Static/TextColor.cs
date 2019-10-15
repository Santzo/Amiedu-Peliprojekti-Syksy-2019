using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextColor
{
    public static string Return(string color = "white")
    {
        switch (color) {
            case ("green"):
                return "<color=#00dd22>";
            case ("yellow"):
                return "<color=#FFDC00>";

            default:
                return "<color=white>";
        }
    }
}
