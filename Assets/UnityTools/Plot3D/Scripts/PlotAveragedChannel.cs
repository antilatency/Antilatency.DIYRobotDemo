using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlotAveragedChannel : PlotChannel {
    public int AverageSamplesCount = 32;

    private CircularBuffer<Vector3> _averageBuffer = new CircularBuffer<Vector3>();

    public Vector3 Average;

    public override void AddSample(Vector3 sample) {
        _averageBuffer.Resize(AverageSamplesCount);
        _averageBuffer.Write(sample);

        Vector3 result = Vector3.zero;

        var averageCount = (AverageSamplesCount < Samples.Count ? AverageSamplesCount : Samples.Count);

        for (int i = 0; i < averageCount; ++i) {
            result += _averageBuffer.Read(i);
        }
        result /= (float)AverageSamplesCount;
        Average = result;
        base.AddSample(result);
    }
}
