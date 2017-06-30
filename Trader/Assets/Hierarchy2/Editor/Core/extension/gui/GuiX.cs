using System;
using UnityEditor;
using UnityEngine;
using vietlabs;

public static class GuiX {
    public static VtHzGL hzLayout {
        get { return new VtHzGL(true); }
    }

    public static VtHzGL vtLayout {
        get { return new VtHzGL(false); }
    }

    public static VtHzGL HzLayout2(params GUILayoutOption[] options) { return new VtHzGL(true, options); }
    public static VtHzGL VtLayout2(params GUILayoutOption[] options) { return new VtHzGL(true, options); }

    public static VtHzGL HzLayout2(GUIContent content, GUIStyle style, params GUILayoutOption[] options) {
        return new VtHzGL(true, content, style, options);
    }

    public static VtHzGL VtLayout2(GUIContent content, GUIStyle style, params GUILayoutOption[] options) {
        return new VtHzGL(true, content, style, options);
    }

    public static VtHzGL HzLayout2(Texture image, GUIStyle style, params GUILayoutOption[] options) {
        return new VtHzGL(true, image, style, options);
    }

    public static VtHzGL VtLayout2(Texture image, GUIStyle style, params GUILayoutOption[] options) {
        return new VtHzGL(true, image, style, options);
    }

    public static VtHzGL HzLayout2(string text, GUIStyle style, params GUILayoutOption[] options) {
        return new VtHzGL(true, text, style, options);
    }

    public static VtHzGL VtLayout2(string text, GUIStyle style, params GUILayoutOption[] options) {
        return new VtHzGL(true, text, style, options);
    }

    public static VtHzGL HzLayout2(GUIStyle style, params GUILayoutOption[] options) {
        return new VtHzGL(true, style, options);
    }

    public static VtHzGL VtLayout2(GUIStyle style, params GUILayoutOption[] options) {
        return new VtHzGL(true, style, options);
    }

    public static ScrollG xScrollView(this Rect clipRect, ref Rect contentRect, ref Vector2 scrollPosition) {
        return new ScrollG(clipRect, ref contentRect, ref scrollPosition);
    }
    public static ScrollGL xScrollView(ref Vector2 scrollPosition) {
        return new ScrollGL(ref scrollPosition);
    }

    public static ColorG GUIColor(Color c) { return new ColorG(c, 0); }
    public static ColorG GUIContentColor(Color c) { return new ColorG(c, 1); }
    public static ColorG GUIBgColor(Color c) { return new ColorG(c, 2); }

    public static DisableG DisableGroup(bool disable) { return new DisableG(disable); }


    public static LabelWidthG LabelWidth(float w) { return new LabelWidthG(w); }


    public static bool xIsLayout(this Event evt) { return evt.type == EventType.layout; }
    public static bool xIsNotLayout(this Event evt) { return evt.type != EventType.layout; }
    public static bool xIsNotUsed(this Event evt) { return evt.type != EventType.used; }

    public static bool xIconSelector(this Rect r, ref int selected, params Texture2D[] textureList) {
        var n = textureList.Length;
        var rectList = r.w(16 * n).h(16f).xHzSubRectsLeftDivide(n);
        var changed = false;

        GUI.DrawTexture(r.xExpand(4f), Color.black.xGetTexture2D());
        
        for (var i = 0; i < textureList.Length; i++) {
            var rr = rectList[i];
            if (i == selected) GUI.DrawTexture(rr, Color.blue.xGetTexture2D());
            GUI.DrawTexture(rr, textureList[i]);

            if ((i != selected) && rr.xLMB_isDown().noModifier) {
                selected = i;
                changed = true;
            }
        }

        return changed;
    }



    private static GUIStyle _miniLabelGray;
    public static GUIStyle miniLabelGrayStyle {
        get {
            if (_miniLabelGray != null) return _miniLabelGray;

            _miniLabelGray = new GUIStyle(EditorStyles.miniLabel);
            var font = _miniLabelGray.font;
            _miniLabelGray.normal.textColor = ColorHSL.gray.xProSkinAdjust(-0.05f);//.dL(-0.1f).xProSkinAdjust(-0.1f)
            _miniLabelGray.font = font;
            return _miniLabelGray; 
        }
    }


    public static Rect FlexibleSpace() {
        var r1 = GUILayoutUtility.GetRect(new GUIContent(" "), EditorStyles.label);
        GUILayout.FlexibleSpace();
        var r = GUILayoutUtility.GetLastRect();
        return new Rect(r1.x, r1.y, r1.width + r.width, r1.height + r.height);
    }

