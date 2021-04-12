using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffsetMeshFilter : ProceduralMeshFilter
{
    public Vector3 Offset = Vector3.zero;
    public OffsetMeshFilter(IProceduralMeshFilter next) : base(next)
    {
    }
    public OffsetMeshFilter(IProceduralMeshFilter next, Vector3 offset) : base(next)
    {
        Offset = offset;
    }
    public override void Vertex(Vector3 vertex)
    {
        base.Vertex(vertex + Offset);
    }
}
