using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlotMinMaxTextView : MonoBehaviour {
    public TextMesh MinText;
    public TextMesh MaxText;
    public PlotChannelStatistics Stats;
    public PlotChannelLineView Channel;

    public int DecimalPoints = -1;

	void Update () {
        if (Stats)
        {
            Vector3 min = Stats.Minimum;
            Vector3 max = Stats.Maximum;

            var pMax = new Vector3(Channel.DataBounds.Max.x, max.y, 0);

            var pMin = new Vector3(Channel.DataBounds.Max.x, min.y, 0);


            if (DecimalPoints >= 0)
            {
                if(MaxText != null) {
                    MaxText.text = pMax.y.ToString(string.Format("F{0}", DecimalPoints));
                }
                if (MinText != null) {
                    MinText.text = pMin.y.ToString(string.Format("F{0}", DecimalPoints));
                }
            }
            else {
                if (MaxText != null) {
                    MaxText.text = pMax.y.ToString();
                }
                if (MinText != null) {
                    MinText.text = pMin.y.ToString();
                }
            }

            /*if (MaxText != null) {
                MaxText.transform.localPosition = Plot.RescaleSample(pMax);
            }
            if (MinText != null) {
                MinText.transform.localPosition = Plot.RescaleSample(pMin);
            }*/
        }
	}
}
