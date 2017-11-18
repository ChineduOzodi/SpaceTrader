namespace Sirenix.OdinInspector.Demos
{
    using System.Collections.Generic;
    using UnityEngine;
    using Sirenix.OdinInspector;

    // Example script from the Unity asset store.
    public class NoBoilerplate : MonoBehaviour
    {
        [TabGroup("Tab 1")]
        [AssetList, InlineEditor(InlineEditorModes.SmallPreview)]
        public GameObject GameObject;

        [TabGroup("Tab 1")]
        private Quaternion Quaternion;

        [TabGroup("Tab 2")]
        private Vector3 Vector3;

        [Multiline, HideLabel, Title("Enter text:", bold: false)]
        public string MyTextArea;

        [ColorPalette("Fall")]
        public List<Color> ColorArray;

        [Range(0, 1)]
        public float[] FloatRange;

        [ShowInInspector, DisplayAsString]
        public string Property { get { return "Support!"; } }

        [Button]
        private void SayHi()
        {
            Debug.Log("Yo!");
        }
    }
}