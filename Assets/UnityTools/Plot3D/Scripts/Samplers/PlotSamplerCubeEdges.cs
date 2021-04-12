using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlotSamplerCubeEdges : MonoBehaviour {
    public PlotChannel Channel;
    public Vector3 CubeSize = Vector3.one;

    void Start() {
        if (!gameObject.activeInHierarchy) {
            return;
        }

        Vector3 e = CubeSize / 2.0f;
        Vector3[] corners = new Vector3[] {
            new Vector3(-e.x, e.y, e.z),
            new Vector3(e.x, e.y, e.z),
            new Vector3(e.x, e.y, -e.z),
            new Vector3(-e.x, e.y, -e.z),

            new Vector3(-e.x, -e.y, e.z),
            new Vector3(e.x, -e.y, e.z),
            new Vector3(e.x, -e.y, -e.z),
            new Vector3(-e.x, -e.y, -e.z),
        };

        var indices = new int[] {
            0,1,2,3,0,
            4,5,6,7,4,
            1,5,2,6,3,7,0
        };

        for (int i = 0; i < indices.Length; ++i) {
            Channel.AddSample(corners[indices[i]]);
        }

    }
}
