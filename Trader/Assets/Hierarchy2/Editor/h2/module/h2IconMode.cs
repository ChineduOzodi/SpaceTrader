using UnityEngine;

namespace vietlabs {
    internal class h2IconMode {
        internal static bool[] _iconModes;
        internal static int currentIconMode;

        internal static float DrawIcons(h2Info info) {
            var offset = currentIconMode * (h2Settings.nIcons + 1);
            if (_iconModes == null) h2LoadIconModes();

            var r   = info.drawRect;
            var go  = info.go;

            //Debug.Log("Draw :: " + Event.current.type + ":" + _iconModes[offset + 7]);
            //if (_iconModes[offset + 7]) h2Tag.Draw(info, r.xSubRight(out r, h2Tag.MaxWidth), go);
            if (_iconModes[offset + 0]) h2Script.Draw(info, r.xSubRight(out r, 6f), go);
            if (_iconModes[offset + 1]) h2Lock.Draw(info, r.xSubRight(out r, 16f), go);
            if (_iconModes[offset + 2]) h2Active.Draw(info, r.xSubRight(out r, 16f, 4f), go);
            if (_iconModes[offset + 3]) h2Static.Draw(info, r.xSubRight(out r, 16f), go);
            if (_iconModes[offset + 4]) h2Combine.Draw(info, r.xSubRight(out r, h2Combine.MaxWidth), go);
            if (_iconModes[offset + 8]) h2Depth.Draw(info, r.xSubRight(out r, h2Depth.maxWidth), go);
            if (_iconModes[offset + 5]) h2Prefab.Draw(info, r.xSubRight(out r, 14f), go); 
            if (_iconModes[offset + 6]) h2Layer.Draw(info, r.xSubRight(out r, h2Layer.maxWidth), go);
            if (_iconModes[offset + 7]) h2Tag.Draw(info, r.xSubRight(out r, h2Tag.MaxWidth), go);

            h2Component.Draw(info, r.xSubRight(out r, h2Component.MaxWidth), go);
            //h2SelectionHL.Draw(info, r.xSubRight(out r, 16f), go);

            return info.drawRect.width - r.width;
        }

        private static void h2LoadIconModes() {
            var modes   = h2Settings.iconModes;
            _iconModes  = new bool[modes.Length];
            currentIconMode = h2Settings.currentMode;

            for (var i = 0;i < modes.Length; i++) {
                _iconModes[i] = modes[i] == '1';
            }
        }

        internal static bool h2CheckIconMode() {
            var e = Event.current;
            if (e.type != EventType.ScrollWheel || !e.shift) return false;

            e.Use();
            currentIconMode = (currentIconMode + (e.delta.y > 0 ? 1 : -1) + h2Settings.nModes) % h2Settings.nModes;
            h2Settings.currentMode = currentIconMode;
            //Debug.Log("Current mode : " +  currentIconMode);
            return true;
        }
    }
}
