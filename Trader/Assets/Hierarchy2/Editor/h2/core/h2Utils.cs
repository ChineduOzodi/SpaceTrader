using UnityEngine;
using System.Collections;


namespace h2 {
	
	internal static class h2Utils {
		static public void ForceRefreshHierarchy() {
			TransformX.RootT.ForEach(
				t => {
					if (t == null) return;
					
					t.gameObject.xForeachChild(
						child => {
							child.xToggleFlag(HideFlags.NotEditable);
							child.xToggleFlag(HideFlags.NotEditable);
						}, true);
					
					WindowX.Hierarchy.Repaint();
				}
			);
		}
	}
	
	internal static class h2Extern {
		internal static bool IsMultiScene(GameObject go) {
			return go != null && IsMultiScene(go.transform);
	    }
				
	    internal static bool IsMultiScene(Transform t) {
	        if (t == null || t.parent != null) return false; //Multiscene Objects must be Root
	        var c = t.GetComponent("Multiscene");
	        return c != null;
	    }
	}
}
