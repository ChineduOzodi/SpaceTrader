using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using vietlabs;

public class h2Info {
    public static int SelectionCount;
    public static int[] SelectedInstIDs;
    public static GameObject[] SelectedGameObjects;
    
    static Dictionary<int, h2Info> map;
    public static List<h2Info> vList;

    public static int timeStamp;
    public int prefabTimeStamp;
    public int highlightTimeStamp;
	//public bool highlightChecked;

    public int instID;
    public Rect drawRect;

    public GameObject go;
    public Transform t;

    h2iTransform _transform;
    h2iComponent _component;
	
	public h2iActiveState Active;
    public h2iPrefab Prefab;
	public h2iHighlight Highlight;

    h2Info(int instID, GameObject go, Transform t) {
        this.instID = instID;
        this.go     = go;
        this.t      = t;

        if (map == null) map = new Dictionary<int, h2Info>();
        map.Add(instID, this);
    }

    public h2iTransform Transform {
        get { return (_transform ?? (_transform = new h2iTransform {info = this })).Read(timeStamp); }
    }
    public h2iComponent Component {
        get {
            //only refresh items being in view & selected
            if (_component == null) {
                _component = new h2iComponent {info = this}.Read(timeStamp); //read first time !
            } else if (Array.IndexOf(SelectedInstIDs, instID) != -1) {// SelectedInstIDs.xIndexOf(instID)
                _component.Read(timeStamp);
            }

            return _component;
        }
    }
    
    //public void Check(float stamp, bool force = false) {
    //    var newStamp = (int)stamp;
    //    if (newStamp == _timeStamp) return;//max update rate : once per second
    //    _timeStamp = newStamp;

    //    checkPrefab();
    //    //TODO : check Highlight ?
    //}

    // ----------------------- STATIC --------------------------------

    public static h2Info Get(int instID, bool autoNew = false) {
        if (map == null) {
            if (!autoNew) return null;
            map = new Dictionary<int, h2Info>();
        }

        if (map.ContainsKey(instID)) return map[instID];
        if (!autoNew) return null;

        var go = EditorUtility.InstanceIDToObject(instID) as GameObject;
        if (go == null) return null;

        return new h2Info(instID, go, go.transform);
    }

    private static float _realTime;
    private static bool _listening;
    static public void ClearVList() {
        if (vList == null) {
            vList = new List<h2Info>();
        } else {
            vList.Clear();  
        }

        if (timeStamp == 0) timeStamp++;

        if (!_listening) {
            _listening = true;

            EditorApplication.update -= ForceRefresh; 
            EditorApplication.update += ForceRefresh;
        }
    }

    static void ForceRefresh() {
        if (Time.realtimeSinceStartup - _realTime > 0.2f) { // refresh rate maximum every 0.2f
            //EditorApplication.update -= ForceRefresh;
            _realTime = Time.realtimeSinceStartup;

            //Debug.Log("Refresh ... ");

            //map.Clear();
            timeStamp++;
        }
    }

    static public void AddToVList(int instID) {
        var info = Get(instID, true);
        if (!vList.Contains(info)) vList.Add(info);
    }
}

public class h2i {
    public int timeStamp;
    public h2Info info;
}
public class h2iTransform : h2i {
    private const string Untagged = "Untagged";
    private const string More999 = "999+";

    //Basic info
    public int childCount;
    public int hiddenCount;
    public int parentCount;
    public bool isLocked;
    public bool isCombined;

    //Basic info - GUI assist
    public float szTag;
    public float szCombine;
    public float szLayer;

    public string lbTag;
    public string lbCombine;

    public h2iTransform Read(int stamp) {
        if (timeStamp == stamp) return this;
        if (timeStamp < stamp) timeStamp = stamp;

        childCount      = info.t.childCount;
        parentCount     = info.go.xParentCount();
        isLocked        = info.go.xGetFlag(HideFlags.NotEditable);

        hiddenCount     = 0;
        for (var i = 0; i< info.t.childCount; i++) {
            if ((info.t.GetChild(i).hideFlags & HideFlags.HideInHierarchy) > 0) hiddenCount++;
        }

        isCombined      = hiddenCount > 0;
        lbCombine       = childCount == 0 ? string.Empty : childCount <= 999 ? string.Empty + childCount : More999;
        lbTag           = info.go.CompareTag(Untagged) ? string.Empty : info.go.tag;
        
        szTag           = EditorStyles.miniLabel.CalcSize(new GUIContent(lbTag)).x;
        szCombine       = EditorStyles.miniLabel.CalcSize(new GUIContent(lbCombine)).x;
        return this;
    }
}
public class h2iComponent : h2i {
    public bool hasCamera;

    public int ignoreCount;
    public int duplicatedCount;
    public int missingCount;

    public h2cScriptStatus status;
    public List<Component> components;
    public List<MonoBehaviour> behaviours;

    public h2iComponent Read(int stamp) {
        if (timeStamp == stamp) return this;
        if (timeStamp < stamp) timeStamp = stamp;

        if (components == null) {
            components = new List<Component>();
        } else {
            components.Clear();
        }

        if (behaviours == null) {
            behaviours = new List<MonoBehaviour>();
        } else {
            behaviours.Clear();
        }
        
        var list = info.go.GetComponents<Component>();

        foreach (var c in list) {
            if (c == null) {
                missingCount++;
                continue;
            }

            if (c is MonoBehaviour) {
                var fullName = c.GetType().FullName;

                if (fullName.IndexOf("UnityEngine.", StringComparison.Ordinal) != 0) { //not Unity built-in type
                    //2DO : check for 3rd extensions
                    //2DO : check for ignore paths
                    behaviours.Add(c as MonoBehaviour);
                    continue;
                }
            }

            if (!hasCamera && c is Camera) hasCamera = true;

            //check for 
            components.Add(c);
        }

        if (missingCount > 0) {
            status = h2cScriptStatus.MISSING;
        } else if (behaviours.Count == 0) {
            status = h2cScriptStatus.NO_SCRIPT;
        } else {
            status = h2cScriptStatus.HAS_SCRIPT;
        }

//        Debug.Log(timeStamp + " status : " + status + ":" + behaviours.Count);

        return this;
    }
}
public class h2iPrefab { // : h2i
    public GameObject prefab;

    public int rootInstID;
    public List<int> childrenInstIDs;

    public string label;
    //public bool isPrefab;
    public PrefabType type;
}
public class h2iHighlight {
    public int goHLStamp;
    public h2HLType goHLStatus;
    public Color goHLColor;
}

public enum h2iActiveState {
	NotCheck	= 0,
	Changed		= 1,
	NotChanged	= 2
}

public enum h2cScriptStatus {
    EMPTY,
    COMPONENT,
    IGNORE,

    NO_SCRIPT,
    MISSING,
    HAS_SCRIPT
}