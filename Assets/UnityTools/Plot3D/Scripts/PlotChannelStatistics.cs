using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlotChannelStatistics : MonoBehaviour {
    public PlotChannel Channel;


    public Vector3 Minimum {
        get {
            UpdateStats();
            return min;
        }
    }

    public Vector3 Maximum {
        get {
            UpdateStats();
            return max;
        }
    }

    public Vector3 min;
    public Vector3 max;
    public Vector3 delta;

    private int lastUpdate = 0;

    void UpdateStats()
    {
        if (Channel && (Channel.Samples != null) && Channel.Samples.Count != 0 && lastUpdate != Channel.UpdateId)
        {
            lastUpdate = Channel.UpdateId;

            min = Channel.Samples.Read(0);
            max = min;

            for (int i = 1; i < Channel.Samples.Count; ++i)
            {
                var sample = Channel.Samples.Read(i);
                min = Vector3.Min(min, sample);
                max = Vector3.Max(max, sample);
            }

            delta = max - min;
        }
    }
}
