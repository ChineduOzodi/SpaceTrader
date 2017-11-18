namespace Sirenix.OdinInspector.Demos
{
    using UnityEngine;

    public class DisplayAsStringExamples : MonoBehaviour
    {
        [InfoBox(
            "Instead of disabling values in the inspector in order to show some information or debug a value. " +
            "You can use DisplayAsString to show the value as text, instead of showing it in a disabled drawer")]
        [DisplayAsString]
        public Color SomeColor;

        [BoxGroup("SomeBox")]
        [DisplayAsString]
        [HideLabel]
        public string SomeText = "Lorem Ipsum";
    }
}