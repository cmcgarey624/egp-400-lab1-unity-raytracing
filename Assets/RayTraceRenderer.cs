using System.Collections.Generic;
using UnityEngine;

public abstract class RayTraceRenderer : MonoBehaviour
{
    private Light _worldLight;
    private Transform _lightTransform;
    protected Mesh _mesh;
    protected List<Vector3> _normals;
    protected List<int> _tris;

    protected virtual void Awake()
    {
        _mesh = GetComponent<MeshCollider>().sharedMesh;

        _normals = new List<Vector3>();
        _tris = new List<int>();
        _mesh.GetNormals(_normals);
        _mesh.GetTriangles(_tris, 0);
        _worldLight = FindObjectOfType<Light>();
        _lightTransform = _worldLight.transform;
    }

    protected Color CalculateLight(Vector3 hitPoint, Vector3 norm, float ambience)
    {
        Color c = CalculateLightPoint(hitPoint, norm, ambience);
        c.a = 1.0f;
        return c;
    }

    protected Color CalculateLightPoint(Vector3 hitPoint, Vector3 norm, float ambience)
    {
        // distance to light
        Vector3 toLight = _lightTransform.position - hitPoint;
        bool inShadow = Physics.Raycast(hitPoint, toLight, toLight.magnitude);
        // checks if the point is in shadow or in light
        if (inShadow)
            return Color.white * ambience; // if in shadow return the ambience

        // get the intensity of the light
        // range /(4*dist^2)
        float intensity = _worldLight.range * 0.25f / (hitPoint - _lightTransform.position).sqrMagnitude;
        intensity = Mathf.Max(intensity, 0.0f);
        float dot = Mathf.Max(Vector3.Dot(toLight.normalized, norm), 0.0f);
        return _worldLight.color * (dot * intensity  + ambience);
    }

    protected Color CalculateLightDirectional(Vector3 hitPoint, Vector3 norm, float ambience)
    {
        // if the light is directional
        Vector3 toLight = _worldLight.transform.forward;
        var inShadow = Physics.Raycast(hitPoint, -toLight);
        if (inShadow)
            return Color.white * ambience;

        return _worldLight.color * Vector3.Dot(-toLight, norm);
    }

    protected Vector3 GetInterpNormal(Vector3 bary, List<int> tris, List<Vector3> normals, int tri)
    {
        // gets a normal of the barycentric coordinate
        Vector3 n0 = normals[tris[tri * 3 + 0]];
        Vector3 n1 = normals[tris[tri * 3 + 1]];
        Vector3 n2 = normals[tris[tri * 3 + 2]];
        return (n0 * bary.x + n1 * bary.y + n2 * bary.z).normalized;
    }

    public abstract Color CalculateColor(RaycastHit hitInfo, int RecursionDepth, float ambience, Ray originalRay);
}
