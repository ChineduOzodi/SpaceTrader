using UnityEngine;

public static class RectX {
    public static readonly Rect rectZero = new Rect(0, 0, 0, 0);

    #region GETTER

    public static bool xIsPortrait(this Rect rect) { return rect.width < rect.height; }
    public static bool xHasValidSize(this Rect rect) { return rect.width > 0 && rect.height > 0; }
    public static Vector2 XY_AsVector2(this Rect rect) { return new Vector2(rect.x, rect.y); }
    public static Vector2 WH_AsVector2(this Rect rect) { return new Vector2(rect.width, rect.height); }

    #endregion

    #region SETTER

    public static Rect x(this Rect rect, float val) {
        rect.x = val;
        return rect;
    }

    public static Rect y(this Rect rect, float val) {
        rect.y = val;
        return rect;
    }

    public static Rect w(this Rect rect, float val) {
        rect.width = val;
        return rect;
    }

    public static Rect h(this Rect rect, float val) {
        rect.height = val;
        return rect;
    }

    public static Rect l(this Rect rect, float val) {
        rect.xMin = val;
        return rect;
    }

    public static Rect r(this Rect rect, float val) {
        rect.xMax = val;
        return rect;
    }

    public static Rect t(this Rect rect, float val) {
        rect.yMin = val;
        return rect;
    }

    public static Rect b(this Rect rect, float val) {
        rect.yMax = val;
        return rect;
    }

    public static Rect xy(this Rect rect, float px, float py) { return new Rect(px, py, rect.width, rect.height); }
    public static Rect wh(this Rect rect, float pw, float ph) { return new Rect(rect.x, rect.y, pw, ph); }

    public static Rect xOrientation(this Rect rect, bool portrait) {
        var c = rect.center;
        var max = Mathf.Max(rect.width, rect.height);
        var min = Mathf.Min(rect.width, rect.height);
        var w = portrait ? min : max;
        var h = portrait ? max : min;
        return new Rect(-w/2f + c.x, c.y - h/2f, w, h);
    }

    public static Rect xPortrait(this Rect rect) {
        var c = rect.center;
        var max = Mathf.Max(rect.width, rect.height);
        var min = Mathf.Min(rect.width, rect.height);
        var w = min;
        var h = max;
        return new Rect(-w/2f + c.x, c.y - h/2f, w, h);
    }

    public static Rect xLandscape(this Rect rect) {
        var c = rect.center;
        var max = Mathf.Max(rect.width, rect.height);
        var min = Mathf.Min(rect.width, rect.height);
        var w = max;
        var h = min;
        return new Rect(-w/2f + c.x, c.y - h/2f, w, h);
    }

    #endregion

    #region DELTA

    public static Rect dx(this Rect rect, float val) {
        rect.x += val;
        return rect;
    }

    public static Rect dy(this Rect rect, float val) {
        rect.y += val;
        return rect;
    }

    public static Rect dw(this Rect rect, float val) {
        rect.width += val;
        return rect;
    }

    public static Rect dh(this Rect rect, float val) {
        rect.height += val;
        return rect;
    }

    public static Rect dl(this Rect rect, float val) {
        rect.xMin += val;
        return rect;
    }

    public static Rect dr(this Rect rect, float val) {
        rect.xMax += val;
        return rect;
    }

    public static Rect dt(this Rect rect, float val) {
        rect.yMin += val;
        return rect;
    }

    public static Rect db(this Rect rect, float val) {
        rect.yMax += val;
        return rect;
    }

    #endregion

    #region SELF TRANSFORM

    public static Rect xExpand(this Rect rect, float p) {
        if (float.IsNaN(p) || float.IsInfinity(p)) return rectZero;

        rect.xMin -= p;
        rect.xMax += p;
        rect.yMin -= p;
        rect.yMax += p;
        return rect;
    }

    public static Rect xExpand(this Rect rect, float px, float py) {
        rect.xMin -= px;
        rect.yMin -= py;
        rect.xMax += px;
        rect.yMax += py;
        return rect;
    }

