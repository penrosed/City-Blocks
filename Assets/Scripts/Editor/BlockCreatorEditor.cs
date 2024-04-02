using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(BlockCreator))]
class BlockCreatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUILayout.Space();

        DrawDefaultInspector();

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();

        var scriptRef = (BlockCreator)target;
        if (GUILayout.Button("Create Block"))
        {
            scriptRef.CreateBlock();
        }
        if (GUILayout.Button("Apply Material Overrides"))
        {
            scriptRef.ApplyMaterialOverrides();
        }

        EditorGUILayout.EndHorizontal();
    }
}