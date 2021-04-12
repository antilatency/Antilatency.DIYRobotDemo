using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlotMinMaxView : MonoBehaviour {

    public PlotChannelStatistics Stats;
    public Plot3D Plot;

    public Color MinColor = Color.blue;
    public Color MaxColor = Color.red;

    void DrawLine(Vector3 p0, Vector3 p1, Color color, bool gizmo)
    {
        if (gizmo)
        {
            Gizmos.color = color;
            Gizmos.DrawLine(p0, p1);
        }
        else
        {
            GL.Color(color);
            GL.Vertex(p0);
            GL.Vertex(p1);
        }
    }

    void Render(bool gizmo)
    {
        /*if (Stats)
        {
            Vector3 min = Stats.Minimum;
            Vector3 max = Stats.Maximum;

            var p0 = new Vector3(Plot.DataBoundsMin.x, max.y, 0);
            var p1 = new Vector3(Plot.DataBoundsMax.x, max.y, 0);
            DrawLine(Plot.RescaleSample(p0), Plot.RescaleSample(p1), MaxColor, gizmo);

            p0 = new Vector3(Plot.DataBoundsMin.x, min.y, 0);
            p1 = new Vector3(Plot.DataBoundsMax.x, min.y, 0);
            DrawLine(Plot.RescaleSample(p0), Plot.RescaleSample(p1), MinColor, gizmo);
        }*/
    }

//     IEnumerator GameViewRender()
//     {
//         while (true)
//         {
//             yield return new WaitForEndOfFrame();
//             Render(false);
//         }
//     }
// 
//     void OnDrawGizmos()
//     {
//         Gizmos.matrix = transform.localToWorldMatrix;
//         Render(true);
//     }
}
