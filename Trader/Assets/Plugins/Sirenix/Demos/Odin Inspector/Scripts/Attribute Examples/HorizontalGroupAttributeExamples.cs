namespace Sirenix.OdinInspector.Demos
{
    using UnityEngine;

    // The width can either be specified as percentage or pixels.
    // All values between 0 and 1 will be treated as a percentage.
    // If the width is 0 the column will be automatically sized.
    // Margin-left and right can only be specified in pixels.

    public class HorizontalGroupAttributeExamples : MonoBehaviour
    {
        // Group 1
        [HorizontalGroup("Group 1")]
        public int A;

        [HorizontalGroup("Group 1")]
        public int B;

        [HorizontalGroup("Group 1")]
        public int C;

        // Group 2
        [HorizontalGroup("Group 2")]
        [HideLabel]
        public int D;

        [HorizontalGroup("Group 2", width: 0.6f)] // Percentage
        [HideLabel]
        public int E;

        // Group 3
        [HorizontalGroup("Group 3", width: 90)] // Pixels
        [HideLabel]
        public int F;

        [HorizontalGroup("Group 3", marginLeft: 20)]
        [HideLabel]
        public int G;

        // My Group
        [HorizontalGroup("My Group")]
        [AssetList(AssetNamePrefix = "Rock")]
        public GameObject[] Left;

        [HorizontalGroup("My Group")]
        [AssetList(AssetNamePrefix = "Rock")]
        public GameObject[] Right;

        // Group 1
        [HorizontalGroup("Group 4")]
        public int H;

        [HorizontalGroup("Group 4", width: 80)]
        [Button]
        public void Button()
        {
        }
    }
}