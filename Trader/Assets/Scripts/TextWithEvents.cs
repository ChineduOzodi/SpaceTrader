using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System.Text;
using UnityEngine.UI.CoroutineTween;

[AddComponentMenu("UI/TextWithEvents", 12), Serializable, RequireComponent(typeof(RectTransform), typeof(CanvasRenderer))]
public class TextWithEvents : Text
{
		[TextArea(3, 10), SerializeField]
		private string
				nonParsedStr;
		public AnimationCurve m_CurveData = AnimationCurve.Linear (0, 0, 1, 0); //jednak normalized curve z y normalized i mulitplied by curvemulti
		public float curveMultiplier = 100; //for animation support too
		public RectTransform rectT;
		public static int lastClickedIndex;

#if UNITY_EDITOR

		protected override void OnValidate ()
		{
				base.OnValidate ();
				base.text =
		OnBeforeValueChange (nonParsedStr);
				var allTextButtons = GetComponentsInChildren<TextButton> ();
				for (var id = 0; id < allTextButtons.Length; id++)
						allTextButtons [id].WrapperForValidation ();
		}
#endif
		public Dictionary<string, List<Link>> linksList = new Dictionary<string, List<Link>> ();
		//workaround for lack of OnValueChanged event
		public new string text {
				get { return base.text; }
				set {
						if (string.IsNullOrEmpty (value)) {
								if (string.IsNullOrEmpty (this.text)) {
										return;
								}
								base.text = string.Empty;
								nonParsedStr = string.Empty;
								this.SetVerticesDirty ();
						} else {
								string tmpStr = Regex.Replace (value, @"((<color=\#([^>\n\s]+)>)?<a href=([^>\n\s]+)>)|(</a>)", string.Empty);
								base.text = value;
								if (!onlyColorChanged && base.text != tmpStr) {
										base.text =
	OnBeforeValueChange (value);
										nonParsedStr = value;
										foreach (var button in linksList)
												foreach (var link in button.Value)
														link.Reset ();
										this.SetVerticesDirty ();
								}
						}
						onlyColorChanged = false;
				}
		}
		public bool onlyColorChanged = false;
		//used for optimizing merge string 
		private StringBuilder sb = new StringBuilder ();

		private string[] splittedStr;
		//compiled regex eat relatively a lot time on start but gain performance later. it require .NET 2.0 instead of .NET 2.0 subset
		//if u dont support .Net 2.0 or hiccup is out of the question simply delete |RegexOptions.Compiled
		private static Regex _regex = new Regex (@"<a href=([^>\n\s]+)>(.*?)(</a>)", RegexOptions.Singleline /*| RegexOptions.Compiled*/);

		//check if text contain any link if yes then strip them and generate some infos useful for OnFillVBO

		protected override void OnEnable ()
		{
				base.OnEnable ();
				rectT = GetComponent<RectTransform> ();
				OnBeforeValueChange (nonParsedStr);
		}

		private string OnBeforeValueChange (string strToParse)
		{
				if (strToParse == null)
						return strToParse;
				splittedStr = _regex.Split (strToParse);
				var i = 0;
				//clear sb
				sb.Length = 0;
				//allocate memory once, no more later if actual text is smaller than previous.
				//Placing the biggest planned string for this component on one frame may be a good idea for optimize allocation
				//but remember - this make hiccup particulary for realy huge text
				sb.EnsureCapacity (strToParse.Length);
				if (linksList.Count != 0) {
						foreach (var button in linksList)
								foreach (var link in button.Value)
										if (link != null)
												link.enabled = false;
				} else {
						var allLinks = GetComponentsInChildren<Link> ();
						foreach (var aloneLink in allLinks) {
								aloneLink.enabled = false;
								if (linksList.ContainsKey (aloneLink.name))
										linksList [aloneLink.name].Add (aloneLink);
								else
										linksList.Add (aloneLink.name, new List<Link> () { aloneLink });
						}
				}
				Transform child;
				foreach (var str in splittedStr) {
						if (i + 2 < splittedStr.Length && !splittedStr [i].EndsWith (@"</a") && splittedStr [i + 2] == "</a>") {
								int[] charsId = new int[2] {
										sb.Length,
										sb.Length + splittedStr [i + 1].Length - 1
								};

								child = transform.FindChild (str);
								if (child != null && linksList.ContainsKey (str)) {
										bool linkAlreadyExist = false;
										for (int id = 0; id < linksList[str].Count; id++) {
												if (id < linksList [str].Count && !linksList [str] [id].enabled) {
														linksList [str] [id].enabled = true;
														linksList [str] [id].linkStartAt [0] = linksList [str] [id].linkStartAt [1] = 0;
														linksList [str] [id].linkStartAt = charsId;
														linkAlreadyExist = true;
														break;
												}
										}
										if (!linkAlreadyExist) {
												var link = child.gameObject.AddComponent<Link> ();
												link.linkStartAt = charsId;
												linksList [str].Add (link);
										}
								} else {
										//add event for new buton created so procedural content will be easier
										Link link;
										if (child == null) {
												var newButton = new GameObject (str);
												newButton.AddComponent<TextButton> ().targetText = this;
												link = newButton.AddComponent<Link> ();
												newButton.AddComponent<Image> ();
												newButton.GetComponent<Image> ().color = new Color (0, 0, 0, 0);
												var rectTr = newButton.GetComponent<RectTransform> ();
												rectTr.SetParent (transform, false);
												rectTr.offsetMin = rectTr.offsetMax = rectTr.anchorMin = new Vector2 (0, 0);
												rectTr.anchorMax = new Vector2 (1, 1);
										} else
												link = child.gameObject.AddComponent<Link> ();

										link.linkStartAt = charsId;
										if (linksList.ContainsKey (str)) {
												linksList [str].Add (link);
										} else {
												linksList.Add (str, new List<Link> () { link });
										}
								}

						} else if (str != "</a>" && str != string.Empty)
								sb.Append (str);
						i++;
				}

				return sb.ToString ();
		}
	protected override void OnPopulateMesh (VertexHelper vbo)
	{
		base.OnPopulateMesh (vbo);
        List<UIVertex> stream = new List<UIVertex>();
        vbo.GetUIVertexStream(stream);
		UIVertex uiVertex;
		for (int index = 0; index < vbo.currentVertCount; index++) {
				uiVertex = stream[index];
				uiVertex.position.y += m_CurveData.Evaluate ((rectT.rect.width * rectT.pivot.x + uiVertex.position.x) / rectT.rect.width) * curveMultiplier;
				stream[index] = uiVertex;
            vbo.SetUIVertex(uiVertex, index);
		}
	}
}
