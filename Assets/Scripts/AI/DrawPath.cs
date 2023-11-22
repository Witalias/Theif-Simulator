using UnityEngine;

[RequireComponent(typeof(PathTrajectory))]
public class DrawPath : MonoBehaviour
{
    [SerializeField] private GameObject _pointPrefab;
    [SerializeField] private GameObject _linePrefab;

    public void Draw(Transform[] points, bool loop)
    {
        for (var i = 0; i < points.Length; i++)
        {
            Instantiate(_pointPrefab, points[i].position, _pointPrefab.transform.rotation, points[i]);
            if (i > 0)
            {
                var lineRenderer = CreateLine(points[i - 1]);
                lineRenderer.positionCount = 2;
                lineRenderer.SetPosition(0, points[i - 1].position);
                lineRenderer.SetPosition(1, points[i].position);

                if (i ==  points.Length - 1 && loop)
                {
                    lineRenderer = CreateLine(points[i]);
                    lineRenderer.positionCount = 2;
                    lineRenderer.SetPosition(0, points[i].position);
                    lineRenderer.SetPosition(1, points[0].position);
                }
            }    
        }
    }

    private LineRenderer CreateLine(Transform point)
    {
        return Instantiate(_linePrefab, point.position, Quaternion.identity, point)
            .GetComponent<LineRenderer>();
    }
}