    public static Rect Height(float h) {
        return GUILayoutUtility.GetRect(0, Screen.width, h, h);
    }


    /*public static void xDrawTextureColor(this Rect r, Texture2D tex, Color c) {
        var oColor = GUI.color;
        GUI.color = c;
        GUI.DrawTexture(r, tex);
        GUI.color = oColor;

        if (r.xLMB_isDown().noModifier) {
            EditorGUIUtility.DrawColorSwatch(r.dy(20f).w(200), c);
        }
    }

    

    public static Rect xDrawBar(Color? c = null) {
        var rect = GUILayoutUtility.GetRect(0, Screen.width, 20f, 20f);
        xDrawBar(rect);
        return rect;
    }

    public static Rect xDrawBar(this Rect r) {
        GUI.DrawTexture(r, c.xGetTexture2D());
        return r;
    }

    public static Rect xDrawTitleBar(string title, float lbOffset = 0, Vector4? padding = null) {
        
        return xDrawTitleBar(r, title, lbOffset, padding);
    }

    public static Rect xDrawTitleBar(Rect r, string title, float lbOffset = 0, Vector4? padding = null) {
        if (padding != null) r = r.xOffset(padding.Value);
        var c = _titleBarColor;

        GUI.DrawTexture(r, c.xGetTexture2D());
        GUI.Label(r.dy(2f).dx(lbOffset), title, EditorStyles.boldLabel);
        return r;
    }
    public static Rect xDrawSub(this Rect r, Action<Rect> drawer, float w, bool left = true, float offset = 0) {
        Rect subRect;
        r = left ? r.xLeft(out subRect, w) : r.xRight(out subRect, w);
        drawer(subRect.dx(offset));
        return r;
    }

    public static Rect xDrawSubArrow(this Rect r, ref bool isExpand, bool left = true, float dx = 0, Color? c = null) {
        var expand = isExpand;

        r = r.xDrawSub(subRect => {
            using (GUIColor(c != null ? c.Value : ColorHSL.gray.xProSkinAdjust())) {
                var drawRect = subRect.wh(16f, 16f).dy(2f);
                GUI.DrawTexture(drawRect, EditorResource.GetTexture2D(expand ? "arrow_d" : "arrow_r"));
            }

            if (subRect.xLMB_isDown().noModifier) {
                expand = !expand;
            }
        }, 16f);

        isExpand = expand;
        return r;
    }

    public static Rect xDrawSubToggle(this Rect r, ref bool isEnable, bool left = true, float dx = 0f) {
        var enable = isEnable;

        r = r.xDrawSub(subRect => {
            enable = GUI.Toggle(subRect, enable, "");
        }, 16f);

        isEnable = enable;
        return r;
    }

    public static Rect xDrawSubLabel(this Rect r, string label, GUIStyle style = null, bool left = true, float w = 0f, float dx = 0f) {
        if (w == 0) {
            if (style == null) style = EditorStyles.label;
            w = style.CalcSize(new GUIContent(label)).x;
        }

        r = r.xDrawSub(subRect => {
            GUI.Label(subRect, label, style);
        }, w);
        return r;
    }*/



    /*public static void xDrawTitleBar(Rect r, string title, float barDx = 0f) {
        var c = _titleBarColor;
        GUI.DrawTexture(r.dx(barDx), c.xGetTexture2D());
        GUI.Label(r.dy(2f), title, EditorStyles.boldLabel);
    }

    public static bool xDrawTitleBar(string title, float titleDx = 0f, Rect? padding = null, bool? isExpand = null) {
        var r = GUILayoutUtility.GetRect(0, Screen.width, 20f, 20f);
        var c = _titleBarColor;

        if (padding != null) r = r.xAdd(padding.Value);

        GUI.DrawTexture(r, c.xGetTexture2D());
        if (isExpand != null) {
            var arrowRect = r.dy(2).wh(16f, 16f);
            using (GUIColor(ColorHSL.gray.xProSkinAdjust())) {
                GUI.DrawTexture(arrowRect, EditorResource.GetTexture2D(isExpand.Value ? "arrow_d" : "arrow_r"));
            }

            if (arrowRect.xLMB_isDown().noModifier) {
                isExpand = !isExpand.Value;
            }

            GUI.Label(r.dy(2f).dx(titleDx + 10f), title, EditorStyles.boldLabel);
            return isExpand.Value;
        }

        GUI.Label(r.dy(2f).dx(titleDx), title, EditorStyles.boldLabel);
        return true;
    }

    public static void xDrawTitleBar(string title, ref bool enable, float barDx = 0f, float toggleDx = 0f) {
        var r = GUILayoutUtility.GetRect(0, Screen.width, 20f, 20f);
        var c = _titleBarColor;

        GUI.DrawTexture(r.dx(barDx), c.xGetTexture2D());

        var r2 = r.dy(2f);
        GUI.Label(r2, title, EditorStyles.boldLabel);
        enable = GUI.Toggle(r2.xSubRectRight(20f).dx(toggleDx), enable, "");
    }*/


