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
    [SerializeField] private GameObject _appearHackingZoneTrigger;

    private MovingFurnitureElements movingFurnitureElements;

    private bool empty = false;
    private bool _isLooting = false;

    public void Loot(MovementController player)
    {
        if (empty)
            return;

        if (!player.IsRunning && !_isLooting)
        {
            player.RotateTowards(_appearHackingZoneTrigger.transform.position);
            TakeResource();
        }

        _hackingArea.SetActive(!_isLooting);
    }

    private void Awake()
    {
        movingFurnitureElements = GetComponent<MovingFurnitureElements>();
    }

    private void TakeResource(System.Action extraAction = null)
    {
        _isLooting = true;
        void ActionDone()
        {
            SoundManager.Instanse.Play(sound);
            empty = true;
            _isLooting = false;
            _appearHackingZoneTrigger.SetActive(false);
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
