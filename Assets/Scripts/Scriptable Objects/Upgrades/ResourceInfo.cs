using UnityEngine;

[CreateAssetMenu(fileName = "Info", menuName = "Scriptable Objects/Resource Info", order = 1)]
public class ResourceInfo : ScriptableObject
{
    public ResourceType Type;
    public Sprite Sprite;
    public Sound Sound;
    public int Price;
}
