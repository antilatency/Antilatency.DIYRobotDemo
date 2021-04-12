using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlotChannel : MonoBehaviour {


    private CircularBuffer<Vector3> _samples = new CircularBuffer<Vector3>();

    /// <summary>
    /// Max samples count in channel. Set 0 to infinite capacity.
    /// </summary>
    public uint Capacity = 100;
    public int UpdateId = 0;

    public void AddSamples(IEnumerable<Vector3> samples, bool lineList = false) {
        if(lineList) {
            var s = samples.ToArray();
            for (int i = 0; i < s.Length - 1; ++i) {
                AddSample(s[i]);
                AddSample(s[i + 1]);
            }
        } else {
            foreach (var sample in samples) {
                AddSample(sample);
            }
        }
       
    }

   

    public CircularBuffer<Vector3> Samples {
        get {
            return _samples;
        }
    }

    public event System.Action Reset;

    public void ResetSamples() {
        _samples.Resize(0);

        if(Reset != null) {
            Reset();
        }
    }

    public virtual void AddSample(Vector3 sample){
        if (Capacity > int.MaxValue) {
            Capacity = (uint)int.MaxValue;
        }

        if(Capacity == 0) {
            _samples.Resize(_samples.Count + 1);
        } else {
            _samples.Resize((int)Capacity);
        }
       
        _samples.Write(sample);
        UpdateId++;
    }
}