    public static void DrawResource(ref string[] icons, ref Color[] colors) {
        var rr = GUILayoutUtility.GetRect(80, 20f).wh(80f, 20f).xHzSplitByWeight(1, 1, 1, 1);
        Color light = new Color32(192,192,192,255);
        Color dark = new Color32(49,49,49,255);

        GUI.DrawTexture(rr[0].w(40f), dark.xGetTexture2D());
        GUI.DrawTexture(rr[0].dx(40f).w(40f), light.xGetTexture2D());

        var oColor = GUI.color;
        for (var i = 0; i < icons.Length; i++) {
            GUI.color = colors[i];
            GUI.DrawTexture(rr[i].wh(16f, 16f), EditorResource.GetTexture2D(icons[i]));
        }

        GUI.color = oColor;
    }

    public static bool GLToggle(bool value, string label, float w) {
        EditorGUI.BeginChangeCheck();
        var v = EditorGUILayout.ToggleLeft(label, value, GUILayout.Width(w));
        return EditorGUI.EndChangeCheck() ? v : value;
    }

    private static GUIContent guiLB;

    private static GUIContent GetLB(string str) {
        if (guiLB == null) {
            guiLB = new GUIContent(str);
        } else {
            guiLB.text = str;
        }
        return guiLB;
    }
    public static bool xMiniTag(this Rect r, string lb, Texture2D bgTex, bool autoSize = true, float lbAlign = 0.5f, float h = 18f) {
        var style   = EditorStyles.miniLabel;
        var glb     = GetLB(lb);
        var lbRect  = style.CalcSize(glb);

        var rr = r.wh(autoSize ? lbRect.x : r.width, r.height);

        if (Event.current.type == EventType.repaint) {
            GUI.DrawTexture(rr, bgTex);
            var offsetX = Mathf.Max(0, rr.width - lbRect.x) * lbAlign;
            var offsetY = (rr.height - h) * 0.5f;
            GUI.Label(rr.dx(offsetX).dy(offsetY).h(lbRect.y), glb, EditorStyles.miniLabel);
        }

        var isClicked = rr.xLMB_isDown().noModifier;
        return isClicked;
    }

   public static bool xMiniTag(this Rect r, string lb, Color c, bool autoSize = true, float lbAlign = 0.5f, float h = 18f) {
        /*var style = EditorStyles.miniLabel;
        var glb = GetLB(lb);
        var lbRect = style.CalcSize(glb);

        var rr = r.wh(autoSize ? lbRect.x : r.width, r.height);

        GUI.DrawTexture(rr, c.xGetTexture2D());
        var offsetX = Mathf.Max(0, rr.width - lbRect.x) * lbAlign;
        var offsetY = (rr.height - h) * 0.5f;

        GUI.Label(rr.dx(offsetX).dy(offsetY).h(lbRect.y), glb, EditorStyles.miniLabel);

        var isClicked = rr.xLMB_isDown().noModifier;*/
        return r.xMiniTag(lb, c.xGetTexture2D(), autoSize, lbAlign, h);
    }

    public static bool xMiniButton(this Rect r, string lb, bool autoSize = true, float lbAlign = 0.5f, bool drawButton = true) {
        //lb = "999+";
        var style = EditorStyles.miniLabel;
        var glb = GetLB(lb);
        var lbRect = style.CalcSize(glb);
        var rr = r.wh( (autoSize ? lbRect.x : r.width), 14f);

        lbRect = EditorStyles.label.CalcSize(glb);
        var isClicked = drawButton && GUI.Button(rr, "", EditorStyles.miniButton);
        GUI.Label(rr.dx((rr.width - lbRect.x) * lbAlign).dy(-1f), glb, drawButton ? EditorStyles.miniLabel : EditorStyles.label);

        return isClicked;
    }



