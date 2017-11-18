namespace Sirenix.OdinInspector.Demos
{
    using System;
    using UnityEngine;

    public class BoxGroupExamples : MonoBehaviour
    {
        [BoxGroup("Centered Title", centerLabel: true)]
        public int A;

        [BoxGroup("Centered Title", centerLabel: true)]
        public int B;

        [BoxGroup("Centered Title", centerLabel: true)]
        public int C;

        [BoxGroup("Left Oriented Title")]
        public int D;

        [BoxGroup("Left Oriented Title")]
        public int E;

        [BoxGroup("Left Oriented Title")]
        public int One;

        [BoxGroup("Left Oriented Title")]
        public int Two;

        [InfoBox("You can also hide the label of a box group.")]
        [BoxGroup("NoLabel", false)]
        public int Three;

        [BoxGroup("NoLabel")]
        public int Four;

        [BoxGroup("NoLabel")]
        public int Five;

        [BoxGroup("Boxed Struct"), HideLabel]
        public SomeStruct BoxedStruct;

        public SomeStruct DefaultStruct;

        [Serializable]
        public struct SomeStruct
        {
            public int One;
            public int Two;
            public int Three;
        }
    }
}