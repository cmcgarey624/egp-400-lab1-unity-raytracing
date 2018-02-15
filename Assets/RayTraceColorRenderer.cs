using System.Collections.Generic;
using UnityEngine;

public class RayTraceColorRenderer : RayTraceRenderer
{
    [SerializeField] private Color _color;
    

    public override Color CalculateColor(RaycastHit hitInfo, int RecursionDepth, float ambience, Ray originalRay)
    {
        // gets the normal
        Vector3 interpNormal = GetInterpNormal(hitInfo.barycentricCoordinate, _tris, _normals, hitInfo.triangleIndex);
        // calculates the light color
        Color lightAmt = CalculateLight(hitInfo.point, interpNormal, ambience);
        Color col = _color * lightAmt;

        col.a = 1.0f;
        return col;
    }
}
