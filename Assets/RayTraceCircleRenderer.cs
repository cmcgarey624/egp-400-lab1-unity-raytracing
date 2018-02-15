using System.Collections.Generic;
using UnityEngine;

public class RayTraceCircleRenderer : RayTraceRenderer
{
    private Vector2 _circleCenter;
    private Mesh _mesh;

    private List<Vector3> _normals;
    private List<int> _tris;

    protected override void Awake()
    {
        base.Awake();
        _circleCenter = Vector2.one * 0.5f;
    }

    public override Color CalculateColor(RaycastHit hitInfo, int RecursionDepth, float ambience, Ray originalRay)
    {
        Vector3 interpNormal = GetInterpNormal(hitInfo.barycentricCoordinate, _tris, _normals, hitInfo.triangleIndex);
        Color lightAmt = CalculateLight(hitInfo.point, interpNormal, ambience);
        Color fragColor = Color.white * lightAmt;

        float rad = 0.125f;
        if ((hitInfo.textureCoord - _circleCenter).sqrMagnitude < rad)
            fragColor = Color.red * lightAmt;

        fragColor.a = 1.0f;

        return fragColor;
    }
}
