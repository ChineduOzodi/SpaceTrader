using UnityEngine;

namespace vietlabs
{
    internal class h2Depth {
        
        internal static int maxWidth;
        static int min;

        private const int DepthBarWidth = 3;

        static Color[] depthColors;
        static Texture2D[] depthTex;

        internal static void CalculateMinMaxDepth() {
            var vlist = h2Info.vList;
            if (vlist == null) return;

            min = 100;
            var max = 0;
            var nDepth = 0;

            for (var i = 0;i < vlist.Count; i++) {
                if (vlist[i] == null || vlist[i].go == null) continue;

                var c = vlist[i].Transform.parentCount;
                if (c < min) min = c;
                if (c > max) max = c;
            }

            nDepth = max - min+1;
            maxWidth = nDepth * DepthBarWidth;
        }

        internal static void Draw(h2Info info, Rect r, GameObject go) { 
            if (depthColors == null) {
                depthColors = h2Settings.color_Layers;
                depthTex = new Texture2D[depthColors.Length];
                return;
            }

            var tInfo = info.Transform;

            if (Event.current.type == EventType.Repaint) {
                var l = depthColors.Length;
                var idx = tInfo.parentCount % l;
                var tex = depthTex[idx];
                if (tex == null) {
                    tex = depthColors[idx].xGetTexture2D();
                    depthTex[idx] = tex;
                }

                GUI.DrawTexture(r.dx(maxWidth-(tInfo.parentCount - min + 1)* DepthBarWidth).w(DepthBarWidth), tex);
            }
            
            /* - (HLFull ? 1 : 0)#1#;

            if (c < 0) return; //don't highlight level 0 on full mode

            

            /*if (HLFull) {
                r.width = r.x + r.width;
                r.x = 0;
            } else {#1#
                var w = c*DepthBarWidth + 4f;
                r.x = r.x + r.width - w;
                r.width = DepthBarWidth;
            /*}#1#

            //Debug.Log("settings.h2_depthColors="+ settings.h2_depthColors);
            var depthColors = h2Settings.color_Depths;
            GUI.DrawTexture(r, depthColors[c % depthColors.Length].xGetTexture2D());
                //.Alpha(0.5f).Adjust(0.3f)*/


        }
    }
}
