using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using TMPro;

[RequireComponent(typeof(Animation))]
public class MessageQueue : MonoBehaviour
{
    private const string showAndHideAnimationName = "Show And Hide";

    [SerializeField] private float delay = 4f;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI text;

    private Animation anim;

    private readonly Queue<MainMessage> messageQueue = new Queue<MainMessage>();
    private bool showed = false;

    public void Add(MainMessage message)
    {
        messageQueue.Enqueue(message);

        if (!showed)
            Show();
    }

    private void Awake()
    {
        anim = GetComponent<Animation>();
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
            anim.Play(showAndHideAnimationName);
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

    public MainMessage(Sprite sprite, string title, string text)
    {
        Sprite = sprite;
        Title = title;
        Text = text;
    }
}
