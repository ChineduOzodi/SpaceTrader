using System.Configuration;
using UnityEditor;
using UnityEngine;
using vietlabs;

internal enum h2ColorType {
    Active      = 0,
    ActiveHalf  = 1,
    NotActive   = 2,

    Lock        = 3,
    Unlock      = 4,

    ParentLine  = 5,
    ParentHL    = 6,

    Script      = 7,
    Missing     = 8,

    Static      = 9,
	NotStatic   = 10,
	
	ActiveChanged		= 11,
	ActiveHalfChanged	= 12,
	NotActiveChanged	= 13,
	
}

internal class h2Color {
    static private readonly Color[] defaultDark = {
        // Active, ActiveHalf, NotActive
        new Color(1f, 1f, 0f, 0.75f),
        new Color(0.5f, 0.5f, 0f),
        new Color(0.0f, 0.0f, 0.0f, 0.3f),

        // Lock, Unlock
        new Color(0.7f, 0.7f, 0f),
        new Color(0.0f, 0.0f, 0.0f, 0.3f),

        // Parent Line, Parent Highlight
        new Color(0.0f, 0.85f, 0.85f),
        new Color(1f, 1f, 1f, 0.05f),

        // Script, Missing
        new Color(0.0f, 0.85f, 0.0f),
	    new Color(0.0f, 0.85f, 0.0f),

        // Static, NotStatic
        new Color(0.7f, 0.7f, 0f),
	    new Color(0.0f, 0.0f, 0.0f, 0.3f),
	    
	    // Active changed, HafActive Changed, NonActive Changed
	    new Color(1f, 0f, 0f, 0.75f),
	    new Color(0.5f, 0.0f, 0f),
	    new Color(0.1f, 0.0f, 0.0f, 0.3f),
    };

    static private readonly Color[] defaultLight = {
        // Active, ActiveHalf, NotActive
        new Color32(245, 245, 51,  255),    // H=60, S=80, B=100
        new Color32(242, 242, 144, 255),    // H=60, S=40, B=95
        new Color32(165, 165, 165, 255),    // H=0,  S=0,  B=65

        // Lock, Unlock
        new Color32(255, 255, 51, 255),
        new Color32(165, 165, 165, 255),

        // Parent Line, Parent Highlight
	    new Color(0.85f, 0.85f, 0.85f),
	    new Color(0f, 0f, 0f, 0.1f),

        // Script, Missing
        new Color(0.4f, 1f, 1f),
        new Color(0.4f, 1f, 1f),

        // Static, NotStatic
        new Color32(255, 255, 51, 255),
	    new Color32(165, 165, 165, 255),
	    
	    
	    // Active changed, HafActive Changed, NonActive Changed
	    new Color32(195, 0, 0, 255),		// H=60, S=80, B=100
	    new Color32(195, 108, 96, 255),    // H=60, S=40, B=95
	    new Color32(255, 255, 255, 255),    // H=0,  S=0,  B=65
    };


    static Color[] _colors; //cache colors for current theme

    static public Color Get(h2ColorType t) {
        if (_colors == null || _colors.Length == 0) _colors = Load();
        return _colors[(int)t];
    }

    static public Color[] Load(bool? proSkin = null) {
        var isPro = proSkin ?? EditorGUIUtility.isProSkin;
        var arr = EditorPersist.Get(
            isPro ? "h2.darkColors" : "h2.lightColors",
            isPro ? defaultDark : defaultLight
        );
        return arr;
    }
    static public void Save(Color[] vals, bool? proSkin = null) {
        var isPro = proSkin ?? EditorGUIUtility.isProSkin;
        EditorPersist.Set(isPro ? "h2.darkColors" : "h2.lightColors", vals);
    }
}

public class h2Settings {

    public static bool enable {
        get { return EditorPersist.Get("h2.enable", true); }
        set { EditorPersist.Set("h2.enable", value); }
    }


    // SHOW / HIDE ICONS
    public const int nIcons        = 9;
    public const int nModes        = 5;
    const string defaultIM         = "101010000.001000000.000000000.000000000.000000000"; //"101011000.001000000.111111111.000000000.000000000.000000000";
    
    //in order : script, lock, visible, static, childCount, prefab, layer, tag, depth
    public static string iconModes {  
        get {
            var result = EditorPersist.Get("h2.iconModes", defaultIM);
            if (result.Length == defaultIM.Length) return result;

            //invalid saved data, return default
            EditorPersist.Set("h2.iconModes", defaultIM);
            return defaultIM;
        }

        set { EditorPersist.Set("h2.iconModes", value); }
    }
    public static int currentMode {
        get { return EditorPersist.Get("h2.currentMode", 0); }
        set { EditorPersist.Set("h2.currentMode", value); }
    }

