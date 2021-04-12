using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

public class TriangleMeshConstructor : IMeshConstructor {
    public List<Vector3> Vertices { get; set; } = new List<Vector3>();

    public int[] Indices { get; set; } 

    public List<Color> Colors { get; set; } = new List<Color>();

    public MeshTopology Topology { get { return MeshTopology.Triangles; } }

    public Color Color { get; set; } = Color.white;

    public List<Vector3> Normals { get; set; } = null;

    public void RecalculateIndicesAsTriangleList() {
        if(Vertices == null) {
            return;
        }
        if(Vertices.Count % 3 != 0) {
            throw new System.Exception("Vertex count is not multiple of 3");
        }
        Indices = Enumerable.Range(0, Vertices.Count).ToArray();
    }

    public void BreakVertices()
    {
        List<Vector3> newVertices = new List<Vector3>();
        List<Color> newColors = new List<Color>();

        for (int i = 0; i < Indices.Length; ++i)
        {
            newVertices.Add(Vertices[Indices[i]]);
            newColors.Add(Colors[Indices[i]]);
        }
       

        Vertices = newVertices;
        Colors = newColors;

        RecalculateIndicesAsTriangleList();
    }

    public void RecalculateFlatNormals()
    {
        Normals = new List<Vector3>();
        //norma
        for (int i = 0; i < Vertices.Count; i+=3)
        {
            var p0 = Vertices[i + 0];
            var p1 = Vertices[i + 1];
            var p2 = Vertices[i + 2];

            var v0 = p0 - p2;
            var v1 = p1 - p2;

            var normal = Vector3.Cross(v0.normalized, v1.normalized).normalized;
            Normals.Add(normal);
            Normals.Add(normal);
            Normals.Add(normal);
        }
    }

    public void AssingColors(Color c) {
        Colors = Enumerable.Repeat(c, Vertices.Count).ToList();
    }

    public delegate Color ColorGenerator(int verticesCount, int vertexId);
    public delegate Color ColorGenerator2(int verticesCount, int vertexId, Vector3 vertex);

    public void AssingColors(ColorGenerator generator) {
        Colors = new List<Color>();
        for (int i = 0; i < Vertices.Count; ++i) {
            Colors.Add(generator(Vertices.Count, i));
        }
    }

    public void AssingColors(ColorGenerator2 generator) {
        Colors = new List<Color>();
        for (int i = 0; i < Vertices.Count; ++i) {
            Colors.Add(generator(Vertices.Count, i, Vertices[i]));
        }
    }

    public void Vertex(Vector3 v, Color c) {
        Vertices.Add(v);
        Colors.Add(c);
    }

    public void Vertex(Vector3 v) {
        Vertices.Add(v);
        Colors.Add(Color);
    }

    public void Triangle(Vector3 p0, Vector3 p1, Vector3 p2) {
        Vertex(p0);
        Vertex(p1);
        Vertex(p2);
    }
}
