using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CreatureVision))]
public class SecurityCamera : MonoBehaviour
{
    [SerializeField] [Range(0f, 5f)] private float visibilityValueInSecond = 0.05f;
    [SerializeField] private Animation recAnimation;
    [SerializeField] private GameObject redPoint;
    [SerializeField] private Illumination illumination;
    [SerializeField] private Device device;

    private VisibilityScale visibilityScale;
    private CreatureVision vision;

    private bool recording = false;

    private void Awake()
    {
        recAnimation.gameObject.SetActive(false);
        vision = GetComponent<CreatureVision>();

        void ActionAfterTurnedOff()
        {
            redPoint.SetActive(false);
            if (illumination != null)
                illumination.Enabled = false;
        }

        device.SetEvent(ActionAfterTurnedOff);
    }

    private void Start()
    {
        visibilityScale = GameObject.FindGameObjectWithTag(Tags.VisibilityScale.ToString()).GetComponent<VisibilityScale>();
    }

    private void Update()
    {
        if (vision.SeesTarget && !device.TurnedOff)
        {
            if (!recording)
            {
                recording = true;
                StartCoroutine(AddVisibility());
                recAnimation.gameObject.SetActive(true);
                recAnimation.Play();
            }
        }
        else if (recording)
        {
            recording = false;
            recAnimation.gameObject.SetActive(false);
        }

    }

    private IEnumerator AddVisibility()
    {
        visibilityScale.Add(visibilityValueInSecond * 0.01f);
        yield return new WaitForSeconds(0.01f);
        if (vision.SeesTarget)
            StartCoroutine(AddVisibility());
    }
}
