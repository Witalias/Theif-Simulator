using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class NoiseEffect : MonoBehaviour
{
    [SerializeField] private float speed;

    private SpriteRenderer sprite;

    private bool played = true;
    private float radius;
    private float currentScaleValue = 1f;

    public void Play(float radius)
    {
        played = false;
        this.radius = radius;
    }

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        if (played)
            return;

        var targetValue = radius * 2f;
        if (currentScaleValue < targetValue)
        {
            currentScaleValue += Time.fixedDeltaTime * radius * speed;
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, Mathf.Lerp(0f, 0.5f, (targetValue - currentScaleValue) / targetValue));
        }
        else
            Destroy(gameObject);

        transform.localScale = new Vector3(currentScaleValue, currentScaleValue, currentScaleValue);
    }
}
