using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public abstract class SetMaterialProperties : MonoBehaviour
{
    private Renderer rend;
    private MaterialPropertyBlock propertyBlock;
    private FieldInfo[] values;

    public void OnValidate()
    {
#if UNITY_EDITOR
        SetProps();
#endif
    }
    public void SetProps()
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
        foreach (var val in values)
        {
            rend.SetValue(propertyBlock, val.Name, val.GetValue(this));
        }
    }
}
