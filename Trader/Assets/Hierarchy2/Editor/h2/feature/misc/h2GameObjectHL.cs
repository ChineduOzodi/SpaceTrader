using System;
using UnityEditor;
using UnityEngine;

namespace vietlabs {
    internal class h2GameObjectHL {
        static int _stamp;

        const float _alpha = 0.3f;
        static Color[] _colors = new Color[] {
            ColorHSL.magenta   .xProSkinAdjust().xAlpha(_alpha),
            ColorHSL.cyan      .xProSkinAdjust().xAlpha(_alpha),
            ColorHSL.blue      .xProSkinAdjust().xAlpha(_alpha),
            ColorHSL.yellow    .xProSkinAdjust().xAlpha(_alpha),
            ColorHSL.green     .xProSkinAdjust().xAlpha(_alpha),
            ColorHSL.white     .xProSkinAdjust().xAlpha(_alpha),
            ColorHSL.black     .xProSkinAdjust().xAlpha(_alpha)
        };

        //static int delayCounter;
        //static string basePath  = "ProjectSettings/VietLabs/";
        //static string path      = basePath + "h2GOHL.asset";

        //static Dictionary<int, h2HLInfo> map;

        static internal void Refresh() {//bool write = false
            _stamp++;
            WindowX.Hierarchy.Repaint();
            //if (write) DelayWrite();
        }

        internal static void Init() {
            //map = new Dictionary<int, h2HLInfo>();

            //if (File.Exists(path)) {
                //var text = path.xReadText();
                //var list = text.Split(';');

                //for (var i = 0;i < list.Length; i++) {
                //    var info = h2HLInfo.FromString(list[i]);
                //    if (info != null) {
                //        map.Add(info.instID, info);
                //    }
                //}
            //}

            h2Shortcut.Add(h2Shortcut.HIGHLIGHT_GAMEOBJECT_COLOR0, cmdClearHL);
            h2Shortcut.Add(h2Shortcut.HIGHLIGHT_GAMEOBJECT_COLOR1, cmdSetHL1);
            h2Shortcut.Add(h2Shortcut.HIGHLIGHT_GAMEOBJECT_COLOR2, cmdSetHL2);
            h2Shortcut.Add(h2Shortcut.HIGHLIGHT_GAMEOBJECT_COLOR3, cmdSetHL3);
            h2Shortcut.Add(h2Shortcut.HIGHLIGHT_GAMEOBJECT_COLOR4, cmdSetHL4);
            h2Shortcut.Add(h2Shortcut.HIGHLIGHT_GAMEOBJECT_COLOR5, cmdSetHL5);
            h2Shortcut.Add(h2Shortcut.HIGHLIGHT_GAMEOBJECT_COLOR6, cmdSetHL6);
            h2Shortcut.Add(h2Shortcut.HIGHLIGHT_GAMEOBJECT_COLOR7, cmdSetHL7);
            h2Shortcut.Add(h2Shortcut.HIGHLIGHT_GAMEOBJECT_COLOR8, cmdSetHL8);
            h2Shortcut.Add(h2Shortcut.HIGHLIGHT_GAMEOBJECT_COLOR9, cmdSetHL9);

            _stamp = 1;
            Refresh();
        }

