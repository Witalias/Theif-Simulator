using UnityEditor;

[CustomEditor(typeof(Cheats))]
public class CheatsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("F1", "New level");
        EditorGUILayout.LabelField("F2", "New task");
        EditorGUILayout.LabelField("F3", "Add 100 money");
        EditorGUILayout.LabelField("F4", "Set backpack capacity to 999");
        EditorGUILayout.LabelField("F5", "Upgrade current building");
        EditorGUILayout.EndVertical();
    }
}
