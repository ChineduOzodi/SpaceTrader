using UnityEngine;

public enum vlbGE {
    CTRL        = 1 << 0,
    ALT         = 1 << 1,
    SHIFT       = 1 << 2,
    
    LMB_DOWN    = 1 << 3,
    LMB_UP      = 1 << 4,

    RMB_DOWN    = 1 << 5,
    RMB_UP      = 1 << 6,

    WHEEL_UP    = 1 << 7,
    WHEEL_DOWN  = 1 << 8,

    KEY_DOWN    = 1 << 9,
    KEY_UP      = 1 << 10
}

public class vlbGEHandler<T> {//where T : Object
    protected const int LMB             = (int)vlbGE.LMB_DOWN;

    protected const int CTRL_LMB        = (int)vlbGE.LMB_DOWN | (int)vlbGE.CTRL;
    protected const int ALT_LMB         = (int)vlbGE.LMB_DOWN | (int)vlbGE.ALT;
    protected const int SHIFT_LMB       = (int)vlbGE.LMB_DOWN | (int)vlbGE.SHIFT;

    protected const int CTRL_ALT_LMB    = (int)vlbGE.LMB_DOWN | (int)vlbGE.CTRL | (int)vlbGE.ALT;
    protected const int CTRL_SHIFT_LMB  = (int)vlbGE.LMB_DOWN | (int)vlbGE.CTRL | (int)vlbGE.SHIFT;
    protected const int ALT_SHIFT_LMB   = (int)vlbGE.LMB_DOWN | (int)vlbGE.ALT  | (int)vlbGE.SHIFT;


    protected const int RMB             = (int)vlbGE.RMB_DOWN;

    protected const int CTRL_RMB        = (int)vlbGE.RMB_DOWN | (int)vlbGE.CTRL;
    protected const int ALT_RMB         = (int)vlbGE.RMB_DOWN | (int)vlbGE.ALT;
    protected const int SHIFT_RMB       = (int)vlbGE.RMB_DOWN | (int)vlbGE.SHIFT;

    protected T target;
    protected int d;

    public vlbGEHandler<T> ReadModifier() {
        var e = Event.current;
        d = d.xBit((int)vlbGE.CTRL,  e.control)
             .xBit((int)vlbGE.ALT,   e.alt)
             .xBit((int)vlbGE.SHIFT, e.shift);

        //if (d != 0) Debug.Log("ReadModifier ... " + d);
        return this;
    }

    public vlbGEHandler<T> ReadMouse()
    {
        var e = Event.current;
        d = d.xBit((int)vlbGE.LMB_DOWN,  e.type == EventType.MouseDown   && e.button == 0)
             .xBit((int)vlbGE.LMB_UP,    e.type == EventType.MouseUp     && e.button == 0)
             .xBit((int)vlbGE.RMB_DOWN,  e.type == EventType.MouseDown   && e.button == 1)
             .xBit((int)vlbGE.RMB_UP,    e.type == EventType.MouseUp     && e.button == 1);
        return this;
    }

    public vlbGEHandler<T> ReadKeyboard()
    {
        var e = Event.current;
        d = d.xBit((int)vlbGE.KEY_DOWN, e.type == EventType.KeyDown)
             .xBit((int)vlbGE.KEY_UP, e.type == EventType.KeyUp);
        return this;
    }

    public bool Check() {
        //Debug.Log("Check --> " + d + ":" + LMB + ":" + RMB + ":" + CTRL_LMB + ":"+ ALT_LMB);

        switch (d) {
            case LMB            : return OnLMB();          
            case RMB            : return OnRMB();          
            case CTRL_LMB       : return OnCtrlLMB();      
            case ALT_LMB        : return OnAltLMB();       
            case SHIFT_LMB      : return OnShiftLMB();     
            case CTRL_RMB       : return OnCtrlRMB();      
            case ALT_RMB        : return OnAltRMB();       
            case SHIFT_RMB      : return OnShiftRMB();     
            case CTRL_ALT_LMB   : return OnCtrlAltLMB();   
            case CTRL_SHIFT_LMB : return OnCtrlShiftLMB(); 
            case ALT_SHIFT_LMB  : return OnAltShiftLMB();  
        }
        return false;
    }

    virtual protected bool OnLMB() { return false; }

    virtual protected bool OnCtrlLMB() { return false; }
    virtual protected bool OnAltLMB() { return false; }
    virtual protected bool OnShiftLMB() { return false; }

    virtual protected bool OnCtrlAltLMB() { return false; }
    virtual protected bool OnCtrlShiftLMB() { return false; }
    virtual protected bool OnAltShiftLMB() { return false; }


    virtual protected bool OnRMB() { return false; }

    virtual protected bool OnCtrlRMB() { return false; }
    virtual protected bool OnAltRMB() { return false; }
    virtual protected bool OnShiftRMB() { return false; }
}

public class vlbListToggleEH<T> : vlbGEHandler<T> {
    public int total;
    public int current;

    virtual protected bool Get(int idx) { return false; }
    virtual protected void Set(int idx, bool value) {  }

    protected override bool OnLMB() { Set(current, !Get(current)); return true; }
    protected override bool OnAltLMB() {
        var v = !Get(current);
        if (total == 0) Debug.LogWarning("Total should not be 0, please set a total value");
        for (var i = 0; i < total; i++) {
            Set(i, (i == current) ? v : !v);
        }
        return true;
    }
    protected override bool OnCtrlLMB() {
        var v = !Get(current);
        if (total == 0) Debug.LogWarning("Total should not be 0, please set a total value");
        for (var i = 0; i < total; i++) { Set(i, v); }
        return true;
    }
    protected override bool OnCtrlAltLMB() {
        if (total == 0) Debug.LogWarning("Total should not be 0, please set a total value");
        for (var i = 0; i < total; i++) { Set(i, !Get(i)); }
        return true;
    }
}