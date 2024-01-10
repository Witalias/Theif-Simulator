using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class UnlockArea : MonoBehaviour
{
    [SerializeField] private int _requiredLevel;
    [SerializeField] private int _cost;
    [SerializeField] private float _purchaseSpeed;
    [SerializeField] private TMP_Text _requiredLevelText;
    [SerializeField] private TMP_Text _costText;
    [SerializeField] private GameObject _requiredLevelPanel;
    [SerializeField] private Transform _arrow;
    [SerializeField] private UnityEvent _onPurchase;

    private bool _triggered;
    private Coroutine _purchaseCoroutine;

    public void OnPlayerStay(MovementController player)
    {
        if (_cost <= 0 || _requiredLevel > Stats.Instanse.Level)
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
        Purchase();
    }

    private void Awake()
    {
        SetCostText(_cost);
        SetRequiredLevelText(_requiredLevel);
    }

    private void Start()
    {
        CheckLevel(Stats.Instanse.Level);
    }

    private void OnEnable()
    {
        Stats.NewLevelReached += CheckLevel;
    }

    private void OnDisable()
    {
        Stats.NewLevelReached -= CheckLevel;
    }

    private void Purchase()
    {
        _purchaseCoroutine = StartCoroutine(Coroutine());
        IEnumerator Coroutine()
        {
            var wait = new WaitForSeconds(Time.deltaTime / _purchaseSpeed);
            while (_cost > 0)
            {
                if (Stats.Instanse.Money <= 0)
                    yield break;

                SetCostText(--_cost);
                Stats.Instanse.AddMoney(-1);
                yield return wait;
            }
            TaskManager.Instance.ProcessTask(TaskType.TutorialBuyZone, 1);
            _onPurchase?.Invoke();
            Hide();
        }
    }

    private void AbortPurchase()
    {
        if (_purchaseCoroutine != null)
            StopCoroutine(_purchaseCoroutine);
    }

    private void Hide()
    {
        DOTween.Sequence()
            .Append(transform.DOScale(transform.localScale + new Vector3(0.3f, 0.3f, 0.3f), 0.2f))
            .Append(transform.DOScale(Vector3.zero, 0.25f))
            .OnComplete(() => Destroy(gameObject))
            .Play();
    }

    private void ShowArrow()
    {
        _arrow.gameObject.SetActive(true);
        //DOTween.Sequence()
        //    .Append(_arrow.DOLocalMoveZ(_arrow.position.x - 2.0f, 1.0f))
        //    .Append(_arrow.DOLocalMoveZ(_arrow.position.x + 2.0f, 1.0f))
        //    .SetLoops(-1);
    }

    private void CheckLevel(int level)
    {
        if (level >= _requiredLevel)
        {
            _requiredLevelPanel.SetActive(false);
            _costText.gameObject.SetActive(true);
            //ShowArrow();
        }
    }

    private void SetCostText(int cost) => _costText.text = $"{cost} <sprite=0>";

    private void SetRequiredLevelText(int level) => _requiredLevelText.text = $"LVL {level}";
}
