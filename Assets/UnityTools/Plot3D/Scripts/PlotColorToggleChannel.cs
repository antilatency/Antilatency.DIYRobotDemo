using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlotColorToggleChannel : PlotColorChannel {
    public Color[] SampleColors = new Color[] { Color.blue, new Color(0, 0.85f, 0, 1) };

    public override Color GetSample(int id) {
        return SampleColors[id % SampleColors.Length];
    }
}
