using System.Globalization;
using UnityEditor;
using UnityEngine;

namespace vietlabs {
    public static class ColorX {
        public static string xColorToHex(this Color color) {
            var c = (Color32) color;
            return c.r.ToString("X2") + c.g.ToString("X2") + c.b.ToString("X2");
        }

        public static Color xFromHSBA(float h, float s, float b, float a) {
            if (s == 0) return new Color(b, b, b, a);

            var m = b * (1 - s);
            var t = h * 6;
            var x = Mathf.FloorToInt(t);
            var d = Mathf.Clamp01((((x % 2) == 0) ? (t - x) : (1 + x - t)) * b * s + m);

            return  t < 1f  ? new Color(b, d, m, a) :
                    t < 2f  ? new Color(d, b, m, a) :
                    t < 3f  ? new Color(m, b, d, a) :
                    t < 4f  ? new Color(m, d, b, a) :
                    t < 5f  ? new Color(d, m, b, a) :
                    t <= 6f ? new Color(b, m, d, a) : new Color(0,0,0);
        }

        public static Color xHexToColor(this string hex) {
            var r = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
            var g = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
            var b = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
            return new Color32(r, g, b, 255);
        }

        public static Color xToColor(this int colorValue) {
            var a = (colorValue >> 24);
            var r = (colorValue >> 16) & 255;
            var g = (colorValue >> 8) & 255;
            var b = colorValue & 255;
            return new Color32((byte) r, (byte) g, (byte) b, (byte) a);
        }

        public static int xToInt(this Color c) {
            Color32 c32 = c;
            return (c32.a << 24) | (c32.r << 16) | (c32.g << 8) | c32.b;
        }

        public static Color xAlpha(this Color c, float alpha) {
            c.a = alpha;
            return c;
        }

        /*public static Color xAdjust(this Color c, float pctAmount) {
            c.r *= 1 - pctAmount;
            c.g *= 1 - pctAmount;
            c.b *= 1 - pctAmount;
            return c;
        }*/

        public static Color xProSkinAdjust(this ColorHSL hsl, float amount = 0.25f) {
            hsl.l += EditorGUIUtility.isProSkin ? -amount : amount;
            return hsl;
        }
    }
}

[System.Serializable]
public struct ColorHSB {
    public float h;
    public float s;
    public float b;
    public float a;

    public ColorHSB(float h, float s, float b, float a) {
        this.h = h;
        this.s = s;
        this.b = b;
        this.a = a;
    }

    public static implicit operator Color(ColorHSB hsb) {
        var r = hsb.b;
        var g = hsb.b;
        var b = hsb.b;
        if (hsb.s == 0) return new Color(Mathf.Clamp01(r), Mathf.Clamp01(g), Mathf.Clamp01(b), hsb.a);

        var max = hsb.b;
        var dif = hsb.b * hsb.s;
        var min = hsb.b - dif;
        var h = hsb.h * 360f;

        if (h < 60f)
        {
            r = max;
            g = h * dif / 60f + min;
            b = min;
        }
        else if (h < 120f)
        {
            r = -(h - 120f) * dif / 60f + min;
            g = max;
            b = min;
        }
        else if (h < 180f)
        {
            r = min;
            g = max;
            b = (h - 120f) * dif / 60f + min;
        }
        else if (h < 240f)
        {
            r = min;
            g = -(h - 240f) * dif / 60f + min;
            b = max;
        }
        else if (h < 300f)
        {
            r = (h - 240f) * dif / 60f + min;
            g = min;
            b = max;
        }
        else if (h <= 360f)
        {
            r = max;
            g = min;
            b = -(h - 360f) * dif / 60 + min;
        }
        else
        {
            r = 0;
            g = 0;
            b = 0;
        }

        return new Color(Mathf.Clamp01(r), Mathf.Clamp01(g), Mathf.Clamp01(b), hsb.a);
    }
    public static implicit operator ColorHSB(Color color)
    {
        var ret = new ColorHSB
        {
            h = 0f,
            s = 0f,
            b = 0f,
            a = color.a
        };

        var r = color.r;
        var g = color.g;
        var b = color.b;

        var max = Mathf.Max(r, Mathf.Max(g, b));

        if (max <= 0) return ret;

        var min = Mathf.Min(r, Mathf.Min(g, b));
        var dif = max - min;

        if (max > min)
        {
            if (g == max)
            {
                ret.h = (b - r) / dif * 60f + 120f;
            }
            else if (b == max)
            {
                ret.h = (r - g) / dif * 60f + 240f;
            }
            else if (b > g)
            {
                ret.h = (g - b) / dif * 60f + 360f;
            }
            else
            {
                ret.h = (g - b) / dif * 60f;
            }
            if (ret.h < 0)
            {
                ret.h = ret.h + 360f;
            }
        }
        else
        {
            ret.h = 0;
        }

        ret.h *= 1f / 360f;
        ret.s = (dif / max) * 1f;
        ret.b = max;

        return ret;
    }
}

