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
            case ("defaultTitle"):
                return "<color=#DCFF94>";
            case ("red"):
                return "<color=#EE2211>";
            case ("purple"):
                return "<color=#FF44FF>";

            default:
                return "<color=white>";
        }
    }
}
