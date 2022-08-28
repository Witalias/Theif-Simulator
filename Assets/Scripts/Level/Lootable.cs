using UnityEngine;
using System.Linq;

public class Lootable : MonoBehaviour
{
    [SerializeField] private ResourceType[] containedResources;
    [SerializeField] private float arriveDistance = 2f;
    [SerializeField] private float lootingTime = 2f;

    private MovementController movementController;
    private MovingFurnitureElements movingFurnitureElements;
    private WaitingAndAction waitingAndAction;

    private bool empty = false;

    public float ArriveDistance { get => arriveDistance; }

    public void TakeLoot(System.Action extraAction = null)
    {
        var settings = GameSettings.Instanse;
        var findingChances = new[]
        {   
            settings.ChanceOfFinidngMainResource, 
            settings.ChanceOfFindingMoney,
            settings.ChanceOfFindingEquipment
        };

        var randomIndex = Randomizator.GetRandomIndexByChances(findingChances);
        var equipmentChances = new[] { settings.ChanceOfFindingMasterKeys, settings.ChanceOfFindingTierIrons };
        switch (randomIndex)
        {
            case 0: TakeResource(containedResources[Random.Range(0, containedResources.Length)], extraAction); break;
            case 1: TakeResource(ResourceType.Money, extraAction); break;
            case 2: TakeResource(new[] { ResourceType.MasterKeys, ResourceType.TierIrons }[Randomizator.GetRandomIndexByChances(equipmentChances)], extraAction); break;
        }
    }

    private void Awake()
    {
        movingFurnitureElements = GetComponent<MovingFurnitureElements>();
    }

    private void Start()
    {
        movementController = GameObject.FindGameObjectWithTag(Tags.Player.ToString()).GetComponent<MovementController>();
        waitingAndAction = GameStorage.Instanse.WaitingAndActionPrefab.GetComponent<WaitingAndAction>();
    }

    private void OnMouseDown()
    {
        //var layer = 1 << 7 | 1 << 8 | 1 << 9; // Door Wall, Wall, Window
        var centeredPoint = GetComponent<CenteredPoint>();
        if (!empty && Physics.Raycast(centeredPoint.CenterPoint, movementController.CenterPoint.position - centeredPoint.CenterPoint, out RaycastHit hit))
        {
            if (hit.collider.GetComponent<MovementController>() == null)
                return;

            if (centeredPoint == null)
                movementController.GoToObject(transform.position, this);
            else
                movementController.GoToObject(centeredPoint.CenterPoint, this);
        }
    }

    //private void Update()
    //{
    //    var centeredPoint = GetComponent<CenteredPoint>();
    //    Debug.DrawRay(centeredPoint.CenterPoint, movementController.CenterPoint.position - centeredPoint.CenterPoint);
    //}

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
        var gap = GameSettings.Instanse.GetAmountResourceFound(type);
        var count = Random.Range(gap.x, gap.y);
        RemoveIllumination();
        void Action()
        {
            empty = true;
            movingFurnitureElements.Move();
            Stats.Instanse.AddResource(type, count);
            PlayResourceAnimation(type, (int)count);
            extraAction?.Invoke();
        }
        waitingAndAction.WaitAndExecute(lootingTime, Action);
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
