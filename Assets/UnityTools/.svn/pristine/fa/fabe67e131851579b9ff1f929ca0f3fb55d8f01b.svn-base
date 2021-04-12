using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;



[RequireComponent(typeof(ProceduralMesh))]
public class Plot3D : MonoBehaviour {
    public Vector3 PlotSize = new Vector3(1, 1, 1);

    [System.Flags]
    public enum AxisFlags
    {
        X = 1, Y = 2, Z = 4
    }

    public int GridXSteps = 1;
    public int GridYSteps = 1;
    public int GridZSteps = 1;

    public AxisFlags DrawAxis = AxisFlags.X | AxisFlags.Y | AxisFlags.Z;
    public bool DrawViewBounds = true;


    public Vector3 GridOffset = Vector3.zero;
    public UnityEvent Validate = new UnityEvent();

    private void Awake() {
        RebuildGraphics();
    }

    private void Reset() {
        var mr = GetComponent<MeshRenderer>();
        mr.sharedMaterial = (Material)Resources.FindObjectsOfTypeAll(typeof(Material)).FirstOrDefault(v => v.name == "Plot3D_LineMat");
    }

    private void RebuildGraphics() {
        var mesh = GetComponent<ProceduralMesh>();
        var constructor = new LineListMeshConstructorExtended();

        constructor.Matrix = Matrix4x4.Scale(PlotSize);
        if (DrawViewBounds) {
            constructor.Color = Color.blue;
            constructor.Box(Vector3.one);
        }

        if ((DrawAxis & AxisFlags.X) == AxisFlags.X) {
            constructor.Color = Color.red;
            constructor.Line(new Vector3(-0.5f, 0, 0), new Vector3(0.5f, 0, 0));
        }

        if ((DrawAxis & AxisFlags.Y) == AxisFlags.Y) {
            constructor.Color = Color.green;
            constructor.Line(new Vector3(0, -0.5f, 0), new Vector3(0, 0.5f, 0));
        }

        if ((DrawAxis & AxisFlags.Z) == AxisFlags.Z) {
            constructor.Color = Color.blue;
            constructor.Line(new Vector3(0, 0, -0.5f), new Vector3(0, 0, 0.5f));
        }

        mesh.Rebuild(constructor);
    }
    private void OnValidate() {
        RebuildGraphics();
        Validate.Invoke();
    }

}
