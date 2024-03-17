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
        lootable.SetActiveShineContainer(lootable.Enabled);
        lootable.SetActiveRefillTimer(lootable.Regenerative);
        lootable.SetActiveNormalShine(!lootable.Valuable);
        lootable.SetActiveGoldShine(lootable.Valuable);
    }
}
