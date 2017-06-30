using System;
using UnityEditor;
using UnityEngine;

namespace vietlabs {
    public class h2SelectionHL {
        static internal void Draw(h2Info info, Rect r, GameObject go) {
            if (h2Info.SelectionCount < 2) return;

            var idx = Array.IndexOf(h2Info.SelectedInstIDs, info.instID);
            if (idx == -1) return;

            using (GuiX.GUIColor(go == Selection.activeGameObject ? Color.red : Color.white)) {
                GUI.DrawTexture(r, EditorResource.GetTexture2D("circle"));
            }
            
            if (r.xLMB_isDown().noModifier) {
                //var instList = Selection.instanceIDs;
                Selection.activeInstanceID = go.GetInstanceID();
                Selection.instanceIDs = h2Info.SelectedInstIDs;
            }
        }
    }
}
