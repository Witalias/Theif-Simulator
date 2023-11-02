using UnityEngine;
using System.Linq;
using System;

[RequireComponent(typeof(MovingFurnitureElements))]
public class Lootable : MonoBehaviour
{
    public static event Action<Action, Action> ShowHoldButton;

    [SerializeField] private ResourceType[] containedResources;
    [SerializeField] private Sound sound;
    [SerializeField] private GameObject _hackingArea;
    [SerializeField] private Transform _centerPoint;

    private MovingFurnitureElements movingFurnitureElements;
    private WaitingAndAction waitingAndAction;

    private bool empty = false;
    private bool _isLooting = false;
    private readonly ResourceType[] equipmentTypes = new[] { ResourceType.MasterKeys, ResourceType.TierIrons, ResourceType.Gadgets };

    private void Awake()
    {
        movingFurnitureElements = GetComponent<MovingFurnitureElements>();
    }

    private void Start()
    {
        waitingAndAction = GameObject.FindGameObjectWithTag(Tags.TimeCircle.ToString()).GetComponent<WaitingAndAction>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (empty || !other.TryGetComponent<MovementController>(out MovementController player))
            return;

        if (!player.IsRunning && !_isLooting)
        {
            player.RotateTowards(_centerPoint.position);
            TakeResource();
        }

        _hackingArea.SetActive(!_isLooting);
    }

    private void TakeResource(System.Action extraAction = null)
    {
        _isLooting = true;
        void ActionDone()
        {
            SoundManager.Instanse.Play(sound);
            empty = true;
            _isLooting = false;
            movingFurnitureElements.Move();
            //Stats.Instanse.AddResource(type, count);
            //PlayResourceAnimation(type, (int)count);
            //extraAction?.Invoke();
            //SoundManager.Instanse.Play(GameStorage.Instanse.GetResourceSound(type));
        }
        void ActionAbort()
        {
            _isLooting = false;
        }
        ShowHoldButton?.Invoke(ActionDone, ActionAbort);
    }

    private void PlayResourceAnimation(ResourceType type, int count)
    {
        var text = count.ToString();
        if (type == ResourceType.Food || type == ResourceType.Water)
            text += '%';
        var newResourceObject = Instantiate(GameStorage.Instanse.NewResourceAnimatinPrefab, Camera.main.WorldToScreenPoint(transform.position), Quaternion.identity, GameStorage.Instanse.MainCanvas);
        var newResource = newResourceObject.GetComponent<NewResourceAnimation>();
        newResource.SetIcon(GameStorage.Instanse.GetResourceSprite(type));
        newResource.SetText(text);
    }
}
