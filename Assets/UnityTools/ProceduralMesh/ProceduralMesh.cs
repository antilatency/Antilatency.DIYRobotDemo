using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
[ExecuteInEditMode]
public class ProceduralMesh : MonoBehaviour {

    private Mesh _mesh;
    private void Awake() {
        
    }

    private void ValidateMeshCreated() {
        if(_mesh == null) {
            _mesh = new Mesh();
            var filter = GetComponent<MeshFilter>();
            filter.sharedMesh = _mesh;
        }
    }

    public void Rebuild(IMeshConstructor constructor, bool recalculateBounds = true) {
        ValidateMeshCreated();
        _mesh.Clear();
        _mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        _mesh.SetVertices(constructor.Vertices);
        _mesh.SetColors(constructor.Colors);
        _mesh.SetIndices(constructor.Indices, constructor.Topology, 0);

        var normals = constructor.Normals;
        if (normals != null)
        {
            _mesh.SetNormals(normals);
        }

        if (recalculateBounds) {
            _mesh.RecalculateBounds();
        }
    }
}
