using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleMeshFilter : ProceduralMeshFilter
{
    public Vector3 Scale = Vector3.one;
    public ScaleMeshFilter(IProceduralMeshFilter next, Vector3 scale) : base(next)
    {
        Scale = scale;
    }

    public override void Vertex(Vector3 vertex)
    {
        vertex.x *= Scale.x;
        vertex.y *= Scale.y;
        vertex.z *= Scale.z;

        base.Vertex(vertex);
    }
}
