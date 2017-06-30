using System;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;

namespace vietlabs {
    internal class h2Context {
        static public GameObject[] Targets;
        static public GenericMenu Get(params h2Info[] infos) { //selected GOs
            var menu = new GenericMenu();
            const string cat1 = "Transform/";
            //var cat2 = string.Empty;// "Advanced/";
            const string cat3 = "Setting/";

	        h2SceneViewHL.AttachMenu(menu, cat3);
	        menu.xAdd("Ping current scene", PingCurrentScene);
	        menu.xAdd("Show scene references", ShowSceneReference);
            menu.xAddSep(string.Empty);

            GotoMenu(menu, cat1).xAddSep(cat1);
            TransformMenu(menu, cat1).xAddSep(string.Empty);

#if UNITY_4_3
			Isolate43Menu(menu, "Isolate/");
#endif

            h2GameObjectHL.Menu(menu, "Highlight/");

            //AdvancedMenu(menu, cat2);

            return menu;
        }
	    
	    
	    static void ShowSceneReference(){
	    	//var t = Time.realtimeSinceStartup;
	    	h2SceneReference.ScanHierarchy();
	    	//Debug.Log("Finish in " + (Time.realtimeSinceStartup - t));
	    	h2SceneReference.LogReferences(Selection.activeGameObject);
	    }
	    
	    static void PingCurrentScene(){
	    	var path = EditorApplication.currentScene;
	    	var sceneObj = AssetDatabase.LoadAssetAtPath(path, typeof(Object));
	    	EditorGUIUtility.PingObject(sceneObj);
	    	
	    }

        static public void Draw(h2Info info, Rect r, GameObject go) {
            if (info.drawRect.xRMB_isDown().with_Ctrl) {
                if (Selection.activeGameObject != go) {
                    Selection.activeGameObject = go;
                }

                Targets = h2Info.SelectedGameObjects;
                Get(info).ShowAsContext();
            }
        }

        //static Action noAction = null;

        static GenericMenu TransformMenu(GenericMenu menu, string prefix = null) {
           /* bool rp = true;
            bool rr = true;
            bool rs = true;

            if (h2Info2.SelectionCount == 0) {
                var t = Selection.activeGameObject.transform;
                rp = t.localPosition != Vector3.zero;
                rr = t.localRotation != Quaternion.identity;
                rs = t.localScale != Vector3.one;
            }*/

            if (prefix == null) prefix = string.Empty;

            menu.xAdd(prefix + "✪  CREATE", null, true)
                .xAddSep(prefix)
                .xAdd(prefix + "Empty Child " + h2Shortcut.NEW_CHILD, h2Transform.cmdNewChild)
                .xAdd(prefix + "Empty Parent " + h2Shortcut.NEW_PARENT, h2Transform.cmdNewParent)
                .xAdd(prefix + "Empty Sibling " + h2Shortcut.NEW_SIBLING,  h2Transform.cmdNewSibling)
                .xAddSep(prefix)
                .xAdd(prefix + "✪  RESET", null, true)
                .xAddSep(prefix)
                .xAdd(prefix + "Local Position " + h2Shortcut.RESET_LOCAL_POSITION, h2Transform.cmdResetPosition)
                .xAdd(prefix + "Local Rotation " + h2Shortcut.RESET_LOCAL_ROTATION, h2Transform.cmdResetRotation)
                .xAdd(prefix + "Local Scale " + h2Shortcut.RESET_LOCAL_SCALE, h2Transform.cmdResetScale)
                .xAddSep(prefix)
                .xAdd(prefix + "Transform " + h2Shortcut.RESET_LOCAL_TRANSFORM, h2Transform.cmdResetTransform)
                //.xAddSep(prefix)
                //.xAdd(prefix + "✪  RESET (FREEZE CHILDREN)", null, true)
                //.xAddSep(prefix)
                //.xAdd(prefix + "Local Position " + h2Shortcut.FREEZE_CHILDREN_RESET_LOCAL_POSITION, rp ? h2Transform.cmdResetPosition : noAction)
                //.xAdd(prefix + "Local Rotation " + h2Shortcut.FREEZE_CHILDREN_RESET_LOCAL_ROTATION, rr ? h2Transform.cmdResetRotation : noAction)
                //.xAdd(prefix + "Local Scale " + h2Shortcut.FREEZE_CHILDREN_RESET_LOCAL_SCALE, rs ? h2Transform.cmdResetScale : noAction)
                //.xAddSep(prefix)
                //.xAdd(prefix + "Local Transform " + h2Shortcut.FREEZE_CHILDREN_RESET_LOCAL_TRANSFORM, (rp || rr || rs) ? h2Transform.cmdResetTransform : noAction)
                ;
            return menu;
        }

        static GenericMenu GotoMenu(GenericMenu menu, string prefix = null) {
            if (prefix == null) prefix = string.Empty;

            menu.xAdd(prefix + "✪  GO TO", null)
                .xAddSep(prefix)
                .xAdd(prefix + "Parent " + h2Shortcut.GOTO_PARENT, h2Goto.cmdPingParent)
                .xAdd(prefix + "Child " + h2Shortcut.GOTO_CHILD, h2Goto.cmdPingChild)
                .xAdd(prefix + "Next Sibling " + h2Shortcut.GOTO_SIBLING, h2Goto.cmdNextSibling);
            return menu;
        }

        /*static GenericMenu AdvancedMenu(GenericMenu menu, string prefix = null) {
            bool rp = true;
            bool rr = true;
            bool rs = true;

            if (Selection.gameObjects.Length == 0) {
                var t = Selection.gameObjects[0].transform;
                rp = t.localPosition != Vector3.zero;
                rr = t.localRotation != Quaternion.identity;
                rs = t.localScale != Vector3.one;
            }

            if (prefix == null) prefix = string.Empty;
            return menu;
        }*/


#if UNITY_4_3
		static GenericMenu Isolate43Menu(GenericMenu menu, string prefix = null){
			if (prefix == null) prefix = string.Empty;
			var type = "UnityEditorInternal.InternalEditorUtility".xGetTypeByName("UnityEditor");

			//append layers & tags
			var layerPrefix = prefix + "Layer/";
			var layers	= (string[])(type.xGetProperty("layers"));
			for (var i = 0; i < layers.Length; i++) {
				var layer = layers[i]; // prevent scope change in lambda
				menu.xAdd(layerPrefix + layers[i], ()=>Isolate43.Isolate_Layer(layer));
			}

			var tagPrefix = prefix + "Tag/";
			var tags	= (string[])(type.xGetProperty("tags"));
			for (var i = 0; i < tags.Length; i++) {
				var tag = tags[i];  // prevent scope change in lambda
				menu.xAdd(tagPrefix + tags[i], () => Isolate43.Isolate_Tag(tag));
			}

			return menu;
		}
#endif
    }
}

