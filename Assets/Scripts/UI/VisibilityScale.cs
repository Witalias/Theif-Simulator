using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class VisibilityScale : MonoBehaviour
{
    private const float mainBarStep = 0.2f;
    private const float scaleFlashingTime = 1f;
    private const string showAndHideAnimationName = "Show And Hide";
    private const string flashAnimationName = "Flash";

    [SerializeField] private float barsSpeed = 10f;
    [SerializeField] private float increaseValueInSecond = 0.05f;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI levelNumber;
    [SerializeField] private UIBar mainBar;
    [SerializeField] private UIBar extraBar;
    [SerializeField] private Animation scaleAnimation;
    [SerializeField] private TextMeshProUGUI message;

    private Animation messageAnimation;
    private MessageQueue messageQueue;

    private Color scaleInitColor;
    private int level = 0;
    private float mainBarValue = 0f;
    private float extraBarValue = 0f;
    private float mainBarValueReached = 0f;
    private float extraBarValueReached = 0f;

    /// <param name="value">From 0.0 to 5.0</param>
    public void Add(float value)
    {
        if (value > 5f)
            value = 5f;

        var integerPart = Mathf.Floor(value);
        if (integerPart > 0)
            AddCells((int)integerPart);
        extraBarValueReached += value - integerPart;
    }

    private void Awake()
    {
        messageAnimation = message.GetComponent<Animation>();
        scaleInitColor = scaleAnimation.GetComponent<Image>().color;
    }

    private void Start()
    {
        messageQueue = GameObject.FindGameObjectWithTag(Tags.MessageQueue.ToString()).GetComponent<MessageQueue>();
        title.text = Translation.GetVisibilityName();
        SetLevel(0);
        StartCoroutine(Add());
    }

    private void Update()
    {
        mainBarValue = Mathf.Lerp(mainBarValue, mainBarValueReached, Time.deltaTime * barsSpeed);
        extraBarValue = Mathf.Lerp(extraBarValue, extraBarValueReached, Time.deltaTime * barsSpeed);

        mainBar.SetValue(mainBarValue * 100f);
        extraBar.SetValue(extraBarValue * 100f);

        if (extraBar.Filled)
        {
            extraBarValue = 0f;
            extraBarValueReached -= 1f;
            AddCells(1);
        }

        if (mainBar.Filled)
        {
            mainBarValue = 0f;
            mainBarValueReached -= 1f;
            SetLevel(level + 1);
            Debug.Log("New visibility level " + level);
        }
    }

    private void SetLevel(int value)
    {
        if (value > level)
            messageQueue.Add(new MainMessage(null, $"{Translation.GetVisibilityLevelName()} {value}", ""));

        level = value;
        levelNumber.text = value.ToString();
    }

    private void AddCells(int value)
    {
        mainBarValueReached += mainBarStep * value;
        mainBarValueReached = Mathf.Round(mainBarValueReached / mainBarStep) * mainBarStep;

        message.gameObject.SetActive(true);
        message.text = $"+{value} {Translation.GetVisibilityName()}";
        messageAnimation.Play(showAndHideAnimationName);

        scaleAnimation.Play(flashAnimationName);
        StartCoroutine(StopScaleFlash());
    }

    private IEnumerator Add()
    {
        yield return new WaitForSeconds(0.01f);
        extraBarValueReached += increaseValueInSecond * 0.01f;
        StartCoroutine(Add());
    }

    private IEnumerator StopScaleFlash()
    {
        yield return new WaitForSeconds(scaleFlashingTime);
        scaleAnimation.GetComponent<Image>().color = scaleInitColor;
        scaleAnimation.Stop();
    }
}