        static internal GenericMenu Menu(GenericMenu menu, string prefix = null)
        {
            if (prefix == null) prefix = string.Empty;

            menu.xAdd(prefix + "Clear " + h2Shortcut.HIGHLIGHT_GAMEOBJECT_COLOR0,     cmdClearHL)
                .xAddSep(prefix)
                .xAdd(prefix + "Color 1 " + h2Shortcut.HIGHLIGHT_GAMEOBJECT_COLOR1,  cmdSetHL1)
                .xAdd(prefix + "Color 2 " + h2Shortcut.HIGHLIGHT_GAMEOBJECT_COLOR2,  cmdSetHL2)
                .xAdd(prefix + "Color 3 " + h2Shortcut.HIGHLIGHT_GAMEOBJECT_COLOR3,  cmdSetHL3)
                .xAdd(prefix + "Color 4 " + h2Shortcut.HIGHLIGHT_GAMEOBJECT_COLOR4,  cmdSetHL4)
                .xAdd(prefix + "Color 5 " + h2Shortcut.HIGHLIGHT_GAMEOBJECT_COLOR5,  cmdSetHL5)
                .xAdd(prefix + "Color 6 " + h2Shortcut.HIGHLIGHT_GAMEOBJECT_COLOR6,  cmdSetHL6)
                .xAdd(prefix + "Color 7 " + h2Shortcut.HIGHLIGHT_GAMEOBJECT_COLOR7,  cmdSetHL7)
                .xAdd(prefix + "Color 8 " + h2Shortcut.HIGHLIGHT_GAMEOBJECT_COLOR8,  cmdSetHL8)
                .xAdd(prefix + "Color 9 " + h2Shortcut.HIGHLIGHT_GAMEOBJECT_COLOR9,  cmdSetHL9);

            return menu;
        }


        /*internal static void UpdateHLInfo(List<h2Info> vList) {

            for (var i = 0;i < vList.Count; i++) {
                var item = vList[i];
                if (item.goHLStatus != h2HLType.Unknown) continue;

                if (map.ContainsKey(item.instID)) {
                    var info = map[item.instID];
                    item.goHLStatus = h2HLType.Highlight;
                    item.goHLColor  = info.color;
                    continue;
                }

                var p = item.go.transform.parent;
                if (p == null) {
                    item.goHLStatus = h2HLType.None;
                    continue;
                }

                var instID = p.gameObject.GetInstanceID();
                if (!map.ContainsKey(instID)) {
                    item.goHLStatus = h2HLType.None;
                    continue;
                }

                var info1 = map[instID];

                item.goHLStatus = h2HLType.DirectChildren;
                item.goHLColor  = info1.color.xAlpha(0.5f);
            }
        }*/

        //static public void Repaint() {
        //    MarkDirty();
        //    WindowX.Hierarchy.Repaint();       
        //}

        internal static h2GOHLInfo Get(GameObject go) {
            if (go == null) return null;
			var vlbGO = VietLabsRT.Get<Hierarchy2RT> (false);
			if (vlbGO == null) return null;

	        var list = vlbGO.listHL;
            if (list == null) return null;

            for (var i = 0; i < list.Count; i++) {
                if (list[i].go == go) return list[i];
            }
            return null;
        }
        

        internal static void Draw(h2Info info, Rect r, GameObject go) {
            var hl = info.Highlight;

            if (info.highlightTimeStamp < h2Info.timeStamp) {//not yet checked
                info.highlightTimeStamp = h2Info.timeStamp;
                info.Highlight = null;

                var hlInfo = Get(go);

                if (hlInfo != null) {
                    hl = info.Highlight;
                    if (hl == null) {
                        hl = new h2iHighlight();
                        info.Highlight = hl;
                    }
                    hl.goHLStatus = h2HLType.Highlight;
                    hl.goHLColor = hlInfo.c;
                } else {
                    var p = info.go.transform.parent;
                    if (p != null) { 
                        hlInfo = Get(p.gameObject);
                        if (hlInfo != null) {
                            hl = info.Highlight;
                            if (hl == null) {
                                hl = new h2iHighlight();
                                info.Highlight = hl;
                            }

                            hl.goHLStatus = h2HLType.DirectChildren;
                            hl.goHLColor = hlInfo.c.xAlpha(hlInfo.c.a * 0.5f);
                        }
                    }
                }
            }

            if (hl == null) return;

            var offsetX = hl.goHLStatus == h2HLType.Highlight ? 14f : 28f;
            var rr = r.dw(offsetX).dx(-offsetX);
            
            //if (info.goHLStatus == h2HLType.Highlight) {
            //    GUI.DrawTexture(rr.w(2f), info.goHLColor.xAlpha(1f).xGetTexture2D());
            //}
            
            if ((Array.IndexOf(h2Info.SelectedInstIDs, info.instID) == -1) && info.instID != Hierarchy2.inspectorLock) {
                GUI.DrawTexture(rr, hl.goHLColor.xGetTexture2D());
            } else {
                GUI.DrawTexture(r.dx(-12f).w(2f), hl.goHLColor.xAlpha(1f).xGetTexture2D());
                //.DrawTexture(rr.w(rr.x).x(0), info.goHLColor.xAlpha(0.5f).xGetTexture2D());
            }
        }

