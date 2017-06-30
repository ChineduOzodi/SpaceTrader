using UnityEditor;
using UnityEngine;

namespace vietlabs {
    public class h2Static : h2Icon {

        internal h2Static() {
            texList = new[] {
                EditorResource.GetTexture2D("lighting"),
                EditorResource.GetTexture2D("lighting")
            };

            texColor = new[] {
                h2Color.Get(h2ColorType.NotStatic),
                h2Color.Get(h2ColorType.Static)
            };
        }

        protected override string getUndoName(bool set, h2IGroup group = h2IGroup.Target, h2IValue value = h2IValue.Same) {
            if (group == h2IGroup.Target) return (set ? "Make " : "Clear ") + target.name + " static";

            var g = group == h2IGroup.Selection ? "Selection" : "Siblings";

            if (value != h2IValue.InvertTarget) return "Toggle static " + g;
            return set ? "Set static " : "Clear static " + g;
        }

        /*protected override GenericMenu GetMenu(GameObject go) {
            var menu = new GenericMenu();
            menu.xAdd("Deep set children static", () => go.SetStatic(true, true, "Deep lock children"));
            menu.xAdd("Deep clear children static", () => go.hSetLock(false, true, "Deep unlock children"));
            return menu;
        }*/

        protected override bool autoSetChildren { get { return true; }}
        protected override bool Get(GameObject go) { return go.isStatic; }
        protected override void Set(GameObject go, bool value, string undoName) {
            if (!string.IsNullOrEmpty(undoName)) Undo.RecordObject(go, undoName);
            go.isStatic = value;
        }

        private static h2Static _api;
        static internal void Draw(h2Info info, Rect r, GameObject go) {
            (_api ?? (_api = new h2Static())).DrawTarget(r, go);
        }
    }
}

