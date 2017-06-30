using System.Linq;
using UnityEditor;
using UnityEngine;

namespace vietlabs {
    public enum h2IValue {
        Same,
        InvertTarget,
        ToggleEach
    }

    public enum h2IGroup {
        Target,
        Selection,
        Siblings,
        Children,
        Parents
    }

    public class h2Icon : vlbGEHandler<GameObject> {
        protected virtual string getUndoName(bool set, h2IGroup group = h2IGroup.Target, h2IValue value = h2IValue.Same) {
            return null;
        }

        protected virtual bool autoSetChildren { get { return false; } }
        // LMB also set children, to clear children use Ctrl + LMB (lock)
        protected virtual bool autoSetParent { get { return false; } }
        // LMB also set parents, to clear parents use Ctrl + LMB (visible)

        protected virtual bool Get(GameObject go) {
            return false;
        }

        protected virtual void Set(GameObject go, bool value, string undoName) {
        }

        protected virtual GenericMenu GetMenu(GameObject go) {
            return null;
        }

        protected Texture2D[] texList;
        protected Color[] texColor;

        protected virtual void DrawTarget(Rect rect, GameObject ptarget, int idx = -1) {
            target = ptarget;
            
            if (Event.current.type == EventType.Repaint) { //only do actual draw on Repaint
                if (idx == -1) {
                    idx = Get(ptarget) ? 1 : 0;
                }
                using (GuiX.GUIColor(texColor[idx])) {
                    GUI.DrawTexture(rect, texList[idx]);
                }
            }

            if (rect.Contains(Event.current.mousePosition)) {
                d = 0;
                ReadModifier().ReadMouse().Check();
            }
        }


        protected GameObject[] GetSiblings() {
            return target.transform.xGetSibling<Transform>().Select(item => item.gameObject).ToArray();
        }

        protected GameObject[] GetChildren() {
            return target.transform.xGetChildren<Transform>(true).Select(item => item.gameObject).ToArray();
        }

        protected GameObject[] GetParents() {
            return target.xGetParents().ToArray();
        }

        protected GameObject[] GetSelection() {
            return h2Info.SelectedGameObjects;
        }


        protected void SetTargetInGroup(bool v, GameObject[] arr = null, string undoName = null,
            h2IValue b = h2IValue.Same, bool useEvent = true) { //Action<T> a, 
            if (useEvent) {
                Event.current.Use();
            }
            Set(target, v, undoName);

            if (arr == null) {
                return;
            }

            foreach (var item in arr) {
                if (item.xIsEquals(target)) {
                    continue;
                }
                Set(item, (b == h2IValue.Same) ? v : (b == h2IValue.InvertTarget) ? !v : !Get(item), undoName);
            }
        }


        protected override bool OnLMB() {
            var value = !Get(target);
            var _selection = GetSelection();
            var inSelection = _selection != null && _selection.Length > 1 && _selection.Contains(target);

            if (inSelection) { //Main target is in Selection : toogle all items together with the main target
                SetTargetInGroup(value, _selection, getUndoName(value, h2IGroup.Selection));
                return true;
            }

            if (value) {
                if (autoSetChildren) {
                    SetTargetInGroup(true, GetChildren(), getUndoName(true));
                    return true;
                }

                if (autoSetParent) {
                    SetTargetInGroup(true, GetParents(), getUndoName(true));
                    return true;
                }
            }

            SetTargetInGroup(value, null, getUndoName(value));
            return true;
        }

        protected override bool OnAltLMB() {
            var value = !Get(target);
            var _selection = GetSelection();
            var inSelection = _selection != null && _selection.Length > 1 && _selection.Contains(target);
            const h2IValue rgv = h2IValue.InvertTarget;

            if (inSelection) {
                SetTargetInGroup(value, _selection, getUndoName(value, h2IGroup.Selection, rgv), rgv);
                return true;
            }

            SetTargetInGroup(value, GetSiblings(), getUndoName(value, h2IGroup.Siblings, rgv), rgv);
            return true;
        }

        protected override bool OnCtrlLMB() {
            var value = !Get(target);
            var _selection = GetSelection();
            var inSelection = _selection != null && _selection.Length > 1 && _selection.Contains(target);

            if (inSelection) {
                SetTargetInGroup(value, null, getUndoName(value));
                return true;
            }

            if (!value) {
                if (autoSetChildren) { // auto clear children
                    SetTargetInGroup(false, GetChildren(), getUndoName(false));
                    return true;
                }

                if (autoSetParent) { // auto clear parent
                    SetTargetInGroup(false, GetParents(), getUndoName(false));
                    return true;
                }
            }

            SetTargetInGroup(value, GetSiblings(), getUndoName(value, h2IGroup.Siblings));
            return true;
        }

        protected override bool OnCtrlAltLMB() {
            var value = !Get(target);
            var _selection = GetSelection();
            var inSelection = _selection != null && _selection.Length > 1 && _selection.Contains(target);
            const h2IValue rgv = h2IValue.ToggleEach;

            if (inSelection) {
                SetTargetInGroup(value, null, getUndoName(value, h2IGroup.Selection, rgv), rgv);
                return true;
            }

            SetTargetInGroup(value, GetSiblings(), getUndoName(value, h2IGroup.Siblings, rgv), rgv);
            return true;
        }

        protected override bool OnRMB() {
            var menu = GetMenu(target);
            if (menu != null) {
                Event.current.Use();
                menu.ShowAsContext();
                return true;
            }
            return false;
        }
    }
}