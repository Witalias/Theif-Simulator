using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System;
using System.Collections;

public class ActionMenuButton : MonoBehaviour, IPointerExitHandler, IPointerMoveHandler, IPointerClickHandler
{
    public static float Height { get; private set; }
    public static float Scale { get; private set; }

    private const string showAnimationName = "Show";
    private const string hideAnimationName = "Hide";
    private const string increaseAnimationName = "Increase";
    private const string decreaseAnimationName = "Decrease";

    [SerializeField] private Image background;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI timeValue;
    [SerializeField] private TextMeshProUGUI loudnessValue;
    [SerializeField] private Color lockedColor;

    private Animation anim;
    private WaitingAndAction waitingAndAction;

    private EquipmentStats equipmentStats;
    private Action clickAction;
    private Action closeMenu;
    private bool selected = false;

    public bool Locked { get; private set; }

    public void Lock()
    {
        Locked = true;
        background.color = lockedColor;
    }

    public void SetEquipment(EquipmentType equipmentType, Obstacle obstacleType, GameObject target)
    {
        equipmentStats = Stats.Instanse.GetEquipmentStats(equipmentType);

        var hackingTime = 1f;
        void ActionAfterWaitingForDoorOrWindow()
        {
            target.GetComponent<Lockable>().Locked = false;
            target.GetComponent<TriggerZone>().SetTrigger();
            Stats.Instanse.AddResource(equipmentType, -1);
        }

        switch (obstacleType)
        {
            case Obstacle.Door: clickAction = () =>
            {
                void ActionAfterWaiting()
                {
                    ActionAfterWaitingForDoorOrWindow();
                    target.GetComponent<Lockable>().Open();
                }
                waitingAndAction.WaitAndExecute(equipmentStats.HackingTimeDoor, ActionAfterWaiting);
            };
                hackingTime = equipmentStats.HackingTimeDoor;
                break;

            case Obstacle.Window: clickAction = () =>
            {
                waitingAndAction.WaitAndExecute(equipmentStats.HackingTimeWindow, ActionAfterWaitingForDoorOrWindow);
            };
                hackingTime = equipmentStats.HackingTimeWindow;
                break;

            case Obstacle.Device: hackingTime = equipmentStats.HackingTimeDevice; break;
        }

        SetIcon(equipmentStats.Icon);
        SetTitle(Translation.Get(equipmentType));
        SetTimeValue((int)Mathf.Ceil(hackingTime));
        SetLoudnessType(equipmentStats.LoudnessType);
    }

    public void SetActionCloseMenu(Action value) => closeMenu = value;

    public void Show() => anim.Play(showAnimationName);

    public void Hide() => anim.Play(hideAnimationName);

    private void SetIcon(Sprite value) => icon.sprite = value;

    private void SetTitle(string value) => title.text = value;

    private void SetTimeValue(int value) => timeValue.text = $"{value} {Translation.GetSecondsAbbreviation()}";

    private void SetLoudnessType(LoudnessType value) => loudnessValue.text = Translation.Get(value);

    private void Awake()
    {
        Height = background.rectTransform.sizeDelta.y;
        Scale = background.rectTransform.localScale.y;
        anim = GetComponent<Animation>();
    }

    private void Start()
    {
        //waitingAndAction = GameObject.FindGameObjectWithTag(Tags.TimeCircle.ToString()).GetComponent<WaitingAndAction>();
        waitingAndAction = GameStorage.Instanse.WaitingAndActionPrefab.GetComponent<WaitingAndAction>();
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (!selected)
            Select();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (selected)
            NotSelect();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Locked)
            return;

        closeMenu?.Invoke();
        StartCoroutine(ExecuteAction());
    }

    private void Select()
    {
        if (Locked || anim.isPlaying)
            return;

        anim.Play(increaseAnimationName);
        selected = true;
    }

    private void NotSelect()
    {
        if (Locked || anim.IsPlaying(hideAnimationName))
            return;

        anim.Play(decreaseAnimationName);
        selected = false;
    }

    private IEnumerator ExecuteAction()
    {
        yield return new WaitForSeconds(0.3f);
        clickAction?.Invoke();
    }
}
