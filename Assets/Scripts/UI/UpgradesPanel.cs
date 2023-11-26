using UnityEngine;

[RequireComponent(typeof(OpenClosePopup))]
public class UpgradesPanel : MonoBehaviour
{
    private OpenClosePopup _popup;

    private void Awake()
    {
        _popup = GetComponent<OpenClosePopup>();
    }

    private void OnEnable()
    {
        UpgradesPopupButton.Clicked += _popup.Open;
    }

    private void OnDisable()
    {
        UpgradesPopupButton.Clicked -= _popup.Open;
    }
}