    public static Rect xOffset(this Rect rect, Vector4 offset) {
        rect.xMin += offset.x;
        rect.yMin += offset.y;
        rect.xMax += offset.z;
        rect.yMax += offset.w;
        return rect;
    }

    public static Rect xOffset(this Rect rect, float? left = null, float? right = null, float? top = null,
        float? bottom = null) {
        rect.xMin += left ?? 0;
        rect.yMin += top ?? 0;
        rect.xMax += right ?? 0;
        rect.yMax += bottom ?? 0;
        return rect;
    }

    public static Rect xAdjustTL(this Rect rect, float? w = null, float? h = null) {
        if (w == null) w = rect.width;
        if (h == null) h = rect.height;

        return new Rect(
            rect.x + rect.width - w.Value,
            rect.y + rect.height - h.Value,
            w.Value, h.Value
        );
    }

    public static Rect xMove(this Rect rect, Vector2 v) {
        return new Rect(rect.x + v.x, rect.y + v.y, rect.width, rect.height);
    }

    public static Rect xMove(this Rect rect, float x, float y) {
        return new Rect(rect.x + x, rect.y + y, rect.width, rect.height);
    }

    public static Rect xRound(this Rect rect) {
        return new Rect(
            Mathf.RoundToInt(rect.x), Mathf.RoundToInt(rect.y), Mathf.RoundToInt(rect.width),
            Mathf.RoundToInt(rect.height));
    }

    public static Rect xMultiply(this Rect rect, Vector2 v) {
        return new Rect(rect.x*v.x, rect.y*v.y, rect.width*v.x, rect.height*v.y);
    }

    public static Rect xMultiply(this Rect rect, float sx, float sy) {
        return new Rect(rect.x*sx, rect.y*sy, rect.width*sx, rect.height*sy);
    }

    public static Rect xSlide(this Rect rect, float pctX = 0, float pctY = 0) {
        rect.x += pctX*rect.width;
        rect.y += pctY*rect.height;
        return rect;
    }

    #endregion

    #region COMPLEX TRANSFORM

    public static Rect xScaleTo(this Rect r, float pw, float ph) {
        var sx = r.width == 0 ? 1 : pw/r.width;
        var sy = r.height == 0 ? 1 : ph/r.height;
        return new Rect(r.x*sx, r.y*sy, r.width*sx, r.height*sy);
    }

    public static Rect xScaleToW(this Rect r, float pw) {
        var s = pw/r.width;
        return new Rect(r.x*s, r.y*s, r.width*s, r.height*s);
    }

    public static Rect xScaleToH(this Rect r, float ph) {
        var s = ph/r.height;
        return new Rect(r.x*s, r.y*s, r.width*s, r.height*s);
    }

    public static Rect xFit(this Rect rect, float pw, float ph) {
        if (pw == 0 || ph == 0) return new Rect();
        var s = Mathf.Min(ph/rect.height, pw/rect.width);
        var nw = s*rect.width;
        var nh = s*rect.height;
        return new Rect((pw - nw)/2f, (ph - nh)/2f, nw, nh);
    }

    public static Rect xFill(this Rect rect, float pw, float ph) {
        if (pw == 0 || ph == 0) return new Rect();
        var s = Mathf.Max(ph/rect.height, pw/rect.width);
        var nw = s*rect.width;
        var nh = s*rect.height;
        return new Rect((pw - nw)/2f, (ph - nh)/2f, nw, nh);
    }

    public static Rect xClip(this Rect r, Rect clipRect) { //simple clipping
        if (r.xMin >= clipRect.xMax || r.xMax <= clipRect.xMin || r.yMin >= clipRect.yMax || r.yMax <= clipRect.yMin) { //out of bound
            r.width = 0;
            r.height = 0;
        }

        r.xMin = Mathf.Max(r.xMin, clipRect.xMin);
        r.xMax = Mathf.Min(r.xMax, clipRect.xMax);
        r.yMin = Mathf.Max(r.yMin, clipRect.yMin);
        r.yMax = Mathf.Min(r.yMax, clipRect.yMax);

        return r;
    }

