using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class PlotUtils {

    public static GameObject CreateGameObject(GameObject parent, string name) {
        var result = new GameObject(name);
        result.transform.parent = parent.transform;
        result.transform.localPosition = Vector3.zero;
        result.transform.localRotation = Quaternion.identity;
        result.transform.localScale = Vector3.one;
        return result;
    }

    public static void AutoAssignPlotChannel(Component owner, ref PlotChannel target) {
        var channels = owner.gameObject.GetComponents<PlotChannel>();
        if (channels != null && channels.Length == 1) {
            target = channels[0];
        }
    }
    public static void AutoAssignPlot(Component owner, ref Plot3D target) {
        var plots = owner.gameObject.GetComponentsInParent<Plot3D>();
        if (plots != null && plots.Length == 1) {
            target = plots[0];
        }
    }

    public static void AutoAssignColorChannel(Component owner, ref PlotColorChannel target) {
        var colorChannels = owner.gameObject.GetComponents<PlotColorChannel>();
        if (colorChannels != null && colorChannels.Length == 1) {
            target = colorChannels[0];
        }
    }

    public static Material FindPlotChannelLineMaterial() {
        var mats = Resources.FindObjectsOfTypeAll(typeof(Material));
        Debug.Log(string.Join(", ", mats.Select(v => v.name).ToArray()));
        return (Material)mats.FirstOrDefault(v => v.name == "Plot3D_ChannelMat");
    }
   
}
