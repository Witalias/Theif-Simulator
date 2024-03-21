using UnityEngine;

[CreateAssetMenu(fileName = "Info", menuName = "Scriptable Objects/Resource Info", order = 1)]
public class ResourceInfo : ScriptableObject
{
    public ResourceType Type;
    public Sprite Sprite;
    public AudioType Sound;
    public int Price;
}
