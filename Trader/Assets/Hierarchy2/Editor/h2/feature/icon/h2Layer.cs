using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

namespace vietlabs {
    internal class h2LayerInfo {
        public string title;
        public Texture2D bg;
    }

    internal class h2Layer {
        internal static float maxWidth;
        private static Dictionary<int, h2LayerInfo> layerMap;

        internal static void CalculateSize() {
            var vList = h2Info.vList;
            if (layerMap == null) RefreshCache();

            var style = EditorStyles.miniLabel;
            maxWidth = 0;
			if (vList == null) return;

            for (var i = 0;i < vList.Count; i++) {
                var info  = vList[i];
                if (info.go.layer == 0) continue;

                var w     = info.Transform.szLayer;
                var layer = layerMap[info.go.layer];

                //if (string.IsNullOrEmpty(info.lbLayer)) continue;
                if (w <= 0) {
                    w = style.CalcSize(new GUIContent(layer.title)).x;
                    info.Transform.szLayer = w;
                }

                if (maxWidth < w) maxWidth = w; 
            }
        }

        internal static void RefreshCache() {
            if (layerMap == null) {
                layerMap = new Dictionary<int, h2LayerInfo>();
            } else {
                layerMap.Clear();
            }
            var cLayer = h2Settings.color_Layers;
            for (var i = 0; i < 32; i++) {
                layerMap.Add(i, new h2LayerInfo() {
                    title   = "L." + (i+1),
                    bg      = cLayer[i % cLayer.Length].xGetTexture2D()
                });
            }

            var layers = (string[])(("UnityEditorInternal.InternalEditorUtility").xGetTypeByName().GetProperty("layers").GetValue(null, null));
            foreach (string layerName in layers) {
                layerMap[LayerMask.NameToLayer(layerName)].title = layerName;
            }
        }

        static internal void Draw(h2Info info, Rect r, GameObject go) {
            if (go.layer == 0) return; //ignore default layer

            if (layerMap == null) RefreshCache();

            if (Event.current.type == EventType.Repaint) {
	            var layer = layerMap[go.layer];
	            var d = r.width-info.Transform.szLayer;
	            r.xMin += d;
	            
                GUI.DrawTexture(r, layer.bg);
                GUI.Label(r, layer.title, EditorStyles.miniLabel);
            }
        }
    } 
}
