using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Linq;
using UnityEngine;

public class LineListMeshConstructor : IMeshConstructor, IProceduralMeshFilter {

    private List<Vector3> _vertices = new List<Vector3>();
    private List<Color> _colors = new List<Color>();
    private int[] _indices = new int[] { };

    public List<Vector3> Vertices => _vertices;
    public int[] Indices {
        get {
            RebuildIndexBuffer();
            return _indices;
        }
    }
    public List<Color> Colors => _colors;

    public Color Color { get; set; } = Color.red;
    public uint SegmentsCount { get; set; } = 16;

    public MeshTopology Topology {
        get {
            return MeshTopology.Lines;
        }
    }

    public List<Vector3> Normals { get; set; } = null;

    public static T Concat<T>(params T[] contructors) where T : LineListMeshConstructor {
        LineListMeshConstructor result = new LineListMeshConstructor();
        result._colors = contructors.SelectMany(v => v._colors).ToList();
        result._vertices = contructors.SelectMany(v => v._vertices).ToList();
        return (T)result;
    }


    public virtual void Clear() {
        _vertices.Clear();
        _colors.Clear();
    }

    public void Line(Vector3 begin, Vector3 end) {
        Vertex(begin);
        Vertex(end);
    }

    public void Line(Vector3 begin, Color beginColor, Vector3 end, Color endColor) {
        Vertex(begin, beginColor);
        Vertex(end, endColor);
    }

    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual void Vertex(Vector3 vertex) {
        _vertices.Add(vertex);
        _colors.Add(Color);
    }

    public virtual void Vertex(Vector3 vertex, Color color) {
        _vertices.Add(vertex);
        _colors.Add(color);
    }

    public void Circle(Vector3 position, float radius) {
        Vector3 first = position;
        first.y += radius;
        Vertex(first);
        float stepSize = (3.14159265358979f * 2) / SegmentsCount;
        for (int i = 1; i < SegmentsCount; i++) {
            float angle = stepSize * i;
            var v = new Vector3(Mathf.Sin(angle) * radius, Mathf.Cos(angle) * radius, 0) + position;
            Vertex(v);
            Vertex(v);
        }
        Vertex(first);
    }

    public void Grid(float width, float height, int widthSegments, int heightSegments) {
        if(widthSegments < 1) {
            widthSegments = 1;
        }
        if (heightSegments < 1) {
            heightSegments = 1;
        }

        float halfWidth = width / 2.0f;
        float halfHeight = height / 2.0f;

        //Vertical lines
        for(int i = 0; i <= widthSegments; ++i) {
            float x = width / widthSegments * i - halfWidth;
            Vertex(new Vector3(x, height / 2, 0));
            Vertex(new Vector3(x, -height / 2, 0));
        }

        //Horizontal lines
        for (int i = 0; i <= heightSegments; ++i) {
            float y = height / heightSegments * i - halfHeight;
            Vertex(new Vector3(-halfWidth, y, 0));
            Vertex(new Vector3(halfWidth, y, 0));
        }
    }

    public void Box(Vector2 size) {
        float halfX = size.x / 2;
        float halfY = size.y / 2;
        Line(new Vector3(-halfX, halfY), new Vector3(halfX, halfY));
        Line(new Vector3(halfX, halfY), new Vector3(halfX, -halfY));
        Line(new Vector3(halfX, -halfY), new Vector3(-halfX,-halfY));
        Line(new Vector3(-halfX, -halfY), new Vector3(-halfX, halfY));
    }

    public void Box(Vector3 size) {
        float halfX = size.x / 2;
        float halfY = size.y / 2;
        float halfZ = size.z / 2;

        Vector3[] vertices = new Vector3[] {
            new Vector3(-halfX, halfY, halfZ),
            new Vector3(halfX, halfY, halfZ),
            new Vector3(halfX, halfY, -halfZ),
            new Vector3(-halfX, halfY, -halfZ),

            new Vector3(-halfX, -halfY, halfZ),
            new Vector3(halfX, -halfY, halfZ),
            new Vector3(halfX, -halfY, -halfZ),
            new Vector3(-halfX, -halfY, -halfZ),
        };

        int[] indices = new int[] {
            0,1,
            1,2,
            2,3,
            3,0,

            4,5,
            5,6,
            6,7,
            7,4,

            0,4,
            1,5,
            2,6,
            3,7
        };

        for(int i = 0;  i < indices.Length; ++i) {
            Vertex(vertices[indices[i]]);
        }
    }

    public void CoordinateSystem() {
        var oldColor = Color;
        Color = Color.red;
        Line(Vector3.zero, Vector3.right);
        Color = Color.green;
        Line(Vector3.zero, Vector3.up);
        Color = Color.blue;
        Line(Vector3.zero, Vector3.forward);
    }

