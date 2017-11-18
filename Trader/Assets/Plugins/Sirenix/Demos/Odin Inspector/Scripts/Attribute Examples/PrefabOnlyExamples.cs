namespace Sirenix.OdinInspector.Demos
{
    using UnityEngine;

    public class PrefabOnlyExamples : MonoBehaviour
    {
        [InfoBox(
            "EnableForPrefabOnly and ShowForPrefabOnly disables and hides properties from prefab instances.\n\n" +
            "On instances, with no prefab connection, these attributes does nothing.")]
        [EnableForPrefabOnly]
        public int EditOnlyInPrefab;

        [ShowForPrefabOnly]
        public int ShowInPrefab;
    }
}