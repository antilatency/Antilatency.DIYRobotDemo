using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineStripMeshWriter {
    public LineListMeshConstructor Constructor { get; private set; }
    private Vector3 _lastVertex = Vector3.zero;
    private bool _newStrip = true;

    public Vector3 LastVertex {
        get { return _lastVertex; }
    }

    public int StripLength { get; private set; }
    public LineStripMeshWriter(LineListMeshConstructor constructor) {
        Constructor = constructor;
    }

    public void Vertex(Vector3 value) {
        if (_newStrip) {
            _newStrip = false;
        } else {
            Constructor.Line(_lastVertex, value);
        }
        _lastVertex = value;
        StripLength++;
    }

    public void Restart() {
        _newStrip = true;
        StripLength = 0;
    }
}
