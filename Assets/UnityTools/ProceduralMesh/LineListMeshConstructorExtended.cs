using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineListMeshConstructorExtended : LineListMeshConstructor {

    public Matrix4x4 Matrix = Matrix4x4.identity;

    public override void Clear() {
        base.Clear();
        Matrix = Matrix4x4.identity;
    }

    public override void Vertex(Vector3 vertex) {
        base.Vertex(Matrix.MultiplyPoint(vertex));
    }

    public override void Vertex(Vector3 vertex, Color color) {
        base.Vertex(Matrix.MultiplyPoint(vertex), color);
    }
}
