using UnityEngine;
using System.Linq;

public class Lootable : MonoBehaviour
{
    [SerializeField] private ResourceType[] containedResources;
    [SerializeField] private float arriveDistance = 2f;

    private MovementController movementController;
    private MovingFurnitureElements movingFurnitureElements;

    private bool empty = false;

    public float ArriveDistance { get => arriveDistance; }

    public void TakeLoot()
    {
        empty = true;

        var settings = GameSettings.Instanse;
        var chances = new[]
        {   
            settings.ChanceOfFinidngMainResource, 
            settings.ChanceOfFindingMoney 
        };
        var randomNumber = Random.Range(0f, chances.Sum());
        var currentNumber = 0f;
        for (var i = 0; i < chances.Length; ++i)
        {
            currentNumber += chances[i];
            if (randomNumber <= currentNumber)
            {
                switch (i)
                {
                    case 0: TakeResource(containedResources[Random.Range(0, containedResources.Length)]); break;
                    case 1: TakeResource(ResourceType.Money); break;
                }
                break;
            }
        }

        RemoveIllumination();

        movingFurnitureElements.Move();
    }

    private void Awake()
    {
        movingFurnitureElements = GetComponent<MovingFurnitureElements>();
    }

    private void Start()
    {
        movementController = GameObject.FindGameObjectWithTag(Tags.Player.ToString()).GetComponent<MovementController>();
    }

    private void OnMouseDown()
    {
        if (!empty)
        {
            var centeredPoint = GetComponent<CenteredPoint>();
            if (centeredPoint == null)
                movementController.GoToObject(transform.position, this);
            else
                movementController.GoToObject(centeredPoint.CenterPoint, this);
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

    private void TakeResource(ResourceType type)
    {
        var gap = GameSettings.Instanse.GetAmountResourceFound(type);
        var count = Random.Range(gap.x, gap.y);
        Stats.Instanse.AddResource(type, count);
        PlayResourceAnimation(type, (int)count);
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
