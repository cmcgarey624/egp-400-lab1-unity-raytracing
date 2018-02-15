using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayTraceReflectiveRenderer : RayTraceRenderer {
    [SerializeField] private Color _color;
    [SerializeField, Range(0, 1)] private float _reflectivity;
    const float reflectBias = 0.001f;

    public override Color CalculateColor(RaycastHit hitInfo, int RecursionDepth, float ambience, Ray originalRay)
    {
        // gets the normal
        Vector3 interpNormal = GetInterpNormal(hitInfo.barycentricCoordinate, _tris, _normals, hitInfo.triangleIndex);
        // calculates the light color
        Color lightAmt = CalculateLight(hitInfo.point, interpNormal, ambience);

        Vector3 reflect = Vector3.Reflect(originalRay.direction, interpNormal);
        Vector3 start = hitInfo.point + reflectBias * reflect;
        Color reflectColor = Bounce(new Ray(start, reflect), RecursionDepth, ambience);

        Color col = _color * lightAmt + reflectColor * _reflectivity;
        

        col.a = 1.0f;
        return col;
    }

    protected Color Bounce(Ray inRay, int depth, float ambience)
    {
        if (depth > 0)
        {
            RaycastHit outHit;
            bool hit = Physics.Raycast(inRay, out outHit);
            if (hit)
            {
                RayTraceRenderer renderer = outHit.transform.gameObject.GetComponent<RayTraceRenderer>();
                if (renderer)
                    return renderer.CalculateColor(outHit, --depth, ambience, inRay);
            }
            
        }


        return Color.clear;
    }
}
