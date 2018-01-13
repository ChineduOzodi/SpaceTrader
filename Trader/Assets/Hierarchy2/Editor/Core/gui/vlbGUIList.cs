using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace vietlabs {

    public class LIInfo<T> : LIInfo {
        public T data;
    }

	public class LIInfo {
		public bool CanDelete; //show or hide Delete button
		public bool CanDrag; //allow drag this item or not
		public bool? Selected; //null = disable selection
		internal Rect _cRect;
		internal Rect _tRect;

        //internal bool _logChange;

		internal bool IsAnimate { get; private set; }
		internal Rect CurrentRect { get { return _cRect; } }

		private LIInfo CheckAnimate {
			get {
				IsAnimate = _tRect.xIsDifferent(_cRect);
				if (!IsAnimate) { _cRect = _tRect; }
				return this;
			}
		}

		internal LIInfo SetX(float value) {
			_cRect.x = value;
			_tRect.x = value;
			return CheckAnimate;
		}

		internal LIInfo SetY(float value) {
			_cRect.y = value;
			_tRect.y = value;

            //if (_logChange) {
            //    //Debug.Log("SetY : " + _cRect.y + ":" + _tRect.y + ":" + value);
            //}

			return CheckAnimate;
		}

		internal LIInfo SetHeight(float value) {
            //if (value == 0) Debug.LogWarning("setting Height to 0 ... " + _tRect + ":" + _cRect);

            _cRect.height = value;
			_tRect.height = value;
			return CheckAnimate;
		}

		internal LIInfo SetWidth(float value) {
			_cRect.width = value;
			_tRect.width = value;
			return CheckAnimate;
		}

		internal LIInfo SetXY(float x, float y) {
			SetX(x);
			SetY(y);
			return CheckAnimate;
		}

		internal LIInfo TweenX(float value) {
			_tRect.x = value;
			return CheckAnimate;
		}

		internal LIInfo TweenY(float value) {
			_tRect.y = value;
			return CheckAnimate;
		}

		internal LIInfo TweenDY(float value) {
			_tRect.y += value;
			return CheckAnimate;
		}

		internal LIInfo TweenH(float value) {
            //if (value == 0) Debug.LogWarning("tween Height to 0 ... " + _tRect + ":" + _cRect);
            _tRect.height = value;
			return CheckAnimate;
		}

		internal Rect AnimateStep() {
			if (!IsAnimate || Event.current.type != EventType.Repaint) { return _cRect; }
			_cRect = _cRect.xLerp(_tRect, 0.1f);
			return CheckAnimate.CurrentRect;
		}
	}

	public class GUIListTheme {
        public bool AllowScroll;
		public float CellHeight = 20f;
        public float DragWidth = 30f;

		public bool Draggable = true;
		public Texture2D DraggingTex;
		public Texture2D EvenBgTex;
		//public float MaxHeight = 200f;
		public Texture2D OddBgTex;

		public bool Selectable = false;
		public Texture2D SelectedTex;
		public bool ShowDelete = false;
		public bool ShowIndex = false;

		public Rect RectOffset;
	}

	internal class DragData {
		internal int DragIdx = -1;
		internal float DragMouseX;
		internal float DragMouseY;
		internal int DropIdx = -1;

		internal int Idx; //same as DragIdx if isDragging
		internal LIInfo Info;
		internal object Target;

		public bool IsDragging { get { return DragIdx != -1; } }
		public bool IsDropping { get { return DropIdx != -1 && DragIdx == -1; } }
	}


	public class vlbGUIList<T> where T : class {
        //static cache : only use when reorder components to prevent refresh-flickering
        //private static Object _c_idx;
        //private static Rect _c_GuiRect;
        //private static Rect _c_ScrollRect;
        //private static Vector2 _c_ScrollPosition;
        //private static int[] _c_SelectedIndexes;

        
		public List<T> CacheList;
        public IList CoArrays;

        public object UserData;

        public bool NeedRepaint;
		public GUIListTheme Theme;
		internal Dictionary<int, LIInfo<T>> RectMap;

		//temporary vars
		//2DO : cache selection
		internal DragData Drag;
		internal Rect GuiRect;
		internal Object Id;
		internal Vector2 MousePos;
		internal Vector2 ScrollPosition;
		internal Rect ScrollRect;
        

        public T1 GetUserData<T1>() { return (T1)UserData; }

        internal vlbGUIList(List<T> list, GUIListTheme theme = null, Object listId = null) {
			Drag = new DragData();
			RectMap = new Dictionary<int, LIInfo<T>>();
			CacheList = list;

            Theme = theme ?? new GUIListTheme {
				DraggingTex		= ColorHSL.blue.xProSkinAdjust().xAlpha(0.7f).xGetTexture2D(),
                SelectedTex     = ColorHSL.yellow.xProSkinAdjust().xAlpha(0.3f).xGetTexture2D(),
                OddBgTex        = ColorHSL.blue.xProSkinAdjust().xAlpha(0.2f).xGetTexture2D(),
                EvenBgTex       = null,
				RectOffset		= new Rect(0f, 0, 0f, 0)
			}; 

			/*if (_c_idx == listId) {
				GuiRect = _c_GuiRect;
				ScrollRect = _c_ScrollRect;
				ScrollPosition = _c_ScrollPosition;

				if (_c_SelectedIndexes != null) {
					foreach (var idx in _c_SelectedIndexes) {
						if (idx > CacheList.Count) {
							Debug.LogWarning("Cache erorr - trying to select <" + idx + "> in a list of " + CacheList.Count + " items - might be conflicting ids");
							continue;
						}

						var info = GetInfo(CacheList[idx]);
						info.Selected = true;
					}
				}
			}*/

			Id = listId;
		}

		public List<T> SelectedList { get { return CacheList.Where(item => GetInfo(-1, item).Selected == true).ToList(); } }

        public T[] AffectList(T item) {
            var info = GetInfo(-1, item);
            if (!info.Selected.Value) return new [] { item };
            return SelectedList.ToArray();
        }

		//-------------------------------------- INTERNAL UTILS -------------------------------------------

		private Rect MouseDragRect {
			get {
				return new Rect(Event.current.mousePosition.x - Drag.DragMouseX,
								Event.current.mousePosition.y - Drag.DragMouseY,
								GuiRect.width,
								Drag.Info.CurrentRect.height);
			}
		}

	    

		internal void Draw(Func<Rect, T, int, int> onDrawCell,
		Action<int, int, T> onReorder = null,
		Action<T, int, vlbGUIList<T>> OnRightClick = null,
        Action<int, int, T> onBeforeReorder = null,
        Rect? drawRect = null) {

			NeedRepaint = false;
			const float scrollW = 16f;

		    var rect        = drawRect ?? (Theme.AllowScroll ? GuiX.FlexibleSpace() : GuiX.Height(GuiRect.height));
			var hasScroll	= (rect.height>2f) && (GuiRect.height > rect.height);
			var counter		= 0;
			var ch			= 0f;

			if (!Event.current.xIsLayout() && Event.current.xIsNotUsed()) {//rect.width > 1f
                rect            = rect.xAdd(Theme.RectOffset);
				GuiRect.x		= rect.x;
				GuiRect.y		= rect.y;
				GuiRect.width	= rect.width;

                MousePos = Event.current.mousePosition;
                //Debug.Log(Event.current + ":" + GuiRect);
            }

			var x = GuiRect.x;
			var y = GuiRect.y;
			var w = GuiRect.width;
			var h = GuiRect.height;

			if (hasScroll) {
				ScrollRect = GuiRect.h(rect.height);
				ScrollPosition = GUI.BeginScrollView(ScrollRect, ScrollPosition, GuiRect.dw(-scrollW), false, true);
			}

            //Debug.Log("----> " + x + ":" + y + ":" + w + ":" + h + ":" + Event.current + ":" + hasScroll);
            //Debug.Log("--------------------------> " + Event.current);

            if (Drag.IsDropping) { HandleDrop(onReorder, onBeforeReorder); } else if (Drag.IsDragging) { HandleDrag(); }
			if (CacheList == null) return;

            

			for (int i = 0; i < CacheList.Count; i++) { //layout
                //var willLog = i < 5 && CacheList.Count == 12;

				T item = CacheList[i];
				var info = GetInfo(i, item).SetWidth(w - (hasScroll ? scrollW : 0)).SetX(x);
				var first = info.CurrentRect.height == 0;

                /*if (RectMap[CacheList[i]] == RectMap[CacheList[0]]) {
                    Debug.Log("Invalid <" + i + ">" + string.IsNullOrEmpty((string)(object)item) + ":" + CacheList[0]);
                }*/

                //Debug.Log(i + " CHECK :: " + CacheList[0] + ":" + (RectMap[CacheList[0]] == RectMap[CacheList[i]]));


                //info._logChange = i == 0;
                //if (i == 0) Debug.Log(item.GetHashCode() + " Before :: " + info.CurrentRect);

                if (first) {
					info.SetY(y);//.SetHeight(Theme.CellHeight);
				}
				info.AnimateStep();

				if (!Drag.IsDragging) {
					if (Drag.IsDropping && counter == Drag.DropIdx) {
						if (item != Drag.Target) {
							LIInfo animx = Drag.Info;
							animx.TweenY(y + ch);
							ch += animx.CurrentRect.height;
						}
					}

					if (item != Drag.Target) {
						if (Event.current.type == EventType.Layout) { info.SetY(y + ch); } else { info.TweenY(y + ch); }
					}
				}

                //if (i == 0) Debug.Log(item.GetHashCode() + " After :: " + info.CurrentRect);

                if (item == Drag.Target) { continue; }

                //if (i == 0) Debug.Log("Before ::" + info.CurrentRect);

                var newH = DrawCell(i != Drag.DragIdx ? onDrawCell : null,
									info.CurrentRect,
									item, i,
									(Drag.Target != null && counter >= Drag.DropIdx) ? counter + 1 : counter); //!(info.Selected==true)

                //if (i == 0) Debug.Log("After ::" + info.CurrentRect);

                if (newH > 0) {
					if (first) {
						info.SetHeight(newH);
					} else {
						info.TweenH(newH);
					}
				}

				if (info.IsAnimate && !NeedRepaint) { NeedRepaint = true; }

				if (!Drag.IsDragging && !Drag.IsDropping) {
                    if (info.CanDrag && info.CurrentRect.w(Theme.DragWidth).h(20f).xLMB_isDown().noModifier) {
                        StartDrag(i);
                    } else if (info.Selected != null && info.CurrentRect.xLMB_isDown().noModifier) {
                        info.Selected = !info.Selected;
                    }

					if (OnRightClick != null && info.CurrentRect.xRMB_isDown().noModifier) { OnRightClick(item, i, this); }
				}


				ch += info.CurrentRect.height;
				counter++;
                //if (willLog) Debug.Log(counter + ":" + ch + "::::" + info.CurrentRect);

                //T item0 = CacheList[0];
                //var info0 = GetInfo(item0);
                //Debug.Log(i + " Last ---> " + info0.CurrentRect);
            }

            /*if (Drag.IsDragging) {
                //DEBUG
                GUI.DrawTexture(GuiRect, Color.yellow.xAlpha(0.2f).xGetTexture2D());
                GUI.DrawTexture(new Rect(MousePos.x, MousePos.y, 10, 10), Color.red.xAlpha(0.2f).xGetTexture2D());
            }*/

            if (Drag.IsDragging) {
			    ch += Drag.Info.CurrentRect.height;
			}

			if (ch != h && ch > 0) {
				GuiRect.height = ch;
				NeedRepaint = true;
			}

            if (Drag.Target != null) {
				LIInfo dragRect = Drag.Info;
				int newH = DrawCell(onDrawCell,
									Drag.IsDragging ? MouseDragRect : dragRect.CurrentRect,
									(T) Drag.Target, Drag.DragIdx,
									Drag.DropIdx);
				if (newH > 0) { dragRect.SetHeight(newH); }
			}

            if (hasScroll) { GUI.EndScrollView(); }
        }

		//---------------------------------------- PUBLIC APIs --------------------------------------------

		public LIInfo GetInfo(int idx, T item) {
			if (item == null) {
				Debug.LogWarning("item should not be null");
				return null;
			}

            if (idx == -1) { //works if items being different
                idx = CacheList.IndexOf(item);
                if (idx == -1) return null;
            }

			if (RectMap.ContainsKey(idx)) {
                var li = RectMap[idx];
			    if (li.data == item) return li;

                //invalid data found
                RectMap.Remove(idx);
			}

			LIInfo<T> info = new LIInfo<T> 
			{
                data        = item,
				Selected    = Theme.Selectable ? (bool?) false : null,
				CanDrag     = Theme.Draggable,
				CanDelete   = Theme.ShowDelete
			};//.SetXY(GuiRect.x, GuiRect.y);.SetHeight(Theme.CellHeight);

            //Debug.Log("NEW :: " + idx);

			RectMap.Add(idx, info);
			return info;
		}

		public void Focus(T item) {
			var idx = CacheList.IndexOf(item);
			if (idx == -1) return;

			var info = GetInfo(idx, item);
			var vh = ScrollRect.height;
			var y	= info._cRect.y;
			var min = GuiRect.y + ScrollPosition.y;
			var max = min + vh;

			if (min <= y && y <= max) return; //already in view : do nothing
			ScrollPosition.y = Mathf.Max( y-GuiRect.y - vh/2, 0f);
		}

		public void StartDrag(int dragIdx) {
			Drag.Target = CacheList[dragIdx]; //List.First(item => item.Index == dragIdx);
			Drag.Info = GetInfo(dragIdx, (T) Drag.Target);
			Drag.DragIdx = dragIdx;
			Drag.DropIdx = dragIdx;
			Drag.Idx = dragIdx;

			Vector2 m = Event.current.mousePosition;
			Rect r = Drag.Info.CurrentRect;
			Drag.DragMouseX = m.x - r.x;
			Drag.DragMouseY = m.y - r.y;

            EditorGUI.FocusTextInControl(null);
		}
		public void StopDrag(bool drop) {
			if (!drop) {
			    Drag.DropIdx = Drag.DragIdx;
			} else {
                var tmp = RectMap[Drag.DragIdx];
                RectMap[Drag.DragIdx] = RectMap[Drag.DropIdx];
                RectMap[Drag.DropIdx] = tmp;
			}

			Drag.DragIdx = -1;
			Rect mRect = MouseDragRect;
			Drag.Info.SetXY(mRect.x, mRect.y).TweenX(GuiRect.x);
		}
		public bool IsSelected(T item) { return GetInfo(-1, item).Selected == true; }
		public void SetSelected(T item, bool value) { GetInfo(-1, item).Selected = value; }
		public void InvertSelection() {
            for (var i = 0;i < CacheList.Count; i++) {
                LIInfo info = GetInfo(i, CacheList[i]);
                info.Selected = !info.Selected;
            }
		}
		public void SetSelection(bool isSelected) {
            for (var i = 0; i < CacheList.Count; i++) {
                LIInfo info = GetInfo(i, CacheList[i]);
                info.Selected = isSelected;
            }
        }
		/*public void CacheRect() {
			if (Id != null) {
				_c_idx = Id;
				_c_GuiRect = GuiRect;
				_c_ScrollRect = ScrollRect;
				_c_ScrollPosition = ScrollPosition;

				var list = new List<int>();
				for (var i = 0; i < CacheList.Count; i++) {
					var info = GetInfo(CacheList[i]);
					if (info.Selected == true) list.Add(i);
				}

				_c_SelectedIndexes = list.ToArray();
			}
		}*/
		private void HandleDrag() {
			NeedRepaint = true;
            //true || 
            if (Event.current.type == EventType.MouseDrag) { //only update when mouse drag
				LIInfo dragAnim = Drag.Info;

				var ch = GuiRect.y;
                var dropIdx = -1;
				var lastCheck = false;
				var cnt = 0;
				var localMouse = Event.current.mousePosition;

                for (var i = 0; i< CacheList.Count; i++) {
                    if (i == Drag.DragIdx) { continue; }

                    var item = CacheList[i];
                    var info = GetInfo(i, item);

                    var checkRect = info.CurrentRect.y(ch);
                    var check = checkRect.Contains(localMouse);
                    if (lastCheck && !check) { dropIdx = cnt - 1; }
                    lastCheck = check;

                    info.TweenY(ch);
                    ch += info.CurrentRect.height;

                    //Debug.Log(i + "--->" + cnt + ":" + ch + ":" + check + ":"+ info.CurrentRect.height + ":" + Drag.DragIdx + ":" + dropIdx);
                    //Debug.Log(i + "--->" + cnt + ":" + ch + ":" + Drag.DragIdx + ":" + dropIdx + "::::" + info.CurrentRect.height);
                    cnt++;
                }

                //Debug.Log("================================================" + ch + ":" + dropIdx + "   " + Event.current);

                GuiRect.height = ch + dragAnim.CurrentRect.height - GuiRect.y;
				if (dropIdx == -1) { dropIdx = CacheList.Count - (lastCheck ? 2 : 1); }
				Drag.DropIdx = dropIdx;

				cnt = 0;

                for (var i = 0; i< CacheList.Count; i++) {
                    if (i == Drag.DragIdx) { continue; }

                    var t = CacheList[i];

                    if (cnt >= dropIdx) {
                        GetInfo(i, t).TweenDY(dragAnim.CurrentRect.height);
                        //Debug.Log(i + "===>" + cnt + "--->" + GetInfo(i, t).CurrentRect);
                    }
                    cnt++;
                }
			}

            //Fix for dragging the last position, ScrollRect's size does not contains the dragging one
            var r2 = ScrollRect.dh(Drag.Info._cRect.height);

			//stop drag if mouse out or up
			if (Drag.IsDragging && ScrollRect.height > 0 && !r2.Contains(MousePos)) {
                //Debug.Log("Stop Drag at : " + MousePos + ":" + GuiRect + ":" + r2);
                StopDrag(false);
			}

			if (Event.current.type == EventType.MouseUp) { StopDrag(true); }
		}
		private void HandleDrop(Action<int, int, T> onReorder, Action<int, int, T> onBeforeReorder) {
			NeedRepaint = true;
			LIInfo dragRect = Drag.Info;
			dragRect.AnimateStep();
			if (dragRect.IsAnimate) {
				return; //check if drop completed
			}

			int dragIdx = Drag.Idx;
			int dropIdx = Drag.DropIdx;

            if (onBeforeReorder != null) { onBeforeReorder(dragIdx, dropIdx, (T)Drag.Target); }
			CacheList.Remove((T) Drag.Target);
			CacheList.Insert(dropIdx, (T) Drag.Target);

            if (CoArrays != null) {
                var item = CoArrays[dragIdx];
                CoArrays.RemoveAt(dragIdx);
                CoArrays.Insert(dropIdx, item);
            }

            if (onReorder != null) { onReorder(dragIdx, dropIdx, (T)Drag.Target); }

			Drag.DropIdx = -1;
			Drag.Target = null;
		}
		private int DrawCell(Func<Rect, T, int, int> onDrawCell, Rect r, T item, int i, int idx = -1) {
            if (item == null) {
                Debug.LogWarning("Item should not be null :: " + i);
            }

			LIInfo info = GetInfo(i, item);

			if (item == Drag.Target && onDrawCell != null) {
				if (Theme.DraggingTex != null) { GUI.DrawTexture(r.h(info.CurrentRect.height), Theme.DraggingTex); }
                if (Theme.ShowIndex) GUI.Label(r.dx(3).w(30f), (idx + 1) + ""); //.dy(3)
            } else {
				if (info.Selected == true) {
					if (Theme.SelectedTex != null) { GUI.DrawTexture(r.dl(1).dt(1), Theme.SelectedTex); }
				} else if (idx%2 == 0) { //even
					if (Theme.EvenBgTex != null) { GUI.DrawTexture(r, Theme.EvenBgTex); }
				} else { //odd
					if (Theme.OddBgTex != null) { GUI.DrawTexture(r, Theme.OddBgTex); }
				}
			}

			if (onDrawCell != null) {
                var indexW = Theme.ShowIndex ? 20 : 0;

                if (Theme.ShowIndex) {
                    GUI.Label(r.dx(3).w(indexW), (idx + 1) + "", GuiX.miniLabelGrayStyle);//.dy(3)
                }
                return onDrawCell(r.dl(indexW), item, idx);
			}

            return -1;
		}
	}
}