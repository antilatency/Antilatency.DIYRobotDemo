using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathDrawer : MonoBehaviour {
    public uint PointCount = 100;
    public Color LineColor = Color.green;

    public bool DrawGizmos = true;
    public bool DrawGL = true;

    private CircularBuffer<Vector3> _points = new CircularBuffer<Vector3>();
    private static Material _lineMaterial;

    public void AppPoint(Vector3 v) {
        if (PointCount > int.MaxValue) {
            PointCount = (uint)int.MaxValue;
        }

        _points.Resize((int)PointCount);
        _points.Write(v);

        //Debug.Log(gameObject.name + ": " + _points.Count);
    }

    public void Render(bool gizmo) {
        for (int i = 0; i < _points.Count - 1; ++i) {
            var p0 = _points.Read(i);
            var p1 = _points.Read(i + 1);

            if (gizmo) {
                Gizmos.color = LineColor;
                Gizmos.DrawLine(p0, p1);
            } else {
                _lineMaterial.SetPass(0);

                GL.PushMatrix();
                GL.Begin(GL.LINES);
                GL.Color(LineColor);
                GL.Vertex(p0);
                GL.Vertex(p1);
                GL.End();
                GL.PopMatrix();
            }
        }
    }

    private void Start() {
        if (DrawGL) {
            StartCoroutine(GameViewRender());

            if (_lineMaterial != null) { return; }

            var shader = Shader.Find("Hidden/Internal-Colored");
            _lineMaterial = new Material(shader);
            _lineMaterial.hideFlags = HideFlags.HideAndDontSave;

            //_lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            //_lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);

            _lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            _lineMaterial.SetInt("_ZWrite", 0);
        }
    }

    private IEnumerator GameViewRender() {
        while (true) {
            yield return new WaitForEndOfFrame();
            Render(false);
        }
    }

    private void OnDrawGizmos() {
        if (!DrawGizmos) { return; }

        Render(true);
    }

    //private void Update() {
    //    AppPoint(transform.position);
    //}
}
