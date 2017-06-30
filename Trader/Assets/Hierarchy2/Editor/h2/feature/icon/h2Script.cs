using UnityEngine;
using UnityEditor;

namespace vietlabs {

    
    public class h2Script {

        //private const int NOT_CHECKED   = 0;
        //private const int NO_SCRIPT     = 1;

        //private const int MISSING       = 2;
        //private const int HAS_SCRIPT    = 3;

        private static Texture2D missingTex;
        private static Texture2D scriptTex;
        private static string[] ignoreScriptPaths;
        private static bool useOldDepthStyle = false;

        static internal void Draw(h2Info info, Rect r, GameObject go) {
            if (Event.current.type != EventType.Repaint) return; //only draw on repaint
            var cInfo = info.Component;
            if (cInfo.status == h2cScriptStatus.NO_SCRIPT) return;

            var rr = useOldDepthStyle ? r.w(20f).dx(-17f) : r.w(4f).dx(3f);
            GUI.DrawTexture(rr, (cInfo.status == h2cScriptStatus.MISSING) ?
                missingTex  ?? (missingTex = ColorHSL.red.xProSkinAdjust(0.1f).xGetTexture2D()) :
                scriptTex   ?? (scriptTex = ColorHSL.green.xProSkinAdjust(0.1f).xGetTexture2D())
            );
        }
    }
}

