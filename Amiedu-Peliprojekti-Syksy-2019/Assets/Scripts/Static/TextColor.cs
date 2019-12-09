using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextColor
{
    public static string Yellow { get {return "<color=#FFDC00>"; } }
    public static string Green { get { return "<color=#00dd22>"; } }
    public static string Title {  get { return "<color=#DCFF94>";  } }
    public static string Red { get { return "<color=#EE2211>"; } }
    public static string Purple { get { return "<color=#FF44FF>"; } }
    public static string White { get { return "<color=white>"; } }
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
