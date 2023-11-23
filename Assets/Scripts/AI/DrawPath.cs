using System.Drawing;
using UnityEngine;

[RequireComponent(typeof(PathTrajectory))]
public class DrawPath : MonoBehaviour
{
    [SerializeField] private GameObject _pointPrefab;
    [SerializeField] private GameObject _linePrefab;

    public void Draw(Transform[] points, bool loop)
    {
        if (points.Length <= 1)
            return;

        var line = Instantiate(_linePrefab, points[0].position, Quaternion.identity, points[0].parent)
            .GetComponent<LineRenderer>();
        line.positionCount = 1;
        line.SetPosition(0, points[0].position);
        line.loop = loop;

        for (var i = 0; i < points.Length; i++)
        {
            if ((i == 0 || i == points.Length - 1) && !loop)
                Instantiate(_pointPrefab, points[i].position, _pointPrefab.transform.rotation, points[i]);
            if (i > 0)
            {
                line.positionCount++;
                line.SetPosition(i, points[i].position);
            }    
        }
    }
}
