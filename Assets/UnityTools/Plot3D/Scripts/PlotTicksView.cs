using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(ProceduralMesh))]
public class PlotTicksView : MonoBehaviour
{
    public PlotGrid Grid;
    public float FontSize = 0.001f;
    public float LineSize = 0.02f;
    public Color TickColor = Color.white;
    public Color TextColor = Color.yellow;

    private void Reset() {
        var mr = GetComponent<MeshRenderer>();
        mr.sharedMaterial = (Material)Resources.FindObjectsOfTypeAll(typeof(Material)).FirstOrDefault(v => v.name == "Plot3D_LineMat");
    }

    private readonly LineListMeshConstructorExtended _constructor = new LineListMeshConstructorExtended();

    private LineFontMeshWriter _textWriter;


    private LineFontMeshWriter TextWriter {
        get {
            if (_textWriter == null) {
                _textWriter = new LineFontMeshWriter(_constructor);
            }
            return _textWriter;
        }
    }

    private readonly List<float> _tickValues = new List<float>();

    private void DrawTick(LineListMeshConstructorExtended mesh, Matrix4x4 dataToPlotTransform, Vector3 position, Vector3 normal, Vector3 textOffsetDir, int textAlignAxis, float value)
    {
        mesh.Color = TickColor;
        var tickStart = dataToPlotTransform.MultiplyPoint(position);
        var tickEnd = tickStart + (normal * LineSize);
        mesh.Line(tickStart, tickEnd);


        string text = value.ToString();

        Vector2 textLength = LineFontMeshWriter.TextSize(text);
        Vector3 textPivot = tickEnd + textOffsetDir * (textLength[textAlignAxis] * 1.5f * FontSize);

        mesh.Color = TextColor;
        mesh.Matrix =  Matrix4x4.Translate(textPivot) * Matrix4x4.Scale(Vector3.one * FontSize );
        TextWriter.WriteText(text);
        mesh.Matrix = Matrix4x4.identity;
    }

    public void OnValidate()
    {
        _constructor.Clear();
        _constructor.Color = Color.yellow;
        _constructor.Matrix = Matrix4x4.identity;

        var mesh = GetComponent<ProceduralMesh>();
        
        var plotTransform = Grid.GridToPlotTransform;
        var minBounds = Grid.DataBounds.Min;
        var maxBounds = Grid.DataBounds.Max;

        float gridCenter;

        Grid.GetGridValues(PlotGrid.GridPlane.Xz, 1, _tickValues, out gridCenter);
        foreach (var value in _tickValues) {
            DrawTick(_constructor, plotTransform, new Vector3(value, gridCenter, minBounds.z), Vector3.back, Vector3.down, 1,  value);
        }

        Grid.GetGridValues(PlotGrid.GridPlane.Yz, 0, _tickValues, out gridCenter);
        foreach (var value in _tickValues) {
            DrawTick(_constructor, plotTransform, new Vector3(gridCenter, value, minBounds.z), Vector3.back, Vector3.left, 0,  value);
        }

        Grid.GetGridValues(PlotGrid.GridPlane.Xz, 0, _tickValues, out gridCenter);
        foreach (var value in _tickValues) {
            DrawTick(_constructor, plotTransform, new Vector3(maxBounds.x, gridCenter, value), Vector3.right, Vector3.down, 1, value);
        }

        mesh.Rebuild(_constructor);
    }
}
