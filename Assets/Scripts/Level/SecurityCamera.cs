using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CreatureVision))]
public class SecurityCamera : MonoBehaviour
{
    [SerializeField] [Range(0f, 5f)] private float visibilityValueInSecond = 0.05f;
    [SerializeField] private Animation recAnimation;
    [SerializeField] private GameObject redPoint;

    private CreatureVision vision;

    private bool recording = false;

    private void Awake()
    {
        recAnimation.gameObject.SetActive(false);
        vision = GetComponent<CreatureVision>();
    }

    private void Update()
    {
        if (vision.SeesTarget)
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
        yield return new WaitForSeconds(0.01f);
        if (vision.SeesTarget)
            StartCoroutine(AddVisibility());
    }
}
