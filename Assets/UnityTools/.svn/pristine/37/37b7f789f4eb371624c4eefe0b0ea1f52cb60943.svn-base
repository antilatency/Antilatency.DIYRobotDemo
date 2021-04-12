using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(ProceduralMesh))]
public class PlotGrid : MonoBehaviour
{
    public Plot3D Plot;
    public Bounds DataBounds;
    public Vector3 GridStep = Vector3.zero;
    public float X = 0.0f;

    public Color GridColor = Color.gray;

    public enum PlanePosition
    {
        Min,
        Center,
        Max
    }

    public UnityEvent Validate = new UnityEvent();

    public PlanePosition XyPlanePosition = PlanePosition.Max;
    public PlanePosition XzPlanePosition = PlanePosition.Min;
    public PlanePosition YzPlanePosition = PlanePosition.Min;

    public int MaxLinesCount = 10000;

    private int[][] gridIds = new int[][]
    {
        new int[]{0,1, 2},
        new int[]{2,0, 1},
        new int[]{1,2, 0}

    };

    private PlanePosition GetPlanePosition (int id )
    {
        if (id == 0)
        {
            return XyPlanePosition;
        }else if (id == 1) {
            return XzPlanePosition;
        } else {
            return YzPlanePosition;
        }
    }


    [Flags]
    public enum GridPlane {
        Xy = 1,
        Xz = 2,
        Yz = 4
    }

    public GridPlane DrawGrids = GridPlane.Xy;

    void Start()
    {
        OnValidate();
    }

    public void GetGridValues(GridPlane plane, int majorAxisId, List<float> result, out float center)
    {
        result.Clear();
        int[] gridSetup = gridIds[(int)plane / 2];

        int mainId = gridSetup[(0 + majorAxisId) % 2];
        var boundsMin = DataBounds.Min;
        var boundsMax = DataBounds.Max;
        var boundsCenter = (boundsMin + boundsMax) / 2;

        int centerId = gridSetup[2];
        var planePosition = GetPlanePosition((int)plane / 2);

        center = boundsCenter[centerId];
        if (planePosition == PlanePosition.Min) {
            center = boundsMin[centerId];

        } else if (planePosition == PlanePosition.Max) {
            center = boundsMax[centerId];
        }


        if (GridStep[mainId] <= 0) {
            return;
        }

     

        float minVal = boundsMin[mainId];
        float maxVal = boundsMax[mainId];

        if (maxVal < minVal) {
            var tmp = maxVal;
            maxVal = minVal;
            minVal = tmp;
        }

        var startLineId = (int)(minVal / GridStep[mainId]);
        if ((startLineId * GridStep[mainId]) < minVal) {
            ++startLineId;
        }

        int id = 0;
        while (true) {
            if (id >= MaxLinesCount) {
                break;
            }
            ++id;
            float lineValue = startLineId * GridStep[mainId];
            if (lineValue <= maxVal) {
                result.Add(lineValue);
            } else {
                break;
            }
            startLineId++;
        }
    }

    private void Reset() {
        var mr = GetComponent<MeshRenderer>();
        mr.sharedMaterial = (Material)Resources.FindObjectsOfTypeAll(typeof(Material)).FirstOrDefault(v => v.name == "Plot3D_LineMat");
    }

    public void OnValidate() {
        for (int i = 0; i < 3; ++i) {
            if (GridStep[i] < 0) {
                GridStep[i] = 0;
            }
        }
        RebuildMesh();
        Validate.Invoke();
    }

    public Matrix4x4 GridToPlotTransform
    {
        get
        {
            var boundsMin = DataBounds.Min;
            var boundsMax = DataBounds.Max;
            var boundsSize = boundsMax - boundsMin;
            var boundsCenter = (boundsMax + boundsMin) / 2;
            Matrix4x4 result = Matrix4x4.identity;

            result *= Matrix4x4.Scale(
                new Vector3(
                    Plot.PlotSize.x / boundsSize.x,
                    Plot.PlotSize.y / boundsSize.y,
                    Plot.PlotSize.z / boundsSize.z
                )
            );

            result *= Matrix4x4.Translate(-boundsCenter);
            return result;
        }
    }

    void RebuildMesh() {
        var mesh = GetComponent<ProceduralMesh>();
        var constructor = new LineListMeshConstructorExtended();

        if((Plot != null) && (DataBounds != null) && GridStep != Vector3.zero) {
            constructor.Matrix = Matrix4x4.Scale(Plot.PlotSize);
            var boundsMin = DataBounds.Min;
            var boundsMax = DataBounds.Max;
            var boundsCenter = (boundsMax + boundsMin) / 2;

            constructor.Matrix = GridToPlotTransform;
            constructor.Color = GridColor;

            for (int g = 0; g < 3; ++g)
            {
                if((int)(DrawGrids & (GridPlane)(1 << g)) == 0)
                {
                    continue;
                }

                int[] gridSetup = gridIds[g];
                int centerId = gridSetup[2];


                for (int dir = 0; dir < 2; ++dir)
                {
                    int mainId = gridSetup[(0 + dir) % 2];
                    if (GridStep[mainId] <= 0) {
                        continue;
                    }
                    List<float> gridValues = new List<float>();
                    float gridCenter;
                    GetGridValues((GridPlane) (1 << g), dir, gridValues, out gridCenter);

                    int secondaryId = gridSetup[(1 + dir) % 2];

                    for (int l = 0; l < gridValues.Count; ++l)
                    {
                        var min = new Vector3();
                        var max = new Vector3();

                        min[secondaryId] = boundsMin[secondaryId];
                        max[secondaryId] = boundsMax[secondaryId];

                        min[mainId] = gridValues[l];
                        max[mainId] = gridValues[l];

                        min[centerId] = gridCenter;
                        max[centerId] = gridCenter;
                        constructor.Line(min, max);
                    }
                }
            }
        }

        mesh.Rebuild(constructor);
    }

    void Update()
    {
        
    }

}
