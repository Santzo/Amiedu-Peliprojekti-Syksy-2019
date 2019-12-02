using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEngine.Experimental;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Reflection;

[CustomEditor(typeof(BaseEnemy))]
public class EnemyDamageDrawer : Editor
{
    SerializedProperty list;
    EnemyDamage[] enemyDamage;
    FieldInfo[] damageFields;
    VisualElement container;
    int listSize;

    public override VisualElement CreateInspectorGUI()
    {
        list = serializedObject.FindProperty("attacks");
        container = new VisualElement();
        enemyDamage = typeof(BaseEnemy).GetField("attacks").GetValue(target) as EnemyDamage[];
        var fields = typeof(BaseEnemy).GetFields();
        damageFields = typeof(EnemyDamage).GetFields();
        PropertyField[] props = new PropertyField[fields.Length];
        container.style.alignItems = new StyleEnum<Align>() { value = Align.Center };
        container.style.flexDirection = new StyleEnum<FlexDirection>() { value = FlexDirection.Row };
        //for (int i = 0; i < props.Length; i++)
        //{
        //    var prop = serializedObject.FindProperty(fields[i].Name);
        //    var type = fields[i].FieldType;
        //    if (type == typeof(string) || prop.isArray && prop.arraySize > 0)
        //    {
        //        props[i] = new PropertyField(prop);
        //        container.Add(props[i]);
        //    }
        //}
        ApplyNewList();
       
        return container;
    }

    private void RemoveFromEnemyList()
    {
        if (list.arraySize > 0) list.arraySize -= 1;
        ApplyNewList();
        list.serializedObject.ApplyModifiedProperties();
    }

    void AddToEnemyList()
    {
        list.arraySize += 1;
        //enemyDamage = new EnemyDamage[list.arraySize];
        //enemyDamage[list.arraySize - 1] = new EnemyDamage();
        ApplyNewList();
        list.serializedObject.ApplyModifiedProperties();
       
    }
    void ApplyNewList()
    {
        container.Clear();
        for (int i = 0; i < list.arraySize; i++)
        {
            foreach (var dmg in damageFields)
            {
                var prop = list.GetArrayElementAtIndex(i).FindPropertyRelative(dmg.Name);
                container.Add(new PropertyField(prop));
            }
        }
        var addButton = new Button() { text = "Add Damage" };
        var removeButton = new Button() { text = "Remove Damage" };
        addButton.clickable.clicked += AddToEnemyList;
        removeButton.clickable.clicked += RemoveFromEnemyList;
        container.Add(addButton);
        container.Add(removeButton);
    }
}