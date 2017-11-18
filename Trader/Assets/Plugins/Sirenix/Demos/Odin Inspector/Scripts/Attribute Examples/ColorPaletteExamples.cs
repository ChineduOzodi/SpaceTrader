namespace Sirenix.OdinInspector.Demos
{
    using UnityEngine;
    using Sirenix.OdinInspector;

    public class ColorPaletteExamples : MonoBehaviour
    {
        [ColorPalette]
        public Color ColorOptions;

        [ColorPalette("Underwater")]
        public Color UnderwaterColor;

        [ColorPalette("Fall"), HideLabel]
        public Color WideColorPalette;

        [ColorPalette("My Palette")]
        public Color MyColor;

        [ColorPalette("Clovers")]
        public Color[] ColorArray;
    }
}