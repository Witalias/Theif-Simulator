using UnityEngine;

public class Illumination : MonoBehaviour
{
    [SerializeField] private bool useCustomInitColor = false;
    [SerializeField] private Color customInitColor;

    private MeshRenderer render;

    private Color illuminationColor;
    private Color initColor;

    public bool Enabled { get; set; } = true;

    public void Illuminate() => SetColor(illuminationColor);

    public void RemoveIllumination() => SetColor(useCustomInitColor ? customInitColor : initColor);

    private void Awake()
    {
        render = GetComponent<MeshRenderer>();
        if (render != null)
            initColor = render.material.color;
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
        if (render != null)
            render.material.color = value;

        var renderers = GetComponentsInChildren<MeshRenderer>();
        foreach (var renderer in renderers)
            renderer.material.color = value;
    }
}
