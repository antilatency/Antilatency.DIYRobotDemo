using UnityEngine;

public class PlotSamplerDataset : MonoBehaviour {
    public PlotChannel Channel;
    public Vector3[] Samples;

    private void Reset() {
        PlotUtils.AutoAssignPlotChannel(this, ref Channel);
    }

    void Start() {
        if (gameObject.activeInHierarchy && Channel != null) {
            if (Samples == null || Samples.Length == 0) {
                Channel.ResetSamples();
            } else {
                for (int i = 0; i < Samples.Length; ++i) {
                    Channel.AddSample(Samples[i]);
                }
            }
        }
    }
}
