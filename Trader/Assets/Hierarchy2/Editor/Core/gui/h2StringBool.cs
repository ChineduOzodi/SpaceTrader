using System;
using System.Text;

class h2StringBool : vlbListToggleEH<string> {
    public int offset;
    public bool changed;
    public StringBuilder source;

    override protected bool Get(int idx) { return source[idx+offset] == '1'; }
    override protected void Set(int idx, bool v) {
        //Debug.Log(source +"@"+ idx + "---> " + v + "---->");
        source[idx+offset] = v ? '1' : '0';
        //Debug.Log("after ::: "+ source); 
    }

    public void Draw(int idx, Func<int, bool, bool> drawer) {
        if (idx == 0) changed = false;

        current = idx;
        var ov = Get(idx);

        //read modifier & mouse before or else we lost the chance
        d = 0;
        ReadModifier();

        var nv = drawer(idx, ov);
        if (ov == nv) return;

        changed = true;

        d = d.xBit((int)vlbGE.LMB_DOWN, true);
        //Debug.LogWarning("BEfore check ... " + d);
        Check();
    }
}