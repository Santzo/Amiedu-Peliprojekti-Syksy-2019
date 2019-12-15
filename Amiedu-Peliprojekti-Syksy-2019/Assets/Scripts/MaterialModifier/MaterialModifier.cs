using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public static class MaterialModifier
{
    private static ShaderValues[] rustValues, outlineValues;
    public static void SetValue(this Renderer rend, MaterialPropertyBlock propertyBlock, string prop, object _value)
    {
        if (_value is float)
        {
            propertyBlock.SetFloat(prop, (float)_value);
        }
        else if (_value is Color)
        {
            Debug.Log("Color set");
            propertyBlock.SetColor(prop, (Color)_value);
        }
        rend.SetPropertyBlock(propertyBlock);
    }
    public static void SetUIPropertyBlock(this Material mat, Renderer change)
    {
        MaterialPropertyBlock block = new MaterialPropertyBlock();
        change.GetPropertyBlock(block);
        Debug.Log(mat.name);
        switch (mat.name)
        {
            case "Rust":
                SetValues(mat, block, rustValues);
                break;
            case "Outline":
                SetValues(mat, block, outlineValues);
                Debug.Log("Outline values set");
                break;
            default:
                return;
        }
    }
    public static void InitializeValues()
    {
        rustValues = InitializeMaterialProps(typeof(RustValues));
        outlineValues = InitializeMaterialProps(typeof(OutlineValues));
    }

    private static ShaderValues[] InitializeMaterialProps(Type type)
    {
        var fields = type.GetFields();
        ShaderValues[] values = new ShaderValues[fields.Length];
        for (int i = 0; i < fields.Length; i++)
        {
            values[i].value = Shader.PropertyToID(fields[i].Name);
            var fieldType = fields[i].FieldType.ToString();
            values[i].type = fields[i].FieldType == typeof(float) ? "Float" : "Color";
        }
        return values;
    }
    private static void SetValues(Material mat, MaterialPropertyBlock block, ShaderValues[] values)
    {
        foreach (var val in values)
        {
            if (val.type == "Float")
            {
                var _float = block.GetFloat(val.value);
                Debug.Log(val.value +  " " + _float);
                mat.SetFloat(val.value, _float);
            }
            else if (val.type == "Color")
            {
                var _color = block.GetColor(val.value);
                mat.SetColor(val.value, _color);
            }
        }
    }
    private struct ShaderValues
    {
        public int value;
        public string type;
    }
}
