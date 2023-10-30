using UnityEngine;
using System.Linq;

[RequireComponent(typeof(MovingFurnitureElements))]
public class Lootable : MonoBehaviour
{
    [SerializeField] private ResourceType[] containedResources;
    [SerializeField] private Sound sound;
    [SerializeField] private float lootingTime = 2f;
    [SerializeField] [Range(0f, 100f)] private float _tapTimePercents = 5f;
    [SerializeField] private GameObject _hackingArea;

    private MovingFurnitureElements movingFurnitureElements;
    private WaitingAndAction waitingAndAction;

    private bool empty = false;
    private bool _isLooting = false;
    private float _lootingCurrentTime = 0f;
    private readonly ResourceType[] equipmentTypes = new[] { ResourceType.MasterKeys, ResourceType.TierIrons, ResourceType.Gadgets };

    public void TakeLoot(System.Action extraAction = null)
    {
        _isLooting = true;

        var settings = GameSettings.Instanse;
        var findingChances = new[]
        {   
            settings.ChanceOfFinidngMainResource, 
            settings.ChanceOfFindingMoney,
            settings.ChanceOfFindingEquipment + settings.ExtraChanceOfFindingEquipment
        };

        var randomIndex = Randomizator.GetRandomIndexByChances(findingChances);
        var equipmentChances = new[] { settings.ChanceOfFindingMasterKeys, settings.ChanceOfFindingTierIrons };
        switch (randomIndex)
        {
            case 0: TakeResource(containedResources[Random.Range(0, containedResources.Length)], extraAction); break;
            case 1: TakeResource(ResourceType.Money, extraAction); break;
            case 2: TakeResource(equipmentTypes[Randomizator.GetRandomIndexByChances(equipmentChances)], extraAction); break;
        }
    }

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
            TakeLoot();

        _hackingArea.SetActive(!_isLooting);
    }

    private void RemoveIllumination()
    {
        var illumination = GetComponent<Illumination>();
        if (illumination != null)
        {
            illumination.RemoveIllumination();
            illumination.Enabled = false;
        }
    }

    private void TakeResource(ResourceType type, System.Action extraAction)
    {
        var settings = GameSettings.Instanse;
        var gap = settings.GetAmountResourceFound(type);
        var count = Random.Range(gap.x, gap.y) + Stats.Instanse.GetExtraResource(type);

        if (System.Array.Exists(equipmentTypes, element => element == type))
        {
            var extraToolChance = settings.GetChanceOfFindingExtraTool(type);
            count += new[] { 1, 0 }[Randomizator.GetRandomIndexByChances(new[] { extraToolChance, 100f - extraToolChance })];
        }

        RemoveIllumination();
        SoundManager.Instanse.Play(sound);
        void ActionDone()
        {
            empty = true;
            _isLooting = false;
            movingFurnitureElements.Move();
            Stats.Instanse.AddResource(type, count);
            PlayResourceAnimation(type, (int)count);
            extraAction?.Invoke();
            SoundManager.Instanse.Play(GameStorage.Instanse.GetResourceSound(type));
        }
        void ActionAbort()
        {
            _isLooting = false;
            _lootingCurrentTime = waitingAndAction.CurrentTime;
        }
        waitingAndAction.WaitAndExecute(lootingTime, ActionDone, ActionAbort, sound, _lootingCurrentTime);
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
