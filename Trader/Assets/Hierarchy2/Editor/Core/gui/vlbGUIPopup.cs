using System;
using UnityEditor;
using UnityEngine;
using vietlabs;

namespace VietLabs {

	internal class vlbGUIPopup {
		string[] Labels;
		string[] SelectedLabels;
		int activeIdx;

		public vlbGUIPopup(Enum enumValue, string[] customLabels = null) {
			var t = enumValue.GetType();
			Labels = Enum.GetNames(t);
			SelectedLabels = customLabels ?? Labels;
		}

		public bool Draw(Rect rect) {
			GUI.Label(rect, SelectedLabels[activeIdx]);
			if (rect.xLMB_isDown().noModifier) {
				var menu = new GenericMenu();

				for (var i = 0; i < Labels.Length; i++) {
					var idx = i;

					menu.xAdd(Labels[i], ()=> {
						activeIdx = idx;
					}, i == activeIdx);
				}

				Event.current.Use();
				menu.ShowAsContext();
			}
			return false;
		}
	}
}
