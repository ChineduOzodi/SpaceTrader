namespace Sirenix.OdinInspector.Demos
{
    using UnityEngine;
    using System.Collections.Generic;

    public class DictionaryExamples : SerializedMonoBehaviour
    {
        [InfoBox("In order to serialize dictionaries, all we need to do is to inherit our class from SerializedMonoBehaviour.")]
        public Dictionary<string, MyCustomType> StringObjectLookup;

        public Dictionary<string, List<int>> stringListDictioanry;

        public Dictionary<int, Material> IntMaterialLookup;

        public Dictionary<string, string> StringStringDictionary;

        [InfoBox("Odin supports all type for its value, but for the key your are currently limited.")]
        [InfoBox("Remember, it is only the Inspector that is lacking support for other key types. Serialization is still going to work as expected.")]
        public Dictionary<MonoBehaviour, float> NotSupportedDictionary;

        // The following key types are supported:
        // string, Char, byte, sbyte, ushort, short, uint, int, ulong, long, float, double, decimal

        public class MyCustomType
        {
            public int  SomeMember;
            public Quaternion SomeRotation;
            public GameObject SomePrefab;
        }
    }
}