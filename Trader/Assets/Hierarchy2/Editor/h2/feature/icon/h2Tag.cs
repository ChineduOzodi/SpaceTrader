using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace vietlabs {
    internal class h2Tag {
        internal static float MaxWidth;
        private static Texture2D bgTex;

        static internal void CalculateSize() {
            var vList = h2Info.vList;
            var style = EditorStyles.miniLabel;
            MaxWidth = 0;
			if (vList == null) return;
            for (var i = 0;i < vList.Count; i++) {
                //if (vList[i] == null || vList[i].go == null) continue;
                
                var info = vList[i].Transform;
                if (string.IsNullOrEmpty(info.lbTag)) continue;

                var w = info.szTag;
                if (info.szTag <= 0) {
                    w = style.CalcSize(new GUIContent(info.lbTag)).x; 
                    info.szTag = w;
                }

                if (MaxWidth < w) MaxWidth = w;
                //Debug.Log(info.lbTag + ":" + MaxWidth + ":" + info.tagWidth + ":" + vList.Count + ":" + Event.current.type);
            }
        }

        static internal void Draw(h2Info info, Rect r, GameObject go) {
            //Debug.Log("Draw xxx :: " + info.lbTag + ": " + info.tagWidth + ":" + go);
            var tInfo = info.Transform;
            if (string.IsNullOrEmpty(tInfo.lbTag)) return;

            if (Event.current.type == EventType.Repaint) {
                if (bgTex == null) bgTex = ColorHSL.cyan.xProSkinAdjust().xGetTexture2D();

                GUI.DrawTexture(r, bgTex);
                GUI.Label(r, tInfo.lbTag, EditorStyles.miniLabel);
            }
        }
    } 
}
