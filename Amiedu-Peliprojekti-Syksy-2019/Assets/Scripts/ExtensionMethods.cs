﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
    public static void Populate<T>(this T[] arr) where T : new()
    {
        for (int i = 0; i < arr.Length; i++)
        {
            arr[i] = new T();
        }
    }

    public static SpriteRenderer AddRenderer(this GameObject ori, Material mat)
    {
        var sr = ori.AddComponent<SpriteRenderer>();
        sr.material = mat;
        return sr;
    }
}
