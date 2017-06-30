using System;
using UnityEditor;
using UnityEditor.Graphs;
using UnityEngine;
using vietlabs;

public static class GDrawX {

/*****************************************************

            GUI LAYOUTS

*****************************************************/

    public static Rect Bar(Vector4? offset = null, Color? color = null) {
        return GUILayoutUtility.GetRect(0, Screen.width, 20f, 20f)
            .xDrawBar(offset, color);
    }

    /*****************************************************

                GUI

    *****************************************************/

    private static Color _titleBarColor {
        get { return new ColorHSL(0f, 0f, 0.35f).xProSkinAdjust(0.25f); }
    }

    public static Rect xDrawSub(this Rect r, Action<Rect> drawer, float w, bool left = true, Vector4? padding = null) {
        drawer(r.xExtractSub(w, out r, left, padding));
        return r;
    }

    public static Rect xDrawBar(this Rect r, Vector4? padding = null, Color? color = null) {
        if (padding != null) r = r.xOffset(padding.Value);
        GUI.DrawTexture(r, (color ?? _titleBarColor).xGetTexture2D());
        return r;
    }

    public static Rect xDrawL_BoldLabel(this Rect r, string title, float dx = 0f, float dy = 2f) {
        var w = EditorStyles.boldLabel.CalcSize(new GUIContent(title)).x;
        GUI.Label(r.xExtractSub(w, out r).dx(dx).dy(dy), title, EditorStyles.boldLabel);
        return r;
    }

    public static Rect xDrawR_BoldLabel(this Rect r, string title, float dx = 0f, float dy = 2f) {
        var w = EditorStyles.boldLabel.CalcSize(new GUIContent(title)).x;
        GUI.Label(r.xExtractSub(w, out r, false).dx(dx).dy(dy), title, EditorStyles.boldLabel);
        return r;
    }

    public static Rect xDrawL_Label(this Rect r, string title, GUIStyle style = null, float dx = 0f, float dy = 2f) {
        if (style == null) style = EditorStyles.label;
        var w = style.CalcSize(new GUIContent(title)).x;
        GUI.Label(r.xExtractSub(w, out r).dx(dx).dy(dy), title, style);
        return r;
    }

    public static Rect xDrawR_Label(this Rect r, string title, GUIStyle style = null, float dx = 0f, float dy = 2f) {
        if (style == null) style = EditorStyles.label;
        var w = style.CalcSize(new GUIContent(title)).x;
        GUI.Label(r.xExtractSub(w, out r, false).dx(dx).dy(dy), title, style);
        return r;
    }

    /*public static Rect xDrawSubLabel(this Rect r, string title, bool left = true, Vector2? offset = null, GUIStyle style = null) {
        if (style == null) style = EditorStyles.label;
        var w = style.CalcSize(new GUIContent(title)).x;
        GUI.Label(r.xExtractSub(w, out r, left, offset), title, style);
        return r;
    }*/

    public static Rect xDrawL_Arrow(this Rect r, ref bool isExpand, float dx = 0f, float dy = 2f, Color? c = null) {
        using (GuiX.GUIColor(c != null ? c.Value : ColorHSL.gray.xProSkinAdjust())) {
            var subRect = r.xExtractSub(16f, out r).h(16f).dx(dx).dy(dy);
            GUI.DrawTexture(subRect, EditorResource.GetTexture2D(isExpand ? "arrow_d" : "arrow_r"));
            if (subRect.xLMB_isDown().noModifier) {
                isExpand = !isExpand;
            }
        }
        return r;
    }

    public static Rect xDrawR_Arrow(this Rect r, ref bool isExpand, float dx = 0f, float dy = 2f, Color? c = null) {
        using (GuiX.GUIColor(c != null ? c.Value : ColorHSL.gray.xProSkinAdjust())) {
            var subRect = r.xExtractSub(16f, out r, false).h(16f).dx(dx).dy(dy);
            GUI.DrawTexture(subRect, EditorResource.GetTexture2D(isExpand ? "arrow_d" : "arrow_r"));
            if (subRect.xLMB_isDown().noModifier) {
                isExpand = !isExpand;
            }
        }
        return r;
    }

    public static Rect xDrawL_Tag(this Rect r, string label, float dx = 0f, float dy = -4f, Color? c = null) {
        var sz = EditorStyles.miniLabel.CalcSize(new GUIContent(label));
        r.xExtractSub(sz.x, out r).dx(dx).dy(dy).h(sz.y).xMiniTag("" + label, c != null ? c.Value : ColorHSL.red.xProSkinAdjust(0.25f));
        return r;
    }


    public static Rect xDrawL_Toggle(this Rect r, ref bool isEnable, float dx = 0f, float dy = 0f) {
        isEnable = GUI.Toggle(r.xExtractSub(16f, out r).dx(dx).dy(dy).wh(16f, 16f), isEnable, "");
        return r;
    }
    public static Rect xDrawR_Toggle(this Rect r, ref bool isEnable, bool left = true, float dx = 0f, float dy = 0f) {
        isEnable = GUI.Toggle(r.xExtractSub(16f, out r, false).dx(dx).dy(dy).wh(16f, 16f), isEnable, "");
        return r;
    }

    /*****************************************************

                UTILS

    *****************************************************/

    static Rect xExtractSub(this Rect r, float w, out Rect remainRect, bool left = true, Vector4? padding = null) {
        Rect subRect;
        var sumW = w;

        if (padding != null) {
            var v = padding.Value;
            v.z = -v.z;
            v.w = -v.w;
            padding = v;
            sumW += padding.Value.x - padding.Value.z;
        }
        remainRect = left ? r.xLeft(out subRect, sumW) : r.xRight(out subRect, sumW);
        if (padding != null) subRect = subRect.xOffset(padding.Value);
        return subRect;
    }
}