    /*private static Color? bgColor;
    private static Color? cColor;
    public static void SaveBGColor(Color c) {
        if (bgColor != null) {
            Debug.LogWarning("Multiple level of SaveBG Color not yet support");
            return;
        }
        bgColor = GUI.backgroundColor;
        GUI.backgroundColor = c;
    }
    public static void RestoreBGColor() {
        if (bgColor == null) {
            Debug.LogWarning("Try to SaveBG Color before restore");
            return;
        }
        GUI.backgroundColor = bgColor.Value;
        bgColor = null;
    }

    public static void SaveContentColor(Color c)
    {
        if (cColor != null)
        {
            Debug.LogWarning("Multiple level of SaveBG Color not yet support");
            return;
        }
        cColor = GUI.color;
        GUI.color = c;
    }
    public static void RestoreContentColor()
    {
        if (cColor == null)
        {
            Debug.LogWarning("Try to SaveBG Color before restore");
            return;
        }
        GUI.color = cColor.Value;
        cColor = null;
    }*/

}

public struct ColorG : IDisposable {
    private readonly Color c;
    private readonly int t;

    public ColorG(Color color, int type) {
        t = type;
        switch (type) {
            case 0:
                c = GUI.color;
                GUI.color = color;
            break;
            case 1:
                c = GUI.contentColor;
                GUI.contentColor = color;
            break;
            default:
                c = GUI.backgroundColor;
                GUI.backgroundColor = color;
            break;
        }
    }

    public void Dispose() {
        switch (t) {
            case 0:
                GUI.color = c;
                break;
            case 1:
                GUI.contentColor = c;
                break;
            default:
                GUI.backgroundColor = c;
                break;
        }
    }
}

public struct DisableG : IDisposable {
    public DisableG(bool isDisable) {
        EditorGUI.BeginDisabledGroup(isDisable);
    }

    public void Dispose() {
        EditorGUI.EndDisabledGroup();
    }
}

public struct VtHzGL : IDisposable /* Using struct to prevent gc */ {
    private readonly bool m_isHorz;

    public VtHzGL(bool hz, params GUILayoutOption[] options) {
        m_isHorz = hz;
        if (m_isHorz) GUILayout.BeginHorizontal(options);
        else GUILayout.BeginVertical(options);
    }

    public VtHzGL(bool hz, GUIContent content, GUIStyle style, params GUILayoutOption[] options) {
        m_isHorz = hz;
        if (m_isHorz) GUILayout.BeginHorizontal(content, style, options);
        else GUILayout.BeginVertical(content, style, options);
    }

    public VtHzGL(bool hz, Texture image, GUIStyle style, params GUILayoutOption[] options) {
        m_isHorz = hz;
        if (m_isHorz) GUILayout.BeginHorizontal(image, style, options);
        else GUILayout.BeginVertical(image, style, options);
    }

    public VtHzGL(bool hz, string text, GUIStyle style, params GUILayoutOption[] options) {
        m_isHorz = hz;
        if (m_isHorz) GUILayout.BeginHorizontal(text, style, options);
        else GUILayout.BeginVertical(text, style, options);
    }

    public VtHzGL(bool hz, GUIStyle style, params GUILayoutOption[] options) {
        m_isHorz = hz;
        if (m_isHorz) GUILayout.BeginHorizontal(style, options);
        else GUILayout.BeginVertical(style, options);
    }

    public void Dispose() {
        if (m_isHorz) GUILayout.EndHorizontal();
        else GUILayout.EndVertical();
    }
}

public struct ScrollG : IDisposable {
    private readonly bool m_scroll;

    public ScrollG(Rect clipRect, ref Rect contentRect, ref Vector2 scrollPosition) {
        var horz = contentRect.width > clipRect.width;
        var vert = contentRect.height > clipRect.height;
        m_scroll = horz || vert;

        if (!m_scroll) return;

        if (vert) contentRect.width -= 18;
        if (horz) contentRect.height -= 18;
        scrollPosition = GUI.BeginScrollView(clipRect, scrollPosition, contentRect, horz, vert);
    }

    public void Dispose() { if (m_scroll) GUI.EndScrollView(); }
}

public struct ScrollGL : IDisposable {
    public ScrollGL(ref Vector2 scrollPos) {
        scrollPos = GUILayout.BeginScrollView(scrollPos);
    }

    public void Dispose() {
        GUILayout.EndScrollView();
    }
}


public struct LabelWidthG : IDisposable {
    private readonly float oSize;
    public LabelWidthG(float sz) {
        oSize = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = sz;
    }
    public void Dispose() {
        EditorGUIUtility.labelWidth = oSize;
    }
}