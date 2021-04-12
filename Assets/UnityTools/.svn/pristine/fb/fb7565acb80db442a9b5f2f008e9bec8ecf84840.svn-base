using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationMeshFilter : ProceduralMeshFilter
{
    public Quaternion Rotation = Quaternion.identity;
    public RotationMeshFilter(IProceduralMeshFilter next) : base(next)
    {
    }

    public RotationMeshFilter(IProceduralMeshFilter next, Quaternion rotation) : base(next)
    {
        Rotation = rotation;
    }

    public override void Vertex(Vector3 vertex)
    {
        base.Vertex(Rotation * vertex);
    }
}
