using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlotSamplerSineWave : MonoBehaviour {
    public PlotChannel Channel;
    private float startTime;

    void Start() {
        startTime = Time.realtimeSinceStartup;
    }

    void Update() {
        if (Channel != null) {
            float t = Time.realtimeSinceStartup - startTime;
            Channel.AddSample(new Vector3(t, Mathf.Sin(t), Mathf.Cos(t)));
        }
    }
}
