using System;
using System.Collections.Generic;
using UnityEngine;
using Debug = UnityEngine.Debug;


namespace vietlabs {
    internal class h2Shortcut {
        static public KBShortcut NO_SHORTCUT                    = new KBShortcut { code = KeyCode.None };

        static public KBShortcut GOTO_PARENT                    = new KBShortcut { code = KeyCode.LeftBracket };
        static public KBShortcut GOTO_CHILD                     = new KBShortcut { code = KeyCode.RightBracket };
        static public KBShortcut GOTO_SIBLING                   = new KBShortcut { code = KeyCode.Slash };

        static public KBShortcut NEW_PARENT                     = new KBShortcut { shift = true, chain = KeyCode.N, code = KeyCode.P};
        static public KBShortcut NEW_CHILD                      = new KBShortcut { shift = true, chain = KeyCode.N, code = KeyCode.C };
        static public KBShortcut NEW_SIBLING                    = new KBShortcut { shift = true, chain = KeyCode.N, code = KeyCode.S };
        
        static public KBShortcut RESET_LOCAL_POSITION           = new KBShortcut { shift = true, code = KeyCode.P };
        static public KBShortcut RESET_LOCAL_ROTATION           = new KBShortcut { shift = true, code = KeyCode.R };
        static public KBShortcut RESET_LOCAL_SCALE              = new KBShortcut { shift = true, code = KeyCode.S };
        static public KBShortcut RESET_LOCAL_TRANSFORM          = new KBShortcut { shift = true, code = KeyCode.T };

        static public KBShortcut FREEZE_CHILDREN_RESET_LOCAL_POSITION   = NO_SHORTCUT;
        static public KBShortcut FREEZE_CHILDREN_RESET_LOCAL_ROTATION   = NO_SHORTCUT;
        static public KBShortcut FREEZE_CHILDREN_RESET_LOCAL_SCALE      = NO_SHORTCUT;
        static public KBShortcut FREEZE_CHILDREN_RESET_LOCAL_TRANSFORM  = NO_SHORTCUT;

        static public KBShortcut CAMERA_LOOKTHROUGH            = new KBShortcut { shift = true, code = KeyCode.L };
        static public KBShortcut CAMERA_CAPTURE_SCENEVIEW      = new KBShortcut { shift = true, code = KeyCode.V };

        static public KBShortcut BREAK_PREFAB                  = new KBShortcut { shift = true, code = KeyCode.B };
        
        static public KBShortcut TOGGLE_HIERARCHY2             = new KBShortcut { ctrl = true, alt = true, shift = true, code = KeyCode.Alpha0 };
        static public KBShortcut TOGGLE_SCENEVIEW_HIGHLIGHT    = new KBShortcut { shift = true, code = KeyCode.H };

        static public KBShortcut TOGGLE_ACTIVE                 = new KBShortcut { code = KeyCode.A };

        static public KBShortcut TOGGLE_COMPONENT              = new KBShortcut { shift = true, chain = KeyCode.C, code = KeyCode.C };
        static public KBShortcut TOGGLE_COMPONENT_RENDER       = new KBShortcut { shift = true, chain = KeyCode.C, code = KeyCode.R };
        static public KBShortcut TOGGLE_COMPONENT_PHYSICS      = new KBShortcut { shift = true, chain = KeyCode.C, code = KeyCode.P };
        static public KBShortcut TOGGLE_COMPONENT_UNUSUAL      = new KBShortcut { shift = true, chain = KeyCode.C, code = KeyCode.U };

        static public KBShortcut HIGHLIGHT_GAMEOBJECT_COLOR0   = new KBShortcut { shift = true, code = KeyCode.Alpha0};
        static public KBShortcut HIGHLIGHT_GAMEOBJECT_COLOR1   = new KBShortcut { shift = true, code = KeyCode.Alpha1};
        static public KBShortcut HIGHLIGHT_GAMEOBJECT_COLOR2   = new KBShortcut { shift = true, code = KeyCode.Alpha2};
        static public KBShortcut HIGHLIGHT_GAMEOBJECT_COLOR3   = new KBShortcut { shift = true, code = KeyCode.Alpha3};
        static public KBShortcut HIGHLIGHT_GAMEOBJECT_COLOR4   = new KBShortcut { shift = true, code = KeyCode.Alpha4};
        static public KBShortcut HIGHLIGHT_GAMEOBJECT_COLOR5   = new KBShortcut { shift = true, code = KeyCode.Alpha5};
        static public KBShortcut HIGHLIGHT_GAMEOBJECT_COLOR6   = new KBShortcut { shift = true, code = KeyCode.Alpha6};
        static public KBShortcut HIGHLIGHT_GAMEOBJECT_COLOR7   = new KBShortcut { shift = true, code = KeyCode.Alpha7};
        static public KBShortcut HIGHLIGHT_GAMEOBJECT_COLOR8   = new KBShortcut { shift = true, code = KeyCode.Alpha8};
        static public KBShortcut HIGHLIGHT_GAMEOBJECT_COLOR9   = new KBShortcut { shift = true, code = KeyCode.Alpha9};
        
