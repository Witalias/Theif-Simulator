using DG.Tweening;
using UnityEngine;

public class Stealth : MonoBehaviour
{
    [SerializeField] private float _animationDuration;
    [SerializeField] private Transform _body;
    [SerializeField] private Transform _box;
    [SerializeField] private CapsuleCollider _collider;
    [SerializeField] private Rigidbody _rigidbody;

    private Vector3 _defaultBodyScale;
    private Vector3 _defaultBoxScale;

    public bool Hided { get; private set; }

    private void Awake()
    {
        _defaultBodyScale = _body.localScale;
        _defaultBoxScale = _box.localScale;
        _box.localScale = Vector3.zero;
        _box.gameObject.SetActive(true);
    }

    public void Show()
    {
        _box.DOScale(Vector3.zero, _animationDuration);
        _body.DOScale(_defaultBodyScale, _animationDuration);
        //_collider.isTrigger = false;
        _rigidbody.isKinematic = false;
        Hided = false;
    }

    public void Hide()
    {
        _box.DOScale(_defaultBoxScale, _animationDuration);
        _body.DOScale(Vector3.zero, _animationDuration);
        //_collider.isTrigger = true;
        _rigidbody.isKinematic = true;
        Hided = true;
    }
}
