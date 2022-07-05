using UnityEngine;

public class Illumination : MonoBehaviour
{
    private Color illuminationColor;
    private Color initColor;

    public bool Enabled { get; set; } = true;

    public void Illuminate() => SetColor(illuminationColor);

    public void RemoveIllumination() => SetColor(initColor);

    private void Awake()
    {
        initColor = GetComponent<Renderer>().material.color;
    }

    private void Start()
    {
        illuminationColor = GameSettings.Instanse.IlluminationColor;
    }

    private void OnMouseEnter()
    {
        if (Enabled)
            Illuminate();
    }

    private void OnMouseExit()
    {
        if (Enabled)
            RemoveIllumination();
    }

    private void SetColor(Color value)
    {
        var renderers = GetComponentsInChildren<Renderer>();
        foreach (var renderer in renderers)
            renderer.material.color = value;
    }
}
