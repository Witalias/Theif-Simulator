using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(Lootable))]
public class LootableEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var lootable = (Lootable)target;
        lootable.SetActiveTriggerZone(lootable.Enabled);
        lootable.SetActiveRefillTimer(lootable.Regenerative);
        lootable.SetActiveShine(!lootable.Valuable);
        lootable.SetActiveGoldShine(lootable.Valuable);
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("New Level", "F3");
        EditorGUILayout.EndVertical();
    }
}
