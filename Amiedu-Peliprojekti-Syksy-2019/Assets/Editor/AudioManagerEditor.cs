using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Audio))]
public class TestObjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorList.Show(serializedObject.FindProperty("clip"), serializedObject);
        serializedObject.ApplyModifiedProperties();

        Audio audioObject = (Audio)target;

        var guiStyle = new GUIStyle(GUI.skin.button);
        guiStyle.fontSize = 12;
        guiStyle.fontStyle = FontStyle.Bold;
        guiStyle.border = new RectOffset(15, 15, 15, 15);
        GUI.backgroundColor = new Color(0.7f, 0.7f, 0.9f, 1f);


        if (GUILayout.Button("Add a new Audio Clip", guiStyle, GUILayout.Height(40f)))
            audioObject.clip.Add(new Audio.Clip());

    }
}

