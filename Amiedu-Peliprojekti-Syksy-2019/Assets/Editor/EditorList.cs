using UnityEngine;
using UnityEditor;

public static class EditorList
{ 
    public static void Show (SerializedProperty list, SerializedObject _object)
    {
  
        var boxStyle = new GUIStyle(EditorStyles.helpBox);
        boxStyle.border = new RectOffset(2, 2, 2, 2);
        boxStyle.margin = new RectOffset(0, 13, 0, 10);
     

        EditorGUILayout.Space();
        
        for (int i = 0; i < list.arraySize; i++)
        {

            if (i % 2 == 0)
                GUI.backgroundColor = new Color(0.4f, 0.4f, 0.4f, 1f);
            else
                GUI.backgroundColor = new Color(0.2f, 0.2f, 0.2f, 1f);
            EditorGUILayout.BeginVertical(boxStyle);

            EditorGUILayout.Space();


            SerializedProperty tag = list.GetArrayElementAtIndex(i).FindPropertyRelative("tag");
            SerializedProperty music = list.GetArrayElementAtIndex(i).FindPropertyRelative("music");
            SerializedProperty clip = list.GetArrayElementAtIndex(i).FindPropertyRelative("clip");

            
            GUI.backgroundColor = new Color(1f, 1f, 1f, 0f);
            GUI.skin.textField.fontSize = 18;
            EditorGUILayout.PropertyField(tag, GUIContent.none, GUILayout.Height(25f));
            GUI.skin.textField.fontSize = 12;
            GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f, 0.8f);
            EditorGUILayout.ObjectField(clip, GUIContent.none);
            EditorGUILayout.PropertyField(music, new GUIContent("Music"));
            
            EditorGUILayout.Space();
            if (GUILayout.Button("Delete this Audio Clip"))
            {
                int x = i;
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Confirm"), false, () => DeleteEntry(list, x));
                menu.ShowAsContext();
            }
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
            
     

        }

       void DeleteEntry(SerializedProperty prop, int i)
        {
            prop.DeleteArrayElementAtIndex(i);
            _object.ApplyModifiedProperties();
        }

       
    }
}
