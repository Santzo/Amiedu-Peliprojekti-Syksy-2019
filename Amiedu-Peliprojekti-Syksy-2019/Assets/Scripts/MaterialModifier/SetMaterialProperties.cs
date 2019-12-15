using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public abstract class SetMaterialProperties : MonoBehaviour
{
    private Renderer rend;
    private MaterialPropertyBlock propertyBlock;
    private FieldInfo[] values;
    private void Awake()
    {
        SetProps();
        //Destroy(this);
    }
    private void OnValidate()
    {
        SetProps();
    }
    private void SetProps()
    {
        if (rend == null || propertyBlock == null)
        {
            rend = GetComponent<Renderer>();
            propertyBlock = new MaterialPropertyBlock();
            rend.GetPropertyBlock(propertyBlock);
        }
        if (values == null)
        {
            values = GetType().GetFields();
        }
        int i = 0;
        foreach (var val in values)
        {
            rend.SetValue(propertyBlock, val.Name, val.GetValue(this));
        }
    }
}
