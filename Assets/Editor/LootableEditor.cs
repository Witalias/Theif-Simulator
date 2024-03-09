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
    }
}
