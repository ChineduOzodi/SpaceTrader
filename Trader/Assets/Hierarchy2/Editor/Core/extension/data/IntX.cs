
using UnityEngine;

static public class IntX {
    public static int xBit(this int source, int b, bool v, bool shift_b = false) {
        //Debug.Log("---> " + b + ":::" + source + ":" + v);
        var bs = shift_b ? 1 << b : b;
        return v ? source | bs : source & ~bs;
    }
}