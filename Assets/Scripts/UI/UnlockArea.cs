using Cinemachine;
using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class UnlockArea : MonoBehaviour, IIdentifiable
{
    public static event Action CostChanged;
    public static event Action<CinemachineVirtualCamera> MoveCamera;

    [SerializeField] private int _requiredLevel;
    [SerializeField] private int _cost;
    [SerializeField, Range(0, 1)] private float _purchaseSpeed;
    [SerializeField] private ResourceType[] _newAvailableResources;
    [SerializeField] private TMP_Text _requiredLevelText;
    [SerializeField] private TMP_Text _costText;
    [SerializeField] private GameObject _requiredLevelPanel;
    [SerializeField] private CinemachineVirtualCamera _virtualCamera;
    [SerializeField] private ParticleSystem _purchaseParticle;
    [SerializeField] private UnityEvent _onPurchase;

    private int _initialCost;
    private bool _triggered;
    private bool _loaded;
    private bool _isUnlocked;
    private Coroutine _purchaseCoroutine;
    private readonly SavedData _savedData = new();

    public int ID { get; set; }

    public SavedData Save()
    {
        _savedData.ID = ID;
        _savedData.Cost = _cost;
        return _savedData;
    }

    public void Load(SavedData data)
    {
        _cost = data.Cost;
        if (_cost <= 0)
            Purchase(true);
        else      
            SetCostText(_cost);
    }

    public void OnPlayerStay(MovementController player)
    {
        if (_cost <= 0 || _requiredLevel > GameData.Instanse.PlayerLevel.Level)
            return;

        if (player.IsRunning)
        {
            if (_triggered)
            {
                _triggered = false;
                AbortPurchase();
            }
            return;
        }
        else if (_triggered || player.Busy)
            return;

        _triggered = true;
        ProcessPurchase();
    }

    private void Awake()
    {
        _initialCost = _cost;
    }

    private void Start()
    {
        SetCostText(_cost);
        SetRequiredLevelText(_requiredLevel);
        CheckLevel(GameData.Instanse.PlayerLevel.Level);
        _loaded = true;
    }

    private void OnEnable()
    {
        PlayerLevelController.NewLevelReached += CheckLevel;
    }

    private void OnDisable()
    {
        PlayerLevelController.NewLevelReached -= CheckLevel;
    }

    private void ProcessPurchase()
    {
        _purchaseCoroutine = StartCoroutine(Coroutine());
        IEnumerator Coroutine()
        {
            var wait = new WaitForEndOfFrame();
            while (_cost > 0)
            {
                if (GameData.Instanse.Money <= 0)
                    yield break;

                var previous = _cost;
                _cost = Mathf.Clamp(_cost - (int)Mathf.Ceil(Mathf.Lerp(0, _cost, _purchaseSpeed)), 0, int.MaxValue);
                SetCostText(_cost);
                GameData.Instanse.AddMoney(_cost - previous);
                yield return wait;
            }
            TaskManager.Instance.ProcessTask(TaskType.TutorialBuyZone, 1);
            Purchase();
        }
    }

    private void Purchase(bool loaded = false)
    {
        _onPurchase?.Invoke();
        CostChanged?.Invoke();
        TaskManager.Instance.AddAvailableResources(_newAvailableResources);
        Hide();
        if (!loaded)
        {
            MetricaSender.UnlockedArea(_initialCost);
            AudioManager.Instanse.Play(AudioType.NewArea);
            ShowPurchaseParticle();
        }
    }

    private void ShowPurchaseParticle()
    {
        _purchaseParticle.transform.parent = null;
        _purchaseParticle.Play();
        DOVirtual.DelayedCall(1.0f, () => _purchaseParticle.Stop());
    }

    private void AbortPurchase()
    {
        if (_purchaseCoroutine != null)
            StopCoroutine(_purchaseCoroutine);
        CostChanged?.Invoke();
    }

    private void Hide()
    {
        DOTween.Sequence()
            .Append(transform.DOScale(transform.localScale + new Vector3(0.3f, 0.3f, 0.3f), 0.2f))
            .Append(transform.DOScale(Vector3.zero, 0.25f))
            .OnComplete(() => Destroy(gameObject))
            .Play();
    }

    private void CheckLevel(int level)
    {
        if (level >= _requiredLevel && !_isUnlocked)
        {
            _isUnlocked = true;
            _requiredLevelPanel.SetActive(false);
            _costText.gameObject.SetActive(true);

            if (_loaded)
                MoveCamera?.Invoke(_virtualCamera);
            //ShowArrow();
        }
    }

    private void SetCostText(int cost) => _costText.text = $"{cost} <sprite=0>";

    private void SetRequiredLevelText(int level) => _requiredLevelText.text = $"{Translation.GetLevelNameAbbreviated()} {level}";

    [Serializable]
    public class SavedData
    {
        public int ID;
        public int Cost;
    }
}
