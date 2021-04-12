using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[System.Serializable]
[System.Runtime.InteropServices.StructLayout(LayoutKind.Sequential)]
public struct ColorRgb16 {
    public ushort r;
    public ushort g;
    public ushort b;

    public ColorRgb16(ushort red, ushort green, ushort blue) {
        r = red;
        g = green;
        b = blue;
    }

    public override string ToString() {
        return $"{r:X4}{g:X4}{b:X4}";
    }

    public static ColorRgb16 FromString(string value) {
        if (value.Length != (4 * 3)) {
            throw new System.Exception("Invalid calibration data length");
        }
        ColorRgb16 result = new ColorRgb16();
        for (int i = 0; i < 3; ++i) {
            var channelValue = value.Substring(i * 4, 4);
            result[i] = ushort.Parse(value);
        }
        return result;
    }

    public static ColorRgb16 White {
        get { return new ColorRgb16 { r = 0xffff, g = 0xffff, b = 0xffff }; }
    }

    public static ColorRgb16 Black {
        get { return new ColorRgb16 { r = 0, g = 0, b = 0 }; }
    }

    public ushort this[int index] {
        get {
            if (index == 0) {
                return r;
            }
            if (index == 1) {
                return g;
            }
            if (index == 2) {
                return b;
            }
            throw new System.IndexOutOfRangeException();
        }
        set {
            if (index == 0) {
                r = value;
            } else if (index == 1) {
                g = value;
            } else
             if (index == 2) {
                b = value;
            } else {
                throw new System.IndexOutOfRangeException();
            }
        }
    }
}