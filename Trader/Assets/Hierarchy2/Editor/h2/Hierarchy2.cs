/*
------------------------------------------------
    Hierarchy2 for Unity3d by VietLabs
------------------------------------------------
    version : 1.4.0 beta 5
    release : 12 July 2015
    require : Unity3d 4.3+
    website : http://vietlabs.net/hierarchy2
--------------------------------------------------

Powerful extension to add the most demanding features
to Hierarchy panel that fully integrated into Unity Editor 

--------------------------------------------------
*/

using System;
using UnityEditor;
using UnityEngine;
using h2;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace vietlabs {

    [InitializeOnLoad]
    class Hierarchy2 {

	    private static EditorWindow h2;
        private static Type HDType;

        static Hierarchy2() {
            HDType = "vietlabs.HierarchyDraw".xGetTypeByName(true);

            if (HDType != null) {
                var addFunc = HDType.GetMethod("Add");
                addFunc.Invoke(null, new object[] { "Hierarchy2", (Func<int, Rect, Rect>) OnHierarchyDraw});

                var priorFunc = HDType.GetMethod("SetPriority");
                priorFunc.Invoke(null, new object[] { "Hierarchy2", -1 });

                //var enableFunc = HDType.GetMethod("SetEnable");
                //enableFunc.Invoke(null, new object[] { "HDrawSample1", false });
                //enableFunc.Invoke(null, new object[] { "HDrawSample2", false });
            } else {
                EditorApplication.hierarchyWindowItemOnGUI -= HierarchyItemCB;
                EditorApplication.hierarchyWindowItemOnGUI += HierarchyItemCB;
            }

            h2Transform.Init();
            h2Component.Init();
            h2SceneViewHL.Init();
            h2Prefab.Init();
            h2Goto.Init();
            h2Camera.Init();
            h2GameObjectHL.Init();

            Undo.undoRedoPerformed  += h2Utils.ForceRefreshHierarchy;
            //EditorApplication.hierarchyWindowChanged    -= OnHierarchyChange;
            //EditorApplication.hierarchyWindowChanged    += OnHierarchyChange;
            EditorApplication.update -= FirstUpdate;
            EditorApplication.update += FirstUpdate;

            EditorApplication.playmodeStateChanged += OnPlayModeState;
        }

        static void OnPlayModeState() {
            //Debug.LogWarning("OnPlayModeState : " + EditorApplication.isPlayingOrWillChangePlaymode);

            h2Info.ClearVList();
            WindowX.Hierarchy.Repaint(); 
        }

        static void FirstUpdate() {
            if (h2Settings.useSceneViewHL) h2SceneViewHL.StartMonitor();
            EditorApplication.update -= FirstUpdate; 
        }

        //static void OnHierarchyChange() {
        //    EditorX.xDelayCall(DelayRefresh);
        //}

        //static bool DelayRefresh() {
        //    h2ParentIndicator.Api.Refresh();
        //    h2GameObjectHL.Refresh();
        //    return false;
        //}

        /******************************************
            CALLBACKS
        ******************************************/

	    static void OnVListReady() {
		    if (h2Info.vList == null){
		    	Debug.LogWarning("Should never be here ! " + Event.current);
		    	return;
		    }
		    
            //re-check vList
            if (h2Info.vList.RemoveAll(item => item == null || item.go == null) > 0) {
                WindowX.Hierarchy.Repaint();
            }

            h2Info.SelectedInstIDs = Selection.instanceIDs;
            h2Info.SelectionCount = h2Info.SelectedInstIDs.Length;
            h2Info.SelectedGameObjects = Selection.gameObjects;

            h2Tag.CalculateSize();
            h2Layer.CalculateSize();
            h2Depth.CalculateMinMaxDepth();

            checkInspectorLock();
        }

        static void OnBeforeEvent() { //call once for each event, before any GO can process it
            if (EditorWindow.focusedWindow == WindowX.Inspector) {
                checkInspectorLock();
            }

            h2Shortcut.Check();
            h2IconMode.h2CheckIconMode();
            //OnBeforeDraw(r);

            //Debug.Log("Redraw ... " + Event.current);

            /*if (Event.current.type == EventType.repaint || Event.current.type == EventType.Ignore) {
                var arr = h2Info2.vList;
                for (var i = 0; i < arr.Count; i++) {
                    h2GameObjectHL.Draw(arr[i], arr[i].drawRect, arr[i].go);
                }
            }*/
        }

        static void OnDrawGO(h2Info info, Rect r) {
            if (info == null || info.go == null) {
                //info / go can be null if GameObjects being destroyed
                return;
            }

            info.drawRect = r.xOffset(h2Settings.iconOffset);//thirdPartyOffset +
            if (Event.current.type == EventType.Repaint) {
                if (_inspectorLocked == info.instID) {
                    r.xDrawBar(null, Color.red.xAlpha(0.2f));
                }
            }
	        //AddTrackPoint("CalculateRect");
	        h2Context.Draw(info, r, info.go);
	        //AddTrackPoint("DrawContext");
	        h2GameObjectHL.Draw(info, r, info.go);
	        //AddTrackPoint("DrawGOHL");
            _width = Mathf.Max(_width, h2IconMode.DrawIcons(info));
	        //AddTrackPoint("Draw Icons");

            //var r = info.drawRect;
            //var go = info.go;

            //h2Layer.Draw(r.xSubRight(out r, 40f), go);
            //h2Tag.Draw(info, r.xSubRight(out r, 70f));
            //h2Prefab.Draw(info, r.xSubRight(out r, 16f));

            //still long way to go, man ....

            //customscript icons
            //explorer & highlight objects collection

            //quickFind
            //quickInfo
            // Camera snapshot

            //PrefabEvo
            //MultiScene
            //FavoriteTabs
            //uScript
            //BehaviorDesigner
        }

        static void OnAfterEvent() { //call once foreach Event, after all GO processed
			if (!string.IsNullOrEmpty(WindowX.Hierarchy.xGetSearchFilterTerm())) return;

            if (Event.current.type == EventType.Repaint || Event.current.type == EventType.Ignore) {
                //h2Info.Check_Cache();
                h2ParentIndicator.Api.Draw(h2Info.vList);
            }
        }

        static void checkInspectorLock() {
            var inspector       = WindowX.Inspector;
            var isLocked        = (bool)inspector.xGetProperty("isLocked");
            _inspectorLocked    = -1;

            if (isLocked) {
                var list = (Editor[])WindowX.Inspector.xGetField("m_Tracker").xGetProperty("activeEditors");
                if (list.Length > 0) {
                    for (var i = 0;i < list.Length; i++) {
                        if (list[i].target != null) {
                            _inspectorLocked = list[i].target.GetInstanceID();
                            return;
                        }
                    }
                }
            }
        }

        static bool h2IsEnabled() {
            var e = Event.current;
            if (e.type != EventType.keyUp || !e.alt || !e.control || !e.shift || e.keyCode != KeyCode.Alpha0) return h2Settings.enable;

            h2Settings.enable = !h2Settings.enable;
            if (h2Settings.useSceneViewHL) {
                if (h2Settings.enable) {
                    h2SceneViewHL.StartMonitor();
                } else {
                    h2SceneViewHL.StopMonitor();
                }
            }
            
            e.Use();

            return h2Settings.enable;
        }

        /******************************************
            CACHE
        ******************************************/

        private static EventType _lastEventType;
        private static int _firstID;
        private static int _lastID;
        private static bool _vListReady;
        private static int _inspectorLocked;
        private static float _width;

        static public int inspectorLock {
            get { return _inspectorLocked; }
        }

        static Rect OnHierarchyDraw(int instID, Rect r) {
            HierarchyItemCB(instID, r);
            return new Rect(r.x, r.y, r.width-_width, r.height);
        }

	    private static float timeSt;
	    private static int timeCount;
	    private static StringBuilder timeList;
	    
	    static void StartTracking(){
	    	timeCount = 0;
	    	timeList = new StringBuilder();
		    timeSt = Time.realtimeSinceStartup * 1000;
	    }
	    
	    static void AddTrackPoint(string point){
	    	var t = Time.realtimeSinceStartup * 1000;
	    	if (t-timeSt > 2) {
	    		timeCount++;
	    		timeList.Append(point + ":" + Mathf.Round(t-timeSt) + "\n");
	    	}
	    	timeSt = t;
	    }
	    
	    static void HierarchyItemCB(int instID, Rect r) {
	    	//StartTracking();
	    	
		    if (!h2IsEnabled()) return;
		    //AddTrackPoint("CheckEnable");
		    
            var isLayout = Event.current.type == EventType.Layout; 

            if (isLayout) {
                if (_lastEventType != EventType.Layout && 
                    _lastEventType != EventType.MouseMove &&
                    _lastEventType != EventType.Repaint)
                {
                    _lastEventType = EventType.Layout;
                    _firstID = instID;
                    _vListReady = false;
                    //h2Info.Clear_vList(); 
                    h2Info.ClearVList();
                }

                _lastID = instID;

                //read to get correct vList order
                //h2Info.Get(instID);
                h2Info.AddToVList(instID);
                return; //do nothing on Layout event (as we only use GUI)
            }

			if (h2Info.vList == null) {
				// sometimes the used event trigger before the layout event and that prevent vlist to be prepare correctly
				// just skip as layout will be calling anyway
				return;
			}
		    //AddTrackPoint("CheckLayout");
		    
            if (!_vListReady) {
                _vListReady = true;
                OnVListReady();
            }
		    
		    //AddTrackPoint("vListReady");

            //call once foreach Event, before draw any vList item
            if (_firstID == instID) OnBeforeEvent();
		    
		    //AddTrackPoint("OnBeforeEvent");
		    
            //instID might not in vList :(
            OnDrawGO(h2Info.Get(instID, true), r);
		    //AddTrackPoint("OnDrawGO");
		    
            if (instID == _lastID) OnAfterEvent();
		    _lastEventType = Event.current.type;
		    
		    //AddTrackPoint("OnAfterEvent");
		    //if (timeCount > 0) Debug.Log(Event.current + ":" + timeList.ToString());
        }

        //----------------------------------  DRAW ICONS   ------------------------------
    }

}