    #endregion

    #region OPERATION

    public static Rect xAdd(this Rect rect1, Rect rect2) {
        return new Rect(rect2.x + rect1.x, rect2.y + rect1.y, rect2.width + rect1.width, rect2.height + rect1.height);
    }

    public static Rect xSubtract(this Rect rect1, Rect rect2) {
        return new Rect(rect2.x - rect1.x, rect2.y - rect1.y, rect2.width - rect1.width, rect2.height - rect1.height);
    }

    public static Rect xSubtract_Abs(this Rect rect1, Rect rect2) {
        return new Rect(
            Mathf.Abs(rect2.x - rect1.x), Mathf.Abs(rect2.y - rect1.y), Mathf.Abs(rect2.width - rect1.width),
            Mathf.Abs(rect2.height - rect1.height));
    }

    public static bool xIsDifferent(this Rect rect1, Rect rect2, float tollerant = 0.5f) {
        var r = rect1.xSubtract_Abs(rect2);
        return (r.x + r.y + r.width + r.height) > tollerant;
    }

    public static Rect xLerp(this Rect rect1, Rect rect2, float t) {
        t = Mathf.Clamp01(t);

        return new Rect(
            rect1.x + (rect2.x - rect1.x)*t, rect1.y + (rect2.y - rect1.y)*t,
            rect1.width + (rect2.width - rect1.width)*t, rect1.height + (rect2.height - rect1.height)*t);
    }

    #endregion

    #region SUB-RECTS

    public static Rect[] xHzSplitByWeight(this Rect r, params float[] weights) {
        var result = new Rect[weights.Length];
        var cx = r.x;
        var w = 0f;

        for (var i = 0; i < weights.Length; i++) {
            w += weights[i];
        }
        if (w == 0f) w = 1f;

        w = r.width/w;
        for (var i = 0; i < weights.Length; i++) {
            var rr = new Rect(cx, r.y, w*weights[i], r.height);
            result[i] = rr;
            cx += rr.width;
        }
        return result;
    }

    public static Rect[] xHzSubRectsLeft(this Rect r, params float[] widths) {
        var result = new Rect[widths.Length];
        var cx = r.x;
        for (var i = 0; i < result.Length; i++) {
            var rr = new Rect(cx, r.y, widths[i], r.height);
            result[i] = rr;
            cx += rr.width;
        }
        return result;
    }

    public static Rect[] xHzSubRectsLeftDivide(this Rect r, int count) {
        var result = new Rect[count];
        var w = r.width/count;

        for (var i = 0; i < count; i++) {
            result[i] = new Rect(i*w + r.x, r.y, w, r.height);
        }

        return result;
    }

    public static Rect[] xHzSubRectsRight(this Rect r, params float[] widths) {
        var result = new Rect[widths.Length];
        var cx = r.x + r.width;
        for (var i = 0; i < result.Length; i++) {
            var w = widths[i];
            var rr = new Rect(cx - w, r.y, w, r.height);
            result[i] = rr;
            cx -= w;
        }
        return result;
    }

    public static Rect xSubRectRight(this Rect r, float w) { return new Rect(r.x + r.width-w, r.y, w, r.height); }


    public static Rect xSubRight(this Rect rect, out Rect r, float w, float spc = 0) { //SubRectRightRef
        var rSub = new Rect(rect.x + rect.width - w, rect.y, w, rect.height);
        r = rect;
        r.width -= w + spc;
        return rSub;
    }

    public static Rect xRight(this Rect r, out Rect rSub, float w, float spc = 0) { //SubRectRightRef
        rSub = new Rect(r.x + r.width-w, r.y, w, r.height);
        r.width -= w+spc;
        return r;
    }
    public static Rect xLeft(this Rect r, out Rect rSub, float w, float spc = 0) {
        rSub = new Rect(r.x, r.y, w, r.height);
        r.x += w + spc;
        r.width -= w + spc;
        return r;
    }

    #endregion
}