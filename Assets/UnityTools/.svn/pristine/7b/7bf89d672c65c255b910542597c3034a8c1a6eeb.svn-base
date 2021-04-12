using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProceduralMeshFilter
{
    void Vertex(Vector3 vertex);
}

public class ProceduralMeshFilter : IProceduralMeshFilter {
    public IProceduralMeshFilter Next { get; }
    public ProceduralMeshFilter(IProceduralMeshFilter next)
    {
        Next = next;
    }
    public virtual void Vertex(Vector3 vertex)
    {
        Next.Vertex(vertex);
    }
}
