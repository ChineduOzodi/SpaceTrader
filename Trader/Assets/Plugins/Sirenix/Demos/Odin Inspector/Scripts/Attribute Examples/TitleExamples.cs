namespace Sirenix.OdinInspector.Demos
{
    using UnityEngine;

    public class TitleExamples : MonoBehaviour
    {
        [InfoBox("Title and Header both have the same functionality, but Title can also be applied on properties.\nTitles also have the option to be non-bold.")]
        [Header("Headers and titles")]
        public int A;

        [Title("Title on a field")]
        public int B;

        [Title("Non-bold title", false)]
        public int C;

        [PropertyOrder(5)]
        [ShowInInspector]
        [Title("Title on a property")]
        public float MyProperty { get; set; }
    }
}