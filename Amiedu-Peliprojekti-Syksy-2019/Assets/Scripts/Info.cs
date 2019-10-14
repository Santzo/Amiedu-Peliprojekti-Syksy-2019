using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Info : MonoBehaviour
{
    public float CanvasScale { get { return 5; } }
    public static float Canvas()
    {
        return GameObject.Find("UI").GetComponent<Canvas>().scaleFactor;
    }
}