    public void Cross2D(float extent) {
        Line(new Vector3(-extent, 0, 0), new Vector3(extent, 0, 0));
        Line(new Vector3(0, -extent, 0), new Vector3(0, extent, 0));
    }

    public void Cross3D(float extent) {
        Line(new Vector3(-extent, 0, 0), new Vector3(extent, 0, 0));
        Line(new Vector3(0, -extent, 0), new Vector3(0, extent, 0));
        Line(new Vector3(0, 0, -extent), new Vector3(0, 0, extent));
    }

    public void QuadraticBezier(Vector3 p0, Vector3 p1, Vector3 p2) {
        Vector3 previous = p0;
        for (int i = 1; i <= SegmentsCount; ++i) {
            float t = 1.0f / SegmentsCount * i;
            float rcpT = 1.0f - t;

            float tPow2 = t * t;
            float rcpTPow2 = rcpT * rcpT;

            Vector3 b = rcpTPow2 * p0 + 2.0f * rcpT * t * p1 + tPow2 * p2;
            Vertex(previous);
            Vertex(b);
            previous = b;
        }
    }

    public void QubicBezier(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3) {
        Vector3 previous = p0;
        for (int i = 1; i <= SegmentsCount; ++i) {
            float t = 1.0f / SegmentsCount * i;
            float rcpT = 1.0f - t;
            float tPow2 = t * t;
            float tPow3 = tPow2 * t;
            float rcpTPow2 = rcpT * rcpT;
            float rcpTPow3 = rcpTPow2 * rcpT;
            Vector3 b = rcpTPow3 * p0 + 3 * rcpTPow2 * t * p1 + 3 * rcpT * tPow2 * p2 + tPow3 * p3;
            Vertex(previous);
            Vertex(b);
            previous = b;
        }
    }

    int[] PascalTriangleRow(int row) {
        int[] result = new int[row + 1];
        result[0] = 1;
        for (int i = 0; i < row; ++i)
            result[1 + i] = result[i] * (row - i) / (i + 1);
        return result;
    }

    public void Bezier(Vector3[] controlPoints) {
        if(controlPoints == null || controlPoints.Length < 2) {
            return;
        }

        Vector3 previous = controlPoints[0];
        int n = controlPoints.Length - 1;

        var row = PascalTriangleRow(n);

        for (int s = 1; s <= SegmentsCount; ++s) {
            float t = 1.0f / SegmentsCount * s;
            float rcpT = 1.0f - t;
            Vector3 b = Vector3.zero;
            for (int i = 0; i < controlPoints.Length; ++i) {
                b += Mathf.Pow(rcpT, n - i) * Mathf.Pow(t, i) * controlPoints[i] * row[i];
            }
            Vertex(previous);
            Vertex(b);
            previous = b;
        }
    }

    public void PolyLine(IEnumerable<Vector3> pointsList) {
        using(var enumerator = pointsList.GetEnumerator()) {
            if (!enumerator.MoveNext()) {
                return;
            }
            Vector3 previous = enumerator.Current;
            while (enumerator.MoveNext()) {
                var current = enumerator.Current;
                Vertex(previous);
                Vertex(current);
                previous = current;
            }
        }
    }

    public void Sphere(Vector3 position, float radius) {
        Vector3[] first = new Vector3[] { position, position, position };
        first[0].x += radius;
        first[1].y += radius;
        first[2].y += radius;

        Vector3[] previous = new Vector3[] { first[0], first[1], first[2] };

        float stepSize = (3.14159265358979f * 2) / SegmentsCount;
        for (int i = 1; i < SegmentsCount; i++) {
            float angle = stepSize * i;
            float a = Mathf.Sin(angle) * radius;
            float b = Mathf.Cos(angle) * radius;

            var v = new Vector3(b, 0, -a) + position;
            Vertex(previous[0]);
            Vertex(v);
            previous[0] = v;

            v = new Vector3(a, b, 0) + position;
            Vertex(previous[1]);
            Vertex(v);
            previous[1] = v;

            v = new Vector3(0, b, a) + position;
            Vertex(previous[2]);
            Vertex(v);
            previous[2] = v;
        }

        for (int i = 0; i < 3; ++i) {
            Vertex(previous[i]);
            Vertex(first[i]);
        }
    }

    private void RebuildIndexBuffer() {
        if (_indices.Length > _vertices.Count) {
            System.Array.Resize(ref _indices, _vertices.Count);
        } else if (_indices.Length < _vertices.Count) {
            int beginIndex = _indices.Length;
            int endIndex = _vertices.Count;
            System.Array.Resize(ref _indices, _vertices.Count);
            for (; beginIndex < endIndex; ++beginIndex) {
                _indices[beginIndex] = beginIndex;
            }
        }
    }
}
