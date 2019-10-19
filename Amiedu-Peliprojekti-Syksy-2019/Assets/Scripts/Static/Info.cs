using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Info
{
    private static Canvas canvas = GameObject.Find("UI").GetComponent<Canvas>();
    public static Camera camera = Camera.main;
    public static float CanvasScale { get { return canvas.scaleFactor; } }
    public static Transform content; 

}
