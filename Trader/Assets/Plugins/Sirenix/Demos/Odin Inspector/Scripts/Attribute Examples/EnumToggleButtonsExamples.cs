namespace Sirenix.OdinInspector.Demos
{
    using UnityEngine;
    using Sirenix.OdinInspector;

    public class EnumToggleButtonsExamples : MonoBehaviour
    {
        public MyBitmaskEnum DefaultEnumBitmask;

        [EnumToggleButtons]
        public MyEnum MyEnumField;

        [EnumToggleButtons, HideLabel]
        public MyBitmaskEnum BitmaskEnumField;

        [EnumToggleButtons]
        public MyBitmaskEnum[] BitmaskArray;
    }

    [System.Flags]
    public enum MyBitmaskEnum
    {
        A = 1 << 1,
        B = 1 << 2,
        C = 1 << 3,
        ALL = A | B | C
    }

    public enum MyEnum
    {
        A, B, C
    }
}