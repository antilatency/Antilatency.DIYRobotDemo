using UnityEngine;
using UnityEngine.Events;

public class PlotChannelView : MonoBehaviour {

    public Plot3D Plot;
    public PlotChannel Channel;

    public Bounds DataBounds;

    public UnityEvent Validate = new UnityEvent();

    public enum OffsetMode {
        None,
        ToZero,
        ToRight
    }

    public OffsetMode Offset = OffsetMode.ToRight;

    protected virtual void Reset() {
        PlotUtils.AutoAssignPlot(this, ref Plot);
        PlotUtils.AutoAssignPlotChannel(this, ref Channel);
    }

    public virtual void OnValidate() {
        Validate.Invoke();

    }

}
