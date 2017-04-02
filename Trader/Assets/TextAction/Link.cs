using UnityEngine;
using System.Collections;
using UnityEngine.UI.CoroutineTween;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[Serializable]
public class Link : UIBehaviour
{
		private Color
				linkColor;
		[SerializeField, HideInInspector]
		private TextWithEvents
				targetText;
		[HideInInspector]
		public int[]
				linkStartAt;
		public TweenRunner<ColorTween> ColorTweener = new TweenRunner<ColorTween> ();
#if UNITY_EDITOR
		protected override void OnValidate ()
		{
				base.OnValidate ();
				SetColor (linkColor);
		}
#endif

		public void SetColor (Color color)
		{
				if (!enabled || linkStartAt == null || linkStartAt [0] - 9 < 0)
						return;
				linkColor = color;
				if (targetText == null)
						targetText = GetComponentInParent<TextWithEvents> ();
				if (targetText == null || targetText.text.Length == 0)
						return;
				targetText.onlyColorChanged = true;
				var id = targetText.text.IndexOf ("<color=#", linkStartAt [0] - 17);
				if (id == linkStartAt [0] - 17) {
						var tmpStr = targetText.text.Remove (linkStartAt [0] - 9, 8);
						targetText.text = tmpStr.Insert (linkStartAt [0] - 9, ColorToHex (color));
				}
		}
		private string ColorToHex (Color32 color)
		{
				return color.r.ToString ("X2") + color.g.ToString ("X2") + color.b.ToString ("X2") + color.a.ToString ("X2");
		}
		void Awake ()
		{
				ColorTweener.Init (this);
		}
		/*private Color HexToColor (string hex)
	{
			byte r = byte.Parse (hex.Substring (0, 2), System.Globalization.NumberStyles.HexNumber);
			byte g = byte.Parse (hex.Substring (2, 2), System.Globalization.NumberStyles.HexNumber);
			byte b = byte.Parse (hex.Substring (4, 2), System.Globalization.NumberStyles.HexNumber);
			byte a = byte.Parse (hex.Substring (6, 2), System.Globalization.NumberStyles.HexNumber);
			return new Color32 (r, g, b, a);
	}*/
		public void CrossFadeColor (Color targetColor, bool ignoreTimeScale, bool useAlpha, bool useRGB)
		{
				ColorTween.ColorTweenMode tweenMode = (!useRGB || !useAlpha) ? ((!useRGB) ? ColorTween.ColorTweenMode.Alpha : ColorTween.ColorTweenMode.RGB) : ColorTween.ColorTweenMode.All;
				ColorTween info = new ColorTween
{
	duration = 0,
	startColor = linkColor,
	targetColor = targetColor
};
				info.AddOnChangedCallback (SetColor);
				info.ignoreTimeScale = ignoreTimeScale;
				info.tweenMode = tweenMode;
				ColorTweener.StartTween (info);

		}
		public void Reset ()
		{
				var button = GetComponent<TextButton> ();
				SetColor ((button.interactable ? button.colors.normalColor : button.colors.disabledColor) * button.colors.colorMultiplier);

		}
}
