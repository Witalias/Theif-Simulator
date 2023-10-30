using UnityEngine;

public class TriggerZone : MonoBehaviour
{
    [SerializeField] private ActionControls action = ActionControls.None;

    private UIHotkey hotkey;

    public bool Triggered { get; private set; } = false;

    public void SetTrigger()
    {
        Triggered = true;
        if (action != ActionControls.None)
            ShowHotkey();
    }

    public void RemoveTrigger()
    {
        Triggered = false;
        if (action != ActionControls.None)
            HideHotkey();
    }

    public void ShowHotkey()
    {
        //if (hotkey != null)
        //    return;

        //hotkey = Instantiate(
        //    GameStorage.Instanse.HotkeyPrefab,
        //    Camera.main.WorldToScreenPoint(transform.position),
        //    Quaternion.identity, GameStorage.Instanse.MainCanvas).GetComponent<UIHotkey>();
        //hotkey.SetKey(Controls.Instanse.GetKey(action));
        //hotkey.Show();
    }

    public void HideHotkey()
    {
        //if (hotkey != null)
        //    hotkey.Hide();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<MovementController>() != null)
            SetTrigger();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<MovementController>() != null)
            RemoveTrigger();
    }
}
