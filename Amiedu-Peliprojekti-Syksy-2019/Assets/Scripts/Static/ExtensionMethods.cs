using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
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

    public static void EmptyTextArray(this TextMeshProUGUI[] ori)
    {
        for (int i = 0; i < ori.Length; i++)
        {
            ori[i].text = "";
        }
    }
    public static string ReplaceUnderScore(string replace)
    {
        replace.Replace("_", " ");
        return replace;

    }

    public static T[] LoadAssets<T>(this T[] arr, string path) where T : Object
    {
        var assetBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, path));
        var assets = assetBundle.LoadAllAssets<T>();
        return assets;
    }

    public static void UItemInitialize(this List<UIItem> uitem, Transform transform)
    {
        foreach (Transform trans in transform)
        {
            if (trans.GetComponent<UIEvents>() != null)
            {
             
                uitem.Add(new UIItem { trans = trans, anim = trans.GetComponent<Animator>(), text = trans.GetComponentInChildren<TextMeshProUGUI>() });
                UIEvents uievent = trans.GetComponent<UIEvents>();
                uievent.mouseController = transform.GetComponent<IUIHandler>();
                uievent.index = uitem.Count - 1;
            }

        }
    }

    public static Transform GetFromAllChildren(this Transform ori, string tag)
    {
        Transform result = null;
        foreach (Transform child in ori)
        {
            if (child.name == tag)
                return child;

            result = child.GetFromAllChildren(tag);

            if (result != null)
                return result;
        }

        return result;

    }
}
