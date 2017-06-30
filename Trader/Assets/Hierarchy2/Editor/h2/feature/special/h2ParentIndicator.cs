using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using h2;

namespace h2 {
	internal class h2ParentIndicator {
        private static h2ParentIndicator _api;
	    //private static Dictionary<int, float> _yMap;
        //Cache
        private Color c;
        private List<int> iList; //prevent allocating
        private GameObject multiSceneRoot;

        private List<GameObject> parents;
        private GameObject selected;
        private float selectedY;
        private Texture2D[] texList; // tr, vt, hz
        internal static h2ParentIndicator Api { get { return _api ?? (_api = new h2ParentIndicator()); } }
        private void DrawLine(int from, int to, List<h2Info> list, int lv, bool capEnd, bool excludeFrom)
        {
            const float w =
#if UNITY_4_5 || UNITY_4_6 || UNITY_5
                14f;
#else
        16f;
#endif
            var x = w * (lv - 1);
            var dy = excludeFrom ? 16f : 0f;
            var h = 16f * (to - from + 1) - dy - (capEnd ? 16f : 0f);

            var fRect = list[from].drawRect.x(x).w(16f);
            var tRect = list[to].drawRect.x(x).w(16f);
            var lRect = fRect.dy(dy).h(h);

            if (h > 0)
            {
                GUI.DrawTexture(lRect, texList[2]);
            }
            if (capEnd)
            {
                GUI.DrawTexture(tRect, texList[0]);
            }

            //Debug.LogWarning("DrawLine=" + from + ":" + to + ":" + lRect + ":" + tRect);
        }

        private Texture2D hlTexture;
        private void Highlight(Rect r) {
	        if (hlTexture == null) {
		        hlTexture = TextureX.xGetTexture2D(h2Color.Get(h2ColorType.ParentHL));
            }

            var rr = r;
            //rr.y += 8f;
            //rr.height = 1;
            rr.yMin += 15f;

            GUI.DrawTexture(rr, hlTexture);
        }
        internal void Check()
        {
            if (selected != Selection.activeGameObject && Selection.activeGameObject != null)
            {
                //if (_yMap == null) _yMap = new Dictionary<int, float>();
                selected = Selection.activeGameObject;
                parents = selected.xGetParents(true, parents);

                //for (var i = 0;i < parents.Count; i++){
                //    Debug.Log(i + ":" + parents[i]);
                //}

                if (parents != null && parents.Count > 0) {
                    //selectedY = -1; 
	                multiSceneRoot = h2Extern.IsMultiScene(parents[0]) ? parents[0] : null;
                    if (multiSceneRoot != null) {
                        parents.RemoveAt(0);
                    }
                }
            }

            if (selected == null)
            {
                Clear();
            }
        }
        internal void Clear()
        {
            //Debug.Log("Selection is null ... ");
            /*if (parents != null)
            {
                parents.Clear();
            }
            if (iList != null)
            {
                iList.Clear();
            }*/
            //multiSceneRoot = null;
        }
        internal void Refresh() {
            selected = null;
            Check();
        }
        internal void Draw(List<h2Info> vList)
        {
            Check();
            if (parents == null || parents.Count == 0 || vList == null || vList.Count == 0) {
                return;
            }

            if (texList == null) {
                c = h2Color.Get(h2ColorType.ParentLine);
                texList = new[] {
                    EditorResource.GetTexture2D("corner_tr"),
                    EditorResource.GetTexture2D("line_hz"),
                    EditorResource.GetTexture2D("line_vt")
                };
            }

            //for (var i = 0; i < vList.Count; i++) {
            //    if (!_yMap.ContainsKey(vList[i].instID)) {
            //        _yMap.Add(vList[i].instID, vList[i].drawRect.y);
            //    } else {
            //        _yMap[vList[i].instID] = vList[i].drawRect.y;
            //    }
            //}


            if (iList == null)
            {
                iList = new List<int>();
            }
            else
            {
                iList.Clear();
            }

            var l = parents.Count;
            var fn = -1;
            var max = -1;

            //Debug.Log("---------------------------------------------------- " + Event.current.type + ":" + vList.Count);

            for (var i = 0; i < vList.Count; i++)
            {
                var info = vList[i];
                if (info == null || info.go == null) continue;

                var count = info.Transform.parentCount;
                if (count > l)
                {
                    continue;
                }

                //Debug.Log(i + ":" + info.go);

                if (count <= max)
                { //back out !
                  //Debug.Log("Back out ... " + i + ":" + max + ":" + count);
                    fn = max;
                    break;
                }

                if (count == l)
                {
                    if (info.go == selected)
                    {
                        selectedY = info.drawRect.y;
                        fn = i;
                        break;
                    }
                    continue;
                }

                if (info.go == parents[count])
                {
                    max = count;
                    iList.Add(i);

                    if (h2Settings.DrawParentHighlight) Highlight(info.drawRect);
                }
            }

            if (fn == -1 && iList.Count == 0)
            { //check for crossing
	            var cgo = vList[0].go;
	            
	            
	            if (cgo != null) {
	            	var pList = cgo.xGetParents(true);
		            var pLevel = -1;
	            	var min = Mathf.Min(pList == null ? 0 : pList.Count, parents.Count) - 1;
	            	
	            	for (var i = min; i >= 0; i--) {
		            	if (pList[i] == parents[i]) {
			            	pLevel = i;
			            	break;
		            	}
	            	}
	            	
	            	if (pLevel != -1) { // crossing 
		            	if (selectedY == 0f || selectedY > vList[0].drawRect.y)
		            	{
			            	using (GuiX.GUIColor(c))
			            	{
				            	DrawLine(0, vList.Count - 1, vList, pLevel + 1, false, false);
			            	}
		            	}
	            	}
	            }
	            
                //Debug.Log("Crossing ... " + pLevel + ":" + selectedY + ":" + vList[0].drawRect.y);
            }
            else
            {
                var st = iList.Count > 0 ? iList[0] : fn;
                var ed = iList.Count > 0 ? iList[iList.Count - 1] : fn;

                var stLv = vList[st].Transform.parentCount;
                var edLv = vList[ed].Transform.parentCount;

                //Debug.Log(st + ":" + ed + ":" + fn);

                using (GuiX.GUIColor(c))
                {
                    if (stLv > 0)
                    { // draw from top
                      //Debug.Log("Top ----> " + stLv + ":" + Event.current);
                        DrawLine(0, st, vList, stLv, true, false);
                    }

                    if (fn == -1)
                    { // draw till end
                      //Debug.Log("End ----> " + edLv);
                        DrawLine(ed, vList.Count - 1, vList, edLv + 1, false, true);
                    }
                    else if (fn != max)
                    {
                        iList.Add(fn);
                    }

                    if (iList.Count > 1)
                    {
                        for (var i = 0; i < iList.Count - 1; i++)
                        {
                            //Debug.Log("MID ----> " + vList[iList[i + 1]].go + ":" + iList[i+1]);
                            DrawLine(iList[i], iList[i + 1], vList, vList[iList[i + 1]].Transform.parentCount, true, true);
                        }
                    }
                }
            }
        }
    }
}