        internal static void SetColor(GameObject go, Color c) {
            var hlInfo = Get(go);
            if (hlInfo == null) { //new
                //Debug.Log("SetColor ---> " + instID + ":" + c);
	            VietLabsRT.Get<Hierarchy2RT>(true).listHL.Add(new h2GOHLInfo {
                    go = go,
                    c = c
                });
            } else { //change color ?
                hlInfo.c = c;
            }

            Refresh();
        }

        internal static void ClearColor(GameObject go) {
			var vlbGO = VietLabsRT.Get<Hierarchy2RT> (false);
			if (vlbGO == null) return;
	        vlbGO.listHL.RemoveAll(item => item.go == go);
            Refresh();
        }

        internal static void cmdClearHL() {
            if (Selection.activeGameObject == null) return;
            ClearColor(Selection.activeGameObject);
        }

        /*internal static void DelayWrite(){
            delayCounter = 100; //delay 10 frames between writes
            EditorX.xDelayCall(WriteCache);
        }

        internal static bool WriteCache() {
            if (delayCounter-- != 0) return true;

            StringBuilder str = new StringBuilder();
            foreach (var item in map) {
                str.Append(item.Value.ToString());
                str.Append(";");
            }

            basePath.xCreatePath();
            var guid = AssetDatabase.AssetPathToGUID(EditorApplication.currentScene);
            str.ToString().xWriteText(basePath + guid, true);
            return false; 
        }*/

        internal static void cmdSetHL1() {
            if (Selection.activeGameObject == null) return;
            SetColor(Selection.activeGameObject, _colors[0]);
        }
        internal static void cmdSetHL2() {
            if (Selection.activeGameObject == null) return;
            SetColor(Selection.activeGameObject, _colors[1]);
        }
        internal static void cmdSetHL3() {
            if (Selection.activeGameObject == null) return;
            SetColor(Selection.activeGameObject, _colors[2]);
        }
        internal static void cmdSetHL4() {
            if (Selection.activeGameObject == null) return;
            SetColor(Selection.activeGameObject, _colors[3]);
        }
        internal static void cmdSetHL5() {
            if (Selection.activeGameObject == null) return;
            SetColor(Selection.activeGameObject, _colors[4]);
        }
        internal static void cmdSetHL6() {
            if (Selection.activeGameObject == null) return;
            SetColor(Selection.activeGameObject, _colors[5]);
        }
        internal static void cmdSetHL7() {
            //if (Selection.activeGameObject == null) return;
            //SetColor(.activeGameObject, Color.yellow.xAlpha(_alpha));
        }
        internal static void cmdSetHL8() {
            //if (Selection.activeGameObject == null) return;
            //SetColor(.activeGameObject, Color.cyan.xAlpha(_alpha));
        }
        internal static void cmdSetHL9() {
            //if (Selection.activeGameObject == null) return;
            //SetColor(.activeGameObject, Color.grey.xAlpha(_alpha));
        }
    }

    internal class h2HLInfo {
        public int instID;
        public Color color;

        public override string ToString() {
            var go = (GameObject) EditorUtility.InstanceIDToObject(instID);
            if (go == null) return string.Empty;
            return go.xHierarchyPath(true) + "|" + color.xToInt();
        }

        internal static h2HLInfo FromString(string str) {
            if (string.IsNullOrEmpty(str)) return null;

            var list = str.Split('|');
            if (list.Length != 2) return null;
            return new h2HLInfo {
                instID  = int.Parse(list[0]),
                color   = int.Parse(list[1]).xToColor()
            };
        }
    }

    public enum h2HLType {
        Highlight,
        DirectChildren,
        HLClear
    }
}

