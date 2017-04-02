using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using UnityEngine.UI.CoroutineTween;

[AddComponentMenu("UI/TextButton", 10), Serializable, RequireComponent(typeof(RectTransform), typeof(CanvasRenderer), typeof(Image))]
public class TextButton : Button, ICanvasRaycastFilter
{
		public TextWithEvents targetText;
		int hoverId = -1;
		private Canvas canvasInParent;
		private CanvasScaler cScaler;

#if UNITY_EDITOR
		protected override void OnValidate ()
		{
				base.OnValidate ();
				WrapperForValidation ();
		}
		public void WrapperForValidation ()
		{
				if (targetText == null)
						targetText = GetComponentInParent<TextWithEvents> ();
				if (targetText.linksList != null && targetText.linksList.ContainsKey (name)) {
						var linksInBtn = GetComponents<Link> ();
						foreach (var link in linksInBtn)
								link.SetColor ((interactable ? colors.normalColor : colors.disabledColor) * colors.colorMultiplier);
				}
		}
#endif

		void Start ()
		{
				targetText = GetComponentInParent<TextWithEvents> ();
				if (canvasInParent == null)
						canvasInParent = targetText.rectTransform.root.GetComponent<Canvas> ();
				if (cScaler == null)
						cScaler = targetText.rectTransform.root.GetComponent<CanvasScaler> ();
		}
		/*private bool CheckIfPointInPolygon (Vector2[] polygon, Vector2 point)
	{
			bool check = false;
			var lastVector = polygon.Length - 1;
			for (var i=0; i<polygon.Length; i++) {
					Vector2 tpi = polygon [i];
					Vector2 tpj = polygon [lastVector];

					if (tpi.y < point.y && tpj.y >= point.y || tpj.y < point.y && tpi.y >= point.y)
					if (tpi.x + (point.y - tpi.y) / (tpj.y - tpi.y) * (tpj.x - tpi.x) < point.x)
							check = !check;
					lastVector = i;
			}
			return check;
	}*/

		//change state like button but for link only
		protected override void DoStateTransition (SelectionState state, bool instant)
		{
				if (targetText == null || targetText.linksList == null)
						return;
				if (targetText.linksList.ContainsKey (name))
						for (int linkId = 0; linkId < targetText.linksList[name].Count; linkId++)
								InternalDoStateTransition (linkId, (state == SelectionState.Disabled || hoverId == linkId) ? state : SelectionState.Normal, instant);
		}

		private void InternalDoStateTransition (int id, SelectionState state, bool instant)
		{
				Color color;
				string triggerName;
				switch (state) {
				case SelectionState.Normal:
						color = colors.normalColor;
						triggerName = animationTriggers.normalTrigger;
						break;
				case SelectionState.Pressed:
						color = colors.pressedColor;
						triggerName = animationTriggers.pressedTrigger;
						break;
				case SelectionState.Highlighted:
						color = colors.highlightedColor;
						triggerName = animationTriggers.highlightedTrigger;
						break;
				case SelectionState.Disabled:
						color = colors.disabledColor;
						triggerName = animationTriggers.disabledTrigger;

						break;
				default:
						color = Color.black;
						triggerName = string.Empty;
						break;
				}

				if (base.gameObject.activeInHierarchy) {
						switch (this.transition) {
						case Selectable.Transition.ColorTint:
								targetText.linksList [name] [id].CrossFadeColor (color * this.colors.colorMultiplier, true, true, true);
								break;
						//case Selectable.Transition.SpriteSwap:
						//	//this.DoSpriteSwap(newSprite);
						//	break;
						}
				}
		}

		public bool IsRaycastLocationValid (Vector2 sp, Camera eventCamera)
		{

				//check if supported text contain any link with href=myname. if no disable button for prevent waste resource		
				if (targetText.linksList.ContainsKey (name) == null)
						return gameObject.active = false;
				Vector2 lp;
				var graphicRect = targetText.rectTransform;
				RectTransformUtility.ScreenPointToLocalPointInRectangle (graphicRect, sp, eventCamera, out lp);
				lp.y -= targetText.m_CurveData.Evaluate ((targetText.rectT.rect.width * targetText.rectT.pivot.x + lp.x) / targetText.rectT.rect.width) * targetText.curveMultiplier;
				lp.Scale (new Vector2 (graphicRect.lossyScale.x / targetText.transform.localScale.x, graphicRect.lossyScale.y / targetText.transform.localScale.y));

				if (canvasInParent.renderMode == RenderMode.WorldSpace)
						lp *= cScaler.dynamicPixelsPerUnit;

				var id = 0;
				bool check;
				int lastVector;
				int idVert;
				Vector2 tpi;
				Vector2 tpj;
				//check if UI is normal or layout and grab verts
				var cVerts = targetText.cachedTextGenerator.verts ?? targetText.cachedTextGeneratorForLayout.verts;
				if (cVerts.Count == 0)
						return false;
				//mnozyc przez pixel density w world mode
				foreach (var rects in targetText.linksList[name]) {
						if (rects.enabled)
								for (int i = rects.linkStartAt[0] * 4; i < (cVerts.Count > rects.linkStartAt[1] * 4 ? rects.linkStartAt[1] * 4 : cVerts.Count); i += 4)
										if (lp.x >= cVerts [i].position.x - ((i < rects.linkStartAt [1] * 4 - 8) ? targetText.fontSize * 2 : 0) && lp.x <= cVerts [i + 1].position.x + ((i < rects.linkStartAt [1] * 4 - 8) ? targetText.fontSize * 2 : 0)) {

												check = false;
												lastVector = i + 3;
												for (var j = 0; j < 4; j++) {
														switch (j) {
														case 1:
																idVert = 1;
																break;
														case 2:
																idVert = (i < cVerts.Count - 10) ? 4 : 0;
																break;
														case 3:
																idVert = (i < cVerts.Count - 10) ? 7 : 3; //sprawdzaj czy nie ucielo albo czy znak nowej lini
																break;
														default:
																idVert = 0;
																break;
														}
														tpi = cVerts [i + idVert].position;
														tpj = cVerts [lastVector].position;
														tpi.y += 7f;
														tpj.y -= 7f;
														tpj.x -= targetText.fontSize * 2;
														tpi.x += targetText.fontSize * 2;
														if (tpi.y < lp.y && tpj.y >= lp.y || tpj.y < lp.y && tpi.y >= lp.y)
														if (tpi.x + (lp.y - tpi.y) / (tpj.y - tpi.y) * (tpj.x - tpi.x) < lp.x)
																check = !check;
														lastVector = i + idVert;
												}

												if (check) {
														TextWithEvents.lastClickedIndex = hoverId = id;
														return true;
												}
										}
						id++;
				}
				hoverId = -1;
				return false;
		}
}
