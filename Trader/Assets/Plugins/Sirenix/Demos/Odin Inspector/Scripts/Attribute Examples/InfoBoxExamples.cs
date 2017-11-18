namespace Sirenix.OdinInspector.Demos
{
    using UnityEngine;

    public class InfoBoxExamples : MonoBehaviour
    {
        [Header("InfoBox message types")]
        [Indent]
        [InfoBox("Default info box.")]
        public int A;

        [Indent]
        [InfoBox("Warning info box.", InfoMessageType.Warning)]
        public int B;

        [Indent]
        [InfoBox("Error info box.", InfoMessageType.Error)]
        public int C;

        [Indent]
        [InfoBox("Info box without an icon.", InfoMessageType.None)]
        public int D;

        [Header("Conditional info boxes")]
        [Indent]
        public bool ToggleInfoBoxes;

        [InfoBox("This info box is only shown while in editor mode.", InfoMessageType.Error, "IsInEditMode")]
        [Indent]
        public float G;

        [InfoBox("This info box is hideable by a static field.", "ToggleInfoBoxes")]
        [Indent]
        public float E;

        [InfoBox("This info box is hideable by a static field.", "ToggleInfoBoxes")]
        [Indent]
        public float F;

        private static bool IsInEditMode()
        {
            return !Application.isPlaying;
        }
    }
}