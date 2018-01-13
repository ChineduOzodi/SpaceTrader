using System;
using UnityEngine;

public static class GuiEventX {
    public static GEInfo xLMB_isUp(this Rect r) {
        var e = Event.current;
        return new GEInfo {prequisite = e.type == EventType.MouseUp && e.button == 0 && r.Contains(e.mousePosition)};
    }

    public static GEInfo xLMB_isDrag(this Rect r) {
        var e = Event.current;
        return new GEInfo { prequisite = e.type == EventType.MouseDrag && r.Contains(e.mousePosition)};
    }

    public static GEInfo xLMB_isDown(this Rect r) {
        var e = Event.current;
        return new GEInfo {prequisite = e.type == EventType.MouseDown && e.button == 0 && r.Contains(e.mousePosition)};
    }

    public static GEInfo xRMB_isDown(this Rect r) {
        var e = Event.current;
        return new GEInfo {prequisite = e.type == EventType.MouseDown && e.button == 1 && r.Contains(e.mousePosition)};
    }

    public static GEInfo xRMB_isUp(this Rect r) {
        var e = Event.current;
        return new GEInfo {prequisite = e.type == EventType.MouseUp && e.button == 1 && r.Contains(e.mousePosition)};
    }

    public static GEInfo xKey_isDown(this KeyCode key) {
        var e = Event.current;
        return new GEInfo {prequisite = e.type == EventType.KeyDown && e.keyCode == key};
    }

    public static GEInfo xKey_isUp(this KeyCode key) {
        var e = Event.current;
        return new GEInfo {prequisite = e.type == EventType.KeyUp && e.keyCode == key};
    }
}

public struct GEInfo {
    internal bool prequisite;

    public static implicit operator bool(GEInfo md) { return md.prequisite; }

    internal bool GetModifier(bool ctrl, bool alt, bool shift) {
        var e = Event.current;
        var result = (e.control == ctrl) && (e.alt == alt) && (e.shift == shift);

        if (result) e.Use();
        return result;
    }

    public bool noModifier {
        get { return prequisite && GetModifier(false, false, false); }
    }

    /*[Obsolete("Use .noModifier instead, more intuitive")]
    public bool withoutModifier {
        get { return prequisite && GetModifier(false, false, false); }
    }*/

    public bool with_Ctrl {
        get { return prequisite && GetModifier(true, false, false); }
    }

    public bool with_Alt {
        get { return prequisite && GetModifier(false, true, false); }
    }

    public bool with_Shift {
        get { return prequisite && GetModifier(false, false, true); }
    }

    public bool with_CtrlAlt {
        get { return prequisite && GetModifier(true, true, false); }
    }

    public bool with_CtrlShift {
        get { return prequisite && GetModifier(true, false, true); }
    }

    public bool with_AltShift {
        get { return prequisite && GetModifier(false, true, true); }
    }

    public bool with_CtrlAltShift {
        get { return prequisite && GetModifier(true, true, true); }
    }
}