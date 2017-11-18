namespace Sirenix.OdinInspector.Demos
{
    using UnityEngine;

    public class FoldoutGroupAttributeExamples : MonoBehaviour
    {
        [FoldoutGroup("Group 1")]
        public int A;

        [FoldoutGroup("Group 1")]
        public int B;

        [FoldoutGroup("Group 1")]
        public int C;

        [FoldoutGroup("Group 2")]
        public int D;

        [FoldoutGroup("Group 2")]
        public int E;

        [FoldoutGroup("Group 3")]
        public int One;

        [FoldoutGroup("Group 3")]
        public int Two;
    }
}