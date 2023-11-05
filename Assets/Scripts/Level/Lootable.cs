using UnityEngine;
using System.Linq;
using System;

[RequireComponent(typeof(MovingFurnitureElements))]
public class Lootable : MonoBehaviour
{
    public static event Action<Action, Action> ShowHoldButton;

    [Tooltip("Counts Changes: индекс+1 - количество предметов")]
    [SerializeField] private ItemsDropChance[] _containedResources;
    [SerializeField] private Sound sound;
    [SerializeField] private GameObject _hackingArea;
    [SerializeField] private GameObject _appearHackingZoneTrigger;

    private MovingFurnitureElements _movingFurnitureElements;
    private Camera _mainCamera;

    private bool _empty = false;
    private bool _isLooting = false;

    public void Loot(MovementController player)
    {
        if (_empty)
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
        _movingFurnitureElements = GetComponent<MovingFurnitureElements>();
        _mainCamera = Camera.main;
    }

    private void TakeResource()
    {
        _isLooting = true;
        SoundManager.Instanse.Play(sound);
        void ActionDone()
        {
            _empty = true;
            _isLooting = false;
            _appearHackingZoneTrigger.SetActive(false);
            _movingFurnitureElements.Move();

            if (_containedResources.Length == 0)
                return;

            var randomIndex = Randomizator.GetRandomIndexByChances(_containedResources.Select(item => item.DropChance).ToArray());
            var randomResource = _containedResources[randomIndex];
            var count = Randomizator.GetRandomIndexByChances(randomResource.CountsChanges) + 1;
            if (randomResource.OnlyMinMaxRange)
                count = (int)UnityEngine.Random.Range(randomResource.MinMaxCount.x, randomResource.MinMaxCount.y);
            Stats.Instanse.AddResource(randomResource.Type, count);
            PlayResourceAnimation(randomResource.Type, count);
            SoundManager.Instanse.Play(GameStorage.Instanse.GetResourceSound(randomResource.Type));
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
        var newResourceObject = Instantiate(GameStorage.Instanse.NewResourceAnimatinPrefab, _mainCamera.WorldToScreenPoint(transform.position), Quaternion.identity, GameStorage.Instanse.MainCanvas);
        var newResource = newResourceObject.GetComponent<NewResourceAnimation>();
        newResource.SetIcon(GameStorage.Instanse.GetResourceSprite(type));
        newResource.SetText(text);
    }

    [Serializable]
    public class ItemsDropChance
    {
        public ResourceType Type;
        public float DropChance;
        public float[] CountsChanges;
        public bool OnlyMinMaxRange;
        public Vector2 MinMaxCount;
    }
}