        static Dictionary<KBShortcut, Action> _keyMap;

        /*public static bool Add(string shortcut, Action act) {
            return Add(KBShortcut.FromString(shortcut), act);
        }*/

        public static bool Add(KBShortcut key, Action act) { 
            if (key.code == KeyCode.None) return false;

            if (_keyMap == null) _keyMap = new Dictionary<KBShortcut, Action>();
            if (_keyMap.ContainsKey(key)) {
                Debug.LogWarning("Duplicated key : " + key + " found, ignoring ...");
                return false;
            }

            _keyMap[key] = act;
            return true;
        }

        static KBShortcut keyChain;

        public static void Check() {
            var e = Event.current;
            if (e.type != EventType.keyDown || e.keyCode == KeyCode.None) return;

            var hasKeyChain = keyChain.code != KeyCode.None && keyChain.ctrl == e.control && keyChain.alt == e.alt && keyChain.shift == e.shift;
            var shortcut = new KBShortcut{
                ctrl    = e.control,
                alt     = e.alt,
                shift   = e.shift,
                chain   = hasKeyChain ? keyChain.code : KeyCode.None,
                code    = e.keyCode
            };

            //if (hasKeyChain) {
            //    Debug.Log("Key chain detected ::: " + shortcut + "--->" + e.keyCode);
            //}

            if (_keyMap.ContainsKey(shortcut)) {
                keyChain.code = KeyCode.None; //clear keyChain
                //Debug.Log("clear chain ::: " + keyChain.code);
                _keyMap[shortcut]();
                return;
            }

            if (hasKeyChain) {
                shortcut.chain = KeyCode.None;

                if (_keyMap.ContainsKey(shortcut)) {
                    keyChain.code = KeyCode.None;
                    _keyMap[shortcut]();
                    //Debug.Log("key chain unmatched :: check normal OK " + shortcut);
                    return;
                }
            }


            if (e.keyCode != KeyCode.None) {
                keyChain = shortcut;
                //Debug.Log("Save chain ::: " + keyChain.code);
            }


        }
    }

    struct KBShortcut {
        public bool ctrl;
        public bool alt;
        public bool shift;
        public KeyCode chain;
        public KeyCode code;

        static public KBShortcut FromString(string str) {
            var result = new KBShortcut();

            if (str[0] == '_') {
                result.code = (KeyCode)Enum.Parse(typeof(KeyCode), str.Substring(1));
            } else {
                var cnt = 0;
                if (str.IndexOf('#') != -1) {
                    result.shift    = true;
                    cnt++;
                }
                if (str.IndexOf('%') != -1) {
                    result.ctrl     = true;
                    cnt++;
                }
                if (str.IndexOf('&') != -1) {
                    result.alt    = true;
                    cnt++;
                }

                result.code = (KeyCode)Enum.Parse(typeof(KeyCode), str.Substring(cnt));
            }

            return result;
        }

        string GetKeyCodeString(KeyCode c) {
            switch (c) {
                case KeyCode.None           : return string.Empty;
                case KeyCode.Slash          : return "\\";
                case KeyCode.LeftBracket    : return "[";
                case KeyCode.RightBracket   : return "]";
                case KeyCode.Alpha0         : return "0";
                case KeyCode.Alpha1         : return "1";
                case KeyCode.Alpha2         : return "2";
                case KeyCode.Alpha3         : return "3";
                case KeyCode.Alpha4         : return "4";
                case KeyCode.Alpha5         : return "5";
                case KeyCode.Alpha6         : return "6";
                case KeyCode.Alpha7         : return "7";
                case KeyCode.Alpha8         : return "8";
                case KeyCode.Alpha9         : return "9";

                default                     : return c.ToString();
            }
        }

        public override string ToString() {
            if (code == KeyCode.None) return string.Empty;
            var modifier = (ctrl ? "#" : "") + (alt ? "&" : "") + (shift ? "#" : "");
            if (modifier == "") modifier = "_";

            var chainStr = GetKeyCodeString(chain);
            if (chainStr != string.Empty) chainStr += "+";

            return modifier +  chainStr + GetKeyCodeString(code);
        }
    }
}

