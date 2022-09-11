using UnityEngine;
using System.Linq;

[RequireComponent(typeof(MovingFurnitureElements))]
[RequireComponent(typeof(CenteredPoint))]
[RequireComponent(typeof(TargetObject))]
public class Lootable : MonoBehaviour
{
    [SerializeField] private ResourceType[] containedResources;
    [SerializeField] private Sound sound;
    [SerializeField] private float lootingTime = 2f;

    private MovementController movementController;
    private MovingFurnitureElements movingFurnitureElements;
    private WaitingAndAction waitingAndAction;
    private CenteredPoint centeredPoint;
    private TargetObject targetObject;

    private bool empty = false;
    private ResourceType[] equipmentTypes = new[] { ResourceType.MasterKeys, ResourceType.TierIrons, ResourceType.Gadgets };

    public void TakeLoot(System.Action extraAction = null)
    {
        var settings = GameSettings.Instanse;
        var findingChances = new[]
        {   
            settings.ChanceOfFinidngMainResource, 
            settings.ChanceOfFindingMoney,
            settings.ChanceOfFindingEquipment + settings.ExtraChanceOfFindingEquipment
        };

        var randomIndex = Randomizator.GetRandomIndexByChances(findingChances);
        var equipmentChances = new[] { settings.ChanceOfFindingMasterKeys, settings.ChanceOfFindingTierIrons, settings.ChanceOfFindingGadgets };
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
        centeredPoint = GetComponent<CenteredPoint>();
        targetObject = GetComponent<TargetObject>();
    }

    private void Start()
    {
        movementController = GameObject.FindGameObjectWithTag(Tags.Player.ToString()).GetComponent<MovementController>();
        waitingAndAction = GameObject.FindGameObjectWithTag(Tags.TimeCircle.ToString()).GetComponent<WaitingAndAction>();
    }

    private void OnMouseDown()
    {
        if (!empty && Physics.Raycast(centeredPoint.CenterPoint, movementController.CenterPoint.position - centeredPoint.CenterPoint, out RaycastHit hit))
        {
            if (centeredPoint == null)
                movementController.GoToObject(transform.position, targetObject, hit);
            else
                movementController.GoToObject(centeredPoint.CenterPoint, targetObject, hit);
        }
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
        void Action()
        {
            empty = true;
            movingFurnitureElements.Move();
            Stats.Instanse.AddResource(type, count);
            PlayResourceAnimation(type, (int)count);
            extraAction?.Invoke();
            SoundManager.Instanse.Play(GameStorage.Instanse.GetResourceSound(type));
        }
        waitingAndAction.WaitAndExecute(lootingTime, Action, sound, GameSettings.Instanse.HearingRadiusDuringLoot);
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
