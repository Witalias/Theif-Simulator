using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ActionMenu : MonoBehaviour
{
    private const float timeBetweenButtons = 0.1f;
    private const float distanceBetweenButtons = 120f;

    [SerializeField] private Obstacle obstacleType;
    [SerializeField] private EquipmentType[] equipmentList;

    private Camera mainCamera;

    private readonly List<ActionMenuButton> buttons = new List<ActionMenuButton>();
    private bool busy = false;
    private bool closeAfterOpen = false;

    public bool Opened { get; private set; } = false;

    public IEnumerator Open()
    {
        if (buttons.Count > 0)
            yield break;

        busy = true;
        Opened = true;
        var storage = GameStorage.Instanse;
        var scaleFactor = storage.MainCanvas.GetComponent<Canvas>().scaleFactor;
        var initButtonPosition = mainCamera.WorldToScreenPoint(transform.position);

        for (var i = 0; i < equipmentList.Length; ++i)
        {
            var correctButtonPosition = new Vector3(initButtonPosition.x, initButtonPosition.y + i * ActionMenuButton.Height * ActionMenuButton.Scale * scaleFactor, initButtonPosition.z);
            var button = Instantiate(storage.ActionMenuButtonPrefab, correctButtonPosition, Quaternion.identity, storage.MainCanvas);
            var buttonScript = button.GetComponent<ActionMenuButton>();
            buttons.Add(buttonScript);
            buttonScript.SetActionCloseMenu(() => StartCoroutine(Close()));
            buttonScript.SetEquipment(equipmentList[i], obstacleType, gameObject);

            if (equipmentList[i] != EquipmentType.Arms && (int)Stats.Instanse.GetResource(equipmentList[i]) <= 0)
                buttonScript.Lock();

            buttonScript.Show();
            yield return new WaitForSeconds(timeBetweenButtons);
        }
        busy = false;

        if (closeAfterOpen)
        {
            closeAfterOpen = false;
            StartCoroutine(Close());
        }
    }

    public IEnumerator Close()
    {
        if (busy)
        {
            if (Opened)
                closeAfterOpen = true;
            yield break;
        }

        busy = true;
        Opened = false;
        for (var i = buttons.Count - 1; i >= 0; --i)
        {
            buttons[i].Hide();
            yield return new WaitForSeconds(timeBetweenButtons);
        }
        buttons.Clear();
        busy = false;
    }

    private void Awake()
    {
        mainCamera = Camera.main;
    }
}
