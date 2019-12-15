using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RustValues : SetMaterialProperties
{
    [Range(0, 1)]
    public float _Strength;
    [Range(0,1)]
    public float _StartX;
    [Range(0, 1)]
    public float _EndX;
    [Range(0, 1)]
    public float _StartY;
    [Range(0, 1)]
    public float _EndY;
}
