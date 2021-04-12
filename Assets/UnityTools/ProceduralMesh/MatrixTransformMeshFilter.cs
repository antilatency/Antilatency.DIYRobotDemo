using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatrixTransformMeshFilter : ProceduralMeshFilter
{
    public Matrix4x4 Matrix = Matrix4x4.identity;
    public MatrixTransformMeshFilter(IProceduralMeshFilter next) : base(next)
    {
    }
    public MatrixTransformMeshFilter(IProceduralMeshFilter next, Matrix4x4 matrix) : base(next)
    {
        Matrix = matrix;
    }

    public override void Vertex(Vector3 vertex)
    {
        base.Vertex(Matrix.MultiplyPoint(vertex));
    }
}
