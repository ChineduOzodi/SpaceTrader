using UnityEditor;
using UnityEngine;

namespace vietlabs {

    /*internal class h2CameraInfo {
        public bool orthor;
        public Vector3 mPosition;
        public Quaternion mRotation;
    }*/

    internal class h2Camera {
        private SceneView _sceneView; 

        //camera being looked through & its saved information
        private Camera      lt_camera;   
        public bool         lt_orthor;
        public Vector3      lt_mPosition;
        public Quaternion   lt_mRotation;

        private SceneView sceneView {
            get {
                if (_sceneView != null) return _sceneView;
                //if (EditorWindow.focusedWindow != null && EditorWindow.focusedWindow.GetType() == typeof (SceneView)) return _sceneView = (SceneView) EditorWindow.focusedWindow;
                return _sceneView = SceneView.lastActiveSceneView ?? (SceneView) SceneView.sceneViews[0];
            }
        }
        private Camera sceneCamera { get { return sceneView.camera; }}

        public void cmdLookThrough() {
            if (Selection.activeGameObject == null) return;
            var cam = Selection.activeGameObject.GetComponent<Camera>();
            if (cam == null) return;
            LookThrough(cam);
        }
        public void cmdCaptureSV() {
            if (Selection.activeGameObject == null) return;
            var cam = Selection.activeGameObject.GetComponent<Camera>();
            if (cam == null) return;
            CaptureSceneView(cam);
        }


        void Refresh() { //hacky way to force SceneView increase drawing frame
            var t = Selection.activeTransform
                    ?? ((Camera.main != null) ? Camera.main.transform : new GameObject("$t3mp$").transform);

            var op = t.position;
            t.position += new Vector3(1, 1, 1); //make some dirty
            t.position = op;

            if (t.name == "$t3mp$") Object.DestroyImmediate(t.gameObject, true);
        }
        /*public h2CameraInfo info {
            get {
                return new h2CameraInfo() {
                    orthor      = orthographic,
                    mRotation   = m_Rotation,
                    mPosition   = m_Position
                };
            }
        }*/
        public void CopyTo(Camera c) { c.CopyFrom(sceneCamera); }
        public void CopyFrom(Camera cam) {
            sceneCamera.CopyFrom(cam);

            orthographic    = cam.orthographic;
            m_Rotation      = cam.transform.rotation;
            m_Position      = cam.transform.position - (cam.transform.rotation * new Vector3(0,0,-cameraDistance));
            Refresh();
        }

        
        private T GetAnimT<T>(string name) {
            if (sceneView == null) return default(T);

            var animT = sceneView.xGetField(name);

            return (T)
#if UNITY_4_5 || UNITY_4_6 || UNITY_5
            animT.xGetProperty("target");
#else
	        animT.xGetField("m_Value");
#endif
        }

        private void SetAnimT<T>(string name, T value) {
            if (sceneView == null) return;

            var animT = sceneView.xGetField(name);
    #if UNITY_4_5 || UNITY_4_6 || UNITY_5
            animT.xSetProperty("target", value);
    #else
	        animT.xInvoke("BeginAnimating", null, null, value, animT.xGetField("m_Value"));
    #endif
        }


        private Vector3 m_Position {
            get { return GetAnimT<Vector3>("m_Position"); }
            set { SetAnimT("m_Position", value.xFixNaN()); }
        }
        private Quaternion m_Rotation {
            get { return GetAnimT<Quaternion>("m_Rotation"); }
            set { SetAnimT("m_Rotation", value); }
        }
        private float cameraDistance {
            get { return (float) sceneView.xGetProperty("cameraDistance"); }
        }
        private bool orthographic {
            get { return sceneView.camera.orthographic; }
            set {
                //sceneView.camera.orthographic = value;
    #if UNITY_4_5 || UNITY_4_6 || UNITY_5
                SetAnimT("m_Ortho", value);
    #else
                SetAnimT("m_Ortho", value ? 1f : 0f);
    #endif
            }
        }
        /*private float m_Size {
            get { return GetAnimT<float>("m_Size"); }
            set { SetAnimT("m_Size", (float.IsInfinity(value) || (float.IsNaN(value)) || value == 0f) ? 100f : value); }
        }*/


        public void CaptureSceneView(Camera cam) {
            lt_camera   = null;

            cam.xRecordUndo("Capture Scene Camera");
            cam.transform.xRecordUndo("Capture Scene Camera");
            cam.CopyFrom(sceneCamera);
        }
        public void LookThrough(Camera cam) {

            if (lt_camera == null) { //save current state of scene-camera
                lt_orthor     = orthographic;
                lt_mPosition  = m_Position;
                lt_mRotation  = m_Rotation;
            }

            if (cam != null) {
                CopyFrom(cam);

                if (Application.isPlaying) {
                    EditorApplication.update -= UpdateLookThrough;
                    EditorApplication.update += UpdateLookThrough;
                }
            } else if (lt_camera != null) { //restore old state of scene-camera
                orthographic    = lt_orthor;
                m_Position      = lt_mPosition;
                m_Rotation      = lt_mRotation;
            }

            lt_camera = cam;
        }
        void UpdateLookThrough() {
            if (lt_camera == null) {
                EditorApplication.update -= UpdateLookThrough;
                return;
            }

            if (EditorApplication.isPaused) return;

            if (lt_camera.transform.position != sceneCamera.transform.position ||
                lt_camera.transform.rotation != sceneCamera.transform.rotation ||
                lt_camera.orthographic != sceneCamera.orthographic)
            {
                CopyFrom(lt_camera);
            }
        }


        private static h2Camera _api;
        internal static h2Camera Api { get { return _api ?? (_api = new h2Camera()); } }


        static public void Init() {
            h2Shortcut.Add(h2Shortcut.CAMERA_LOOKTHROUGH, Api.cmdLookThrough);
            h2Shortcut.Add(h2Shortcut.CAMERA_CAPTURE_SCENEVIEW, Api.cmdCaptureSV);
        }
    }
}


