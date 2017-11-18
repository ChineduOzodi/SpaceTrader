namespace Sirenix.OdinInspector.Demos
{
    using UnityEngine;

    public class HideLabelExamples : MonoBehaviour
    {
        [Title("Wide Colors", bold: false)]
        [HideLabel]
        [ColorPalette("Fall")]
        public Color WideColor1;

        [HideLabel]
        [ColorPalette("Fall")]
        public Color WideColor2;

        [Title("Wide Vector", bold: false)]
        [HideLabel]
        public Vector3 WideVector1;

        [HideLabel]
        public Vector4 WideVector2;

        [Title("Wide String", bold: false)]
        [HideLabel]
        public string WideString;

        [Title("Wide Multiline Text Field", bold: false)]
        [HideLabel]
        [MultiLineProperty]
        public string WideMultilineTextField = "";
    }
}