using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Resource", menuName = "Scriptable Objects/Contained Resource", order = 1)]
public class ContainedResource : ScriptableObject
{
    [SerializeField] private ResourceType _type;
    [SerializeField] private float _dropChance;
    [SerializeField, Tooltip("Количество: индекс + 1\nШанс: значение")] private float[] _countsChances;
    [SerializeField] private bool _onlyMinMaxRange;
    [SerializeField] private Vector2 _minMaxCount;

    public ResourceType Type => _type;
    public float DropChance => _dropChance;
    public float[] CountsChances => _countsChances;
    public bool OnlyMinMaxRange => _onlyMinMaxRange;
    public Vector2 MinMaxCount => _minMaxCount;
}