[System.Serializable]
public struct ColorHSL
{
    internal static ColorHSL black    = Color.black;
    internal static ColorHSL blue     = Color.blue;
    internal static ColorHSL clear    = Color.clear;
    internal static ColorHSL cyan     = Color.cyan;
    internal static ColorHSL gray     = Color.gray;
    internal static ColorHSL green    = Color.green;
    internal static ColorHSL grey     = Color.grey;
    internal static ColorHSL magenta  = Color.magenta;
    internal static ColorHSL red      = Color.red;
    internal static ColorHSL white    = Color.white;
    internal static ColorHSL yellow   = Color.yellow;

    public float h;
    public float s;
    public float l;
    public float a;

    public ColorHSL(float h, float s, float l = 0.5f, float a = 1f) {
        this.h = h;
        this.s = s;
        this.l = l;
        this.a = a;
    }

    public ColorHSL dS(float ds) {
        return new ColorHSL(h, s + ds, l, a);
    }
    public ColorHSL dL(float dl) {
        return new ColorHSL(h, s, l + dl, a);
    }
    public ColorHSL dA(float da) {
        return new ColorHSL(h, s, l, a + da);
    }

    static float Value(float n1, float n2, float hue)
    {
        hue = Mathf.Repeat(hue, 360f);

        if (hue < 60f)
        {
            return n1 + (n2 - n1) * hue / 60f;
        }
        
        if (hue < 180f)
        {
            return n2;
        }
        
        if (hue < 240f)
        {
            return n1 + (n2 - n1) * (240f - hue) / 60f;
        }
        
        return n1;
    }

    public static implicit operator ColorHSL(Color c)
    {
        float h, s;
        var a = c.a;
        var cmin = Mathf.Min(c.r, c.g, c.b);
        var cmax = Mathf.Max(c.r, c.g, c.b);
        var l = (cmin + cmax) / 2f;

        if (cmin == cmax)
        {
            s = 0;
            h = 0;
        }
        else
        {
            var delta = cmax - cmin;

            s = (l <= .5f) ? (delta / (cmax + cmin)) : (delta / (2f - (cmax + cmin)));
            h = 0;

            if (c.r == cmax)
            {
                h = (c.g - c.b) / delta;
            }
            else if (c.g == cmax)
            {
                h = 2f + (c.b - c.r) / delta;
            }
            else if (c.b == cmax)
            {
                h = 4f + (c.r - c.g) / delta;
            }

            h = Mathf.Repeat(h * 60f, 360f);
        }

        return new ColorHSL(h, s, l, a);
    }

    public static implicit operator Color(ColorHSL c)
    {
        float r, g, b;
        var a = c.a;

        //	Note: there is a typo in the 2nd International Edition of Foley and
        //	van Dam's "Computer Graphics: Principles and Practice", section 13.3.5
        //	(The HLS Color Model). This incorrectly replaces the 1f in the following
        //	line with "l", giving confusing results.
        var m2 = (c.l <= .5f) ? (c.l * (1f + c.s)) : (c.l + c.s - c.l * c.s);
        var m1 = 2f * c.l - m2;

        if (c.s == 0f) {
            r = g = b = c.l;
        } else {
            r = Value(m1, m2, c.h + 120f);
            g = Value(m1, m2, c.h);
            b = Value(m1, m2, c.h - 120f);
        }

        return new Color(r, g, b, a);
    }

    public override string ToString() {
        return string.Format("[ColorHSL(h={0},s={1},l={2},a={3})]", h, s, l, a);
    }
}