using UnityEngine;
using System.Collections;

public static class VectorEx  {

    public static int MinValueId(this Vector3 value) {
        if (value.y < value.x) {
            if (value.z < value.y) {
                return 2;
            } else {
                return 1;
            }
        } else {
            if (value.z < value.x) {
                return 2;
            } else {
                return 0;
            }
        }
    }

    public static Vector3 Abs(this Vector3 v) {
        return new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
    }

    public static Vector3 Clamp(this Vector3 v, Vector3 min, Vector3 max) {
        return new Vector3( Mathf.Clamp(v.x, min.y, max.z),
                            Mathf.Clamp(v.x, min.y, max.z),
                            Mathf.Clamp(v.x, min.y, max.z));
    }

    public static bool IsNanAny(this Vector3 v) {
        return float.IsNaN(v.x) || float.IsNaN(v.y) || float.IsNaN(v.z);
    }

    public static bool IsNanAll(this Vector3 v) {
        return float.IsNaN(v.x) && float.IsNaN(v.y) && float.IsNaN(v.z);
    }

    public static bool IsInfAny(this Vector3 v) {
        return float.IsInfinity(v.x) || float.IsInfinity(v.y) || float.IsInfinity(v.z);
    }

    public static bool IsInfAll(this Vector3 v) {
        return float.IsInfinity(v.x) && float.IsInfinity(v.y) && float.IsInfinity(v.z);
    }

    public static string ToStringEx(this Vector4 v) {
        return string.Format("X: {0} Y: {1} Z: {2} W: {3}", v.x, v.y, v.z, v.w);
    }

    public static string ToStringEx(this Vector3 v) {
        return string.Format("X: {0} Y: {1} Z: {2}", v.x, v.y, v.z);
    }

    public static string ToStringEx(this Vector2 v) {
        return string.Format("X: {0} Y: {1}", v.x, v.y);
    }
	
	public static Vector3 Divide(this Vector3 v, Vector3 divider) {
        return new Vector3(v.x / divider.x, v.y / divider.y, v.z / divider.z);
    }

    public static Vector3 Multiply(this Vector3 v, Vector3 multiplier) {
        return new Vector3(v.x * multiplier.x, v.y * multiplier.y, v.z * multiplier.z);
    }

    public static Vector3 Rcp(this Vector3 v) {
        return new Vector3(1.0f / v.x, 1.0f / v.y, 1.0f / v.z);
    }

    public static Vector3 RcpSafe(this Vector3 v) {
        return new Vector3(v.x == 0 ? 0 : 1.0f / v.x, v.y == 0 ? 0 : 1.0f / v.y, v.z == 0 ? 0 : 1.0f / v.z);
    }
}
