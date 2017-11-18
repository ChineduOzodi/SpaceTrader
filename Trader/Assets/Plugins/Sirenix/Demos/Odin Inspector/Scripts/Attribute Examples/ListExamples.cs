namespace Sirenix.OdinInspector.Demos
{
    using UnityEngine;
    using System.Collections.Generic;

    public class ListExamples : MonoBehaviour
    {
        public List<float> FloatList;

        [Range(0, 1)]
        public float[] FloatRangeArray;

        public List<GameObject> GameObjectList;

        public List<AnimationCurve> AnimationCurveArray;
    }
}