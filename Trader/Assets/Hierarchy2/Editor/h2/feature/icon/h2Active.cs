using UnityEditor;
using UnityEngine;

namespace vietlabs {
	public class h2Active : h2Icon {
		
		Color[] texColorOrg;
		Color[] texColorChanged;
		
        internal h2Active() {
            texList = new[] {
                EditorResource.GetTexture2D("eye_dis"),
                EditorResource.GetTexture2D("eye"),
                EditorResource.GetTexture2D("eye_dis")
            };
	        
	        texColorOrg = new []{
	        	h2Color.Get(h2ColorType.NotActive),
		        h2Color.Get(h2ColorType.Active),
		        h2Color.Get(h2ColorType.ActiveHalf)
	        };
		    
	        texColorChanged = new[] {
		        h2Color.Get(h2ColorType.NotActiveChanged),
	            h2Color.Get(h2ColorType.ActiveChanged),
	            h2Color.Get(h2ColorType.ActiveHalfChanged),
            };
	        
	        texColor = texColorOrg;

            h2Shortcut.Add(h2Shortcut.TOGGLE_ACTIVE, () => {
                var selection = h2Info.SelectedGameObjects;
                var activeGO = Selection.activeGameObject;
                var value = !activeGO.activeSelf;

                if (selection.Length > 1) {
                    Undo.RecordObjects(selection, "Toggle Active");
                    foreach (var go in selection) {
                        go.SetActive(value);
                    }
                } else {
                    activeGO.hToggleActive(true);
                }
            });
        }

        protected override string getUndoName(bool set, h2IGroup group = h2IGroup.Target, h2IValue value = h2IValue.Same) {
            if (group == h2IGroup.Target) return (set ? "Active " : "Deactive ") + target.name;
            var g = group == h2IGroup.Selection ? "Selection" : "Siblings";
            if (value != h2IValue.InvertTarget) return "Toggle Active " + g;
            return set ? "Deactive " : "Active " + g;
        }
        protected override GenericMenu GetMenu(GameObject go) {
            var menu = new GenericMenu();
            menu.xAdd("Deep Active children", ()=>go.hSetActiveChildren(true, false));
            menu.xAdd("Deep Deactive children", ()=>go.hSetActiveChildren(false, false));
            return menu;
        }
        protected override void Set(GameObject go, bool value, string undoName) {
            if (undoName != null) Undo.RecordObject(go, undoName);
            go.SetActive(value);
        }
        protected override bool Get(GameObject go) { return go.activeSelf; }
        protected override bool autoSetParent { get { return true; }}


        private static h2Active _api;
		static internal void Draw(h2Info info, Rect r, GameObject go) {
			_api = (_api ?? (_api = new h2Active()));
			
			//TODO : read if info.Active == 0
			_api.texColor = info.Active == h2iActiveState.Changed ? _api.texColorChanged : _api.texColorOrg;
			_api.DrawTarget(r, go, (go.activeSelf && !go.activeInHierarchy) ? 2 : -1);
        }
    }

    static public class h2ActiveUtils {
        internal static bool hSmartActiveGo(this GameObject go, bool value) {
            if (h2Settings.useDKVisible) {
                var c = go.GetComponent("dfControl");
                if (c != null) {
                    c.xSetProperty("IsVisible", value);
                    return true;
                }
            }
            go.SetActive(value);
            return false;
        }

        internal static bool hIsSmartActive(this GameObject go) {
            if (h2Settings.useDKVisible) {
                var c = go.GetComponent("dfControl");
                if (c != null) return (bool) c.xGetProperty("IsVisible");
            }
            return go.activeSelf;
        }

        internal static void hSetGOActive(this GameObject go, bool value, bool? activeParents = null,
            string undoKey = "h@-auto") {
            //activeParents == null : activeParents if setActive==true
            if (undoKey == "h@-auto") undoKey = value ? "Show GameObject" : "Hide GameObject";

            //if (!string.IsNullOrEmpty(undoKey)) Undo.RecordObject(go, undoKey);
            go.xRecordUndo(undoKey);
            go.SetActive(value);
            var smart = go.hSmartActiveGo(value);

            if (!smart && (activeParents ?? value) && !go.activeInHierarchy) {
                go.xForeachParent2(
                    p => {
                        //if (!string.IsNullOrEmpty(undoKey)) Undo.RecordObject(p, undoKey);
                        p.xRecordUndo(undoKey);
                        p.SetActive(true);
                        return !p.activeInHierarchy;
                    });
            }
        }

        internal static void hToggleActive(this GameObject go, bool invertMe, bool? activeParents = null) {
            var isActive = go.activeSelf;
            var key = isActive ? "Hide Selected GameObjects" : "Show Selected GameObjects";

            go.xForeachSelected(
                (item, idx) => item.hSetGOActive((!invertMe || (item == go)) ? !isActive : isActive, activeParents, key));
        }

        internal static void hSetActiveChildren(this GameObject go, bool value, bool? activeParents) {
            var key = value ? "Show Children" : "Hide Children";
            go.xForeachChild(child => child.hSetGOActive(value, activeParents, key), true);
            go.hSetGOActive(value, false, key);
        }

        internal static void hSetActiveSibling(this GameObject go, bool value, bool? activeParents = null) {
            var key = value ? "Show siblings" : "Hide siblings";
            go.xForeachSibling(item => item.hSetGOActive(value, activeParents, key));
            go.hSetGOActive(!value, false, key);
        }

        internal static void hSetActiveParents(this GameObject go, bool value) {
            var p = go.transform.parent;
            var key = value ? "Show Parents" : "Hide Parents";
            //if (go.activeSelf != value) go.SetActive(value);

            while (p != null) {
                if (p.gameObject.activeSelf != value) {
                    p.gameObject.xRecordUndo(key);
                    p.gameObject.SetActive(value);
                }
                p = p.parent;
            }
        }
    }

}
