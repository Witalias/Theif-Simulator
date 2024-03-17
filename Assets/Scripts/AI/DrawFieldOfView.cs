using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DrawFieldOfView : MonoBehaviour
{
    [SerializeField] private Material _coneMaterial;
    [SerializeField] private float _range;
    [SerializeField] private float _angle;
    [SerializeField] private LayerMask _obstructingLayer;
    [SerializeField] private int _coneResolution = 120;
    [SerializeField] private FieldOfView _fieldOfView;

    private MeshRenderer _meshRenderer;
    private Mesh _coneMesh;
    private MeshFilter _meshFilter;

    public Material Material => _meshRenderer.material;

    private void Awake()
    {
        if (_fieldOfView != null)
        {
            _range = _fieldOfView.Radius;
            _angle = _fieldOfView.Angle;
        }
        _meshRenderer = transform.AddComponent<MeshRenderer>();
        _meshRenderer.material = _coneMaterial;
        _meshFilter = transform.AddComponent<MeshFilter>();
        _coneMesh = new Mesh();
        _angle *= Mathf.Deg2Rad;
    }

    private void Update()
    {
        DrawVisionCone();
    }

    public void SetMaterial(Material material) => _meshRenderer.material = material;

    private void DrawVisionCone()
    {
        var triangles = new int[(_coneResolution - 1) * 3];
        var vertices = new Vector3[_coneResolution + 1];
        vertices[0] = Vector3.zero;
        var currentAngle = -_angle / 2;
        var angleIcrement = _angle / (_coneResolution - 1);

        for (var i = 0; i < _coneResolution; i++)
        {
            var sin = Mathf.Sin(currentAngle);
            var cos = Mathf.Cos(currentAngle);
            var raycastDirection = (transform.forward * cos) + (transform.right * sin);
            var verticeForward = (Vector3.forward * cos) + (Vector3.right * sin);
            if (Physics.Raycast(transform.position, raycastDirection, out RaycastHit hit, _range, _obstructingLayer))
                vertices[i + 1] = verticeForward * hit.distance;
            else
                vertices[i + 1] = verticeForward * _range;
            currentAngle += angleIcrement;
        }

        for (int i = 0, j = 0; i < triangles.Length; i += 3, j++)
        {
            triangles[i] = 0;
            triangles[i + 1] = j + 1;
            triangles[i + 2] = j + 2;
        }
        _coneMesh.Clear();
        _coneMesh.vertices = vertices;
        _coneMesh.triangles = triangles;
        _meshFilter.mesh = _coneMesh;
    }
}