    public static bool enableIcons {
        get { return EditorPersist.Get("h2.enableIcons", true); }
        set { EditorPersist.Set("h2.enableIcons", value); }
    }
    public static bool showIco_Active {
        get { return EditorPersist.Get("h2.showIco_Active", true); }
        set { EditorPersist.Set("h2.showIco_Active", value); }
    }
    public static bool showIco_Script {
        get { return EditorPersist.Get("h2.showIco_Script", true); }
        set { EditorPersist.Set("h2.showIco_Script", value); }
    }
    public static bool showIco_Lock {
	    get { return EditorPersist.Get("h2.showIco_Lock", true); }
        set { EditorPersist.Set("h2.showIco_Lock", value); }
    }
    public static bool showIco_Children {
	    get { return EditorPersist.Get("h2.showIco_Children", true); }
        set { EditorPersist.Set("h2.showIco_Children", value); }
    }
    public static bool showIco_Static {
	    get { return EditorPersist.Get("h2.showIco_Static", true); }
        set { EditorPersist.Set("h2.showIco_Static", value); }
    }
    public static bool showIco_Depth {
        get { return EditorPersist.Get("h2.showIco_Depth", true); }
        set { EditorPersist.Set("h2.showIco_Depth", value); }
    }
    public static bool showIco_Prefab {
        get { return EditorPersist.Get("h2.showIco_Prefab", true); }
        set { EditorPersist.Set("h2.showIco_Prefab", value); }
    }

    public static bool showIco_Component {
        get { return EditorPersist.Get("h2.showIco_Component", true); }
        set { EditorPersist.Set("h2.showIco_Component", value); }
    }
    public static bool showIco_Layer {
        get { return EditorPersist.Get("h2.showIco_Layer", true); }
        set { EditorPersist.Set("h2.showIco_Layer", value); }
    }
    public static bool showIco_Tag {
        get { return EditorPersist.Get("h2.showIco_Tag", true); }
        set { EditorPersist.Set("h2.showIco_Tag", value); }
    }




    // ENABLE / DISABLE SHORTCUTS
    public static bool enableShortcut {
        get { return EditorPersist.Get("h2.enableShortcut", true); }
        set { EditorPersist.Set("h2.enableShortcut", value); }
    }

    public static bool use_Alt_Shortcut {
        get { return EditorPersist.Get("h2.use_Alt_Shortcut", true); }
        set { EditorPersist.Set("h2.use_Alt_Shortcut", value); }
    }
    public static bool use_Shift_Shortcut {
        get { return EditorPersist.Get("h2.use_Shift_Shortcut", true); }
        set { EditorPersist.Set("h2.use_Shift_Shortcut", value); }
    }
    public static bool use_Single_Shortcut {
        get { return EditorPersist.Get("h2.use_Single_Shortcut", true); }
        set { EditorPersist.Set("h2.use_Single_Shortcut", value); }
    }

    public static bool DrawParentHighlight {
        get { return EditorPersist.Get("h2.DrawParentHighlight", true); }
        set { EditorPersist.Set("h2.DrawParentHighlight", value); }
    }


    // COLOR SETTINGS
    const float ds = -0.5f; 
    const float da = -0.5f;

    //public static ColorHSL ColorMissing = ColorHSL.red.dS(ds).dA(da);
    //public static ColorHSL ColorValid = ColorHSL.green.dS(ds).dA(da); 

