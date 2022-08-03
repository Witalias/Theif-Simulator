using UnityEngine;
using System.Collections.Generic;
using System;

public class PathTrajectory : MonoBehaviour
{
    private Queue<Vector3> path = null;
    private Transform body;
    private Vector3 start;
    private Vector3 end;
    private bool smoothly;
    private float speed = 1f;
    private float detectionDistance;
    private Action actionAfterFinishing = null;

    private float interpolationValue = 0;

    public bool Finished { get; private set; } = true;

    public void Go(Transform body, Queue<Vector3> path, float speed, bool smoothly, Action actionAfterFinishing, float detectionDistance)
    {
        this.path = path;
        this.smoothly = smoothly;
        this.body = body;
        this.speed = speed;
        this.actionAfterFinishing = actionAfterFinishing;
        this.detectionDistance = detectionDistance;

        if (path.Count <= 1)
            return;

        start = path.Dequeue();
        end = path.Dequeue();
        Finished = false;
    }

    private void FixedUpdate()
    {
        if (path == null || Finished)
            return;

        if (smoothly)
            body.position = Vector3.Lerp(body.position, end, Time.fixedDeltaTime * speed);
        else
        {
            interpolationValue += Time.fixedDeltaTime * speed;
            body.position = Vector3.Lerp(start, end, interpolationValue);
        }

        if (Vector3.Distance(body.position, end) <= detectionDistance)
        {
            interpolationValue = 0;
            if (path.Count == 0)
                Finish();
            else
            {
                start = end;
                end = path.Dequeue();
            }
        }
    }

    private void Finish()
    {
        path = null;
        start = Vector3.zero;
        end = Vector3.zero;
        Finished = true;
        actionAfterFinishing?.Invoke();
        actionAfterFinishing = null;
    }
}
