using UnityEditor.UI;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine;

[CanEditMultipleObjects, CustomEditor(typeof(TextButton), true)]
public class TexButtonHandler : ButtonEditor
{
		private TextButton Target {
				get {
						return target as TextButton;
				}
		}
		protected override void OnEnable ()
		{
				base.OnEnable ();
		}
		[MenuItem("GameObject/UI/Text Button")]
		static void Test (MenuCommand command)
		{
				var go = new GameObject ("TextButton");
				GameObjectUtility.SetParentAndAlign (go, command.context as GameObject);
				go.AddComponent<TextButton> ().targetText = command.context as TextWithEvents;
				go.GetComponent<Image> ().color = new Color (0, 0, 0, 0);
				Undo.RegisterCreatedObjectUndo (go, "Create" + go.name);
				Selection.activeObject = go;
		}
		public override void OnInspectorGUI ()
		{
				base.OnInspectorGUI ();
				if (Target.transition == Selectable.Transition.SpriteSwap || Target.transition == Selectable.Transition.Animation)
						EditorGUILayout.HelpBox ("WARNING! SriteSwap isnt supported if ever and Animation isnt ready yet", MessageType.Warning);
		}
}