    /*public static Color color_HasScript {
        get { return EditorPersist.Get("h2.color_HasScript", (Color)(ColorHSL.green.dS(ds))); }
        set { EditorPersist.Set("h2.color_HasScript", value); }
    }
    public static Color color_MissingScript {
        get { return EditorPersist.Get("h2.color_MissingScript", (Color)(ColorHSL.red.dS(ds))); }
        set { EditorPersist.Set("h2.color_MissingScript", value); }
    }*/
    public static Color[] color_Depths {
        get {
            return EditorPersist.Get("h2.color_Depths", new Color[] {
                ColorHSL.red.dS(ds).dA(da),
                ColorHSL.yellow.dS(ds).dA(da),
                ColorHSL.green.dS(ds).dA(da),
                ColorHSL.blue.dS(ds).dA(da),
                ColorHSL.magenta.dS(ds).dA(da),
                ColorHSL.cyan.dS(ds).dA(da)

                //ColorX.xFromHSBA(0.0f, 0.65f, 1, 0.5f),
                //ColorX.xFromHSBA(0.1f, 0.65f, 1, 0.5f),
                //ColorX.xFromHSBA(0.2f, 0.65f, 1, 0.5f),
                //ColorX.xFromHSBA(0.3f, 0.65f, 1, 0.5f),
                //ColorX.xFromHSBA(0.4f, 0.65f, 1, 0.5f),
                //ColorX.xFromHSBA(0.5f, 0.65f, 1, 0.5f),
                //ColorX.xFromHSBA(0.6f, 0.65f, 1, 0.5f),
                //ColorX.xFromHSBA(0.7f, 0.65f, 1, 0.5f),
                //ColorX.xFromHSBA(0.8f, 0.65f, 1, 0.5f),
                //ColorX.xFromHSBA(0.9f, 0.65f, 1, 0.5f)
            });
        }
        set { EditorPersist.Set("h2.color_Depths", value); }
    }
    public static Color[] color_Layers {
        get
        {
            return EditorPersist.Get("h2.color_Layers", new Color[] {
                ColorHSL.red.dS(ds).dA(da),
                ColorHSL.yellow.dS(ds).dA(da),
                ColorHSL.green.dS(ds).dA(da),
                ColorHSL.blue.dS(ds).dA(da),
                ColorHSL.magenta.dS(ds).dA(da),
                ColorHSL.cyan.dS(ds).dA(da)
            });
        }
        set { EditorPersist.Set("h2.color_Layers", value); } 
    }
    public static Color[] color_Tags {
        get {
            return EditorPersist.Get("h2.color_Tags", new Color[] {
                ColorHSL.red.dS(ds).dA(da),
                ColorHSL.yellow.dS(ds).dA(da),
                ColorHSL.green.dS(ds).dA(da),
                ColorHSL.blue.dS(ds).dA(da),
                ColorHSL.magenta.dS(ds).dA(da),
                ColorHSL.cyan.dS(ds).dA(da)
            });
        }
        set { EditorPersist.Set("h2.color_Tags", value); }
    }


    // CUSTOM SCRIPTS
    public static h2CustomScript[] customScripts {
        get {
            var strList = EditorPersist.Get("h2.customScripts", new[] {
                h2CustomScript.FromParts("UILabel", "UILabel", Color.green),
                h2CustomScript.FromParts("UISprite", "UISprite", Color.green),
                h2CustomScript.FromParts("UITexture", "UITexture", Color.green)
            });
            return h2CustomScript.FromStrings(strList);
        }

        set {
            EditorPersist.Set("h2.customScripts", h2CustomScript.ToStrings(value));
        }
    }


    public static bool useSceneViewHL {
        get { return EditorPersist.Get("h2.useSceneViewHL", false); }
        set { EditorPersist.Set("h2.useSceneViewHL", value); }
    }


    // MISC SETTINGS
    private static readonly string[] _ignoreScriptPathsDefault = {
        ".dll",
        "Daikon Forge",
        "FlipbookGames",
        "iTween",
        "NGUI",
        "PlayMaker",
        "TK2DROOT",
        "VietLabs"
    };
    public static string[] ignoreScriptPaths {
        get {
            return EditorPersist.Get("h2.ignoreScriptPaths", _ignoreScriptPathsDefault); }
        set { EditorPersist.Set("h2.ignoreScriptPaths", value); }
    }
    public static int iconOffset {
	    get { return 20; /*EditorPersist.Get("h2.iconOffset", 20);*/ }
        set { EditorPersist.Set("h2.iconOffset", value); }
    }
    public static bool useDKVisible {
        get { return EditorPersist.Get("h2.useDKVisible", true); }
        set { EditorPersist.Set("h2.useDKVisible", value); }
    }


    public static string[] icoActive {
        get { return EditorPersist.Get("h2.icoActive", new []{"eye", "eye_dis", "eye", "eye_dis"}); }
        set { EditorPersist.Set("h2.icoActive", value); }
    }








    
}

public class h2CustomScript {
    public string name;
    public string displayName;
    public Color color;

    public override string ToString() {
        return FromParts(name, displayName, color);
    }

    public static string FromParts(string name, string displayName, Color c) {
        return name + "!" + displayName + "!" + c.xToInt();
    }

    public h2CustomScript FromString(string str) {
        var arr = str.Split('!');
        name = arr[0];
        displayName = arr[1];
        color = int.Parse(arr[2]).xToColor();

        return this;
    }

    public static h2CustomScript[] FromStrings(string[] vals) {
        if (vals == null || vals.Length == 0) return new h2CustomScript[0];

        var scripts = new h2CustomScript[vals.Length];
        for (var i = 0; i < scripts.Length; i++) {
            scripts[i] = new h2CustomScript().FromString(vals[i]);
        }

        return scripts;
    }

    public static string[] ToStrings(h2CustomScript[] scripts) {
        if (scripts == null || scripts.Length == 0) return new string[0];

        var strList = new string[scripts.Length];
        for(var i= 0; i< strList.Length; i++) {
            strList[i] = scripts[i].ToString();
        }

        return strList;
    }
}

