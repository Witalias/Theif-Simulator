using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using TMPro;

[RequireComponent(typeof(Animation))]
[RequireComponent(typeof(Image))]
public class MessageQueue : MonoBehaviour
{
    private const string showAndHideAnimationName = "Show And Hide";

    [SerializeField] private float delay = 4f;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI text;

    private Animation anim;
    private Image background;

    private readonly Queue<MainMessage> messageQueue = new Queue<MainMessage>();
    private bool showed = false;
    private Color backgroundInitialColor;
    private Color titleInitialColor;

    public void Add(MainMessage message)
    {
        messageQueue.Enqueue(message);

        if (!showed)
            Show();
    }

    private void Awake()
    {
        anim = GetComponent<Animation>();
        background = GetComponent<Image>();
        backgroundInitialColor = background.color;
        titleInitialColor = title.color;
    }

    private void Show()
    {
        if (messageQueue.Count > 0)
        {
            showed = true;
            var message = messageQueue.Dequeue();

            title.text = message.Title;
            text.text = message.Text;
            icon.sprite = message.Sprite;

            if (message.CustomColor)
            {
                title.color = message.TitleColor;
                background.color = message.BackgroundColor;
            }
            else
            {
                title.color = titleInitialColor;
                background.color = backgroundInitialColor;
            }

            anim.Play(showAndHideAnimationName);
            SoundManager.Instanse.Play(message.Sound);
        }
        else
            showed = false;
    }

    private void Next()
    {
        StartCoroutine(NextMessage());
    }

    private IEnumerator NextMessage()
    {
        yield return new WaitForSeconds(delay);
        Show();
    }
}

public class MainMessage
{
    public Sprite Sprite { get; }
    public string Title { get; }
    public string Text { get; }
    public Color TitleColor { get; }
    public Color BackgroundColor { get; }
    public bool CustomColor { get; } = false;
    public Sound Sound { get; }

    public MainMessage(Sprite sprite, string title, string text, Sound sound)
    {
        Sprite = sprite;
        Title = title;
        Text = text;
        Sound = sound;
    }

    public MainMessage(Sprite sprite, string title, string text, Sound sound, Color titleColor, Color backgroundColor)
        : this(sprite, title, text, sound)
    {
        CustomColor = true;
        TitleColor = titleColor;
        BackgroundColor = backgroundColor;
    }
}
