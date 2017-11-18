namespace Sirenix.OdinInspector.Demos
{
    using System.Collections.Generic;
    using UnityEngine;

    public class DisableInEditorAndPlayModeExamples : MonoBehaviour
    {
        [Title("Disabled in play mode")]
        [Indent]
        [DisableInPlayMode]
        public List<Material> MyMaterials;

        [Indent]
        [DisableInPlayMode]
        public Material MyMaterial;

        [Title("Disabled in edit mode")]
        [Indent]
        [DisableInEditorMode]
        public List<GameObject> MyPrefabs;

        [Indent]
        [DisableInEditorMode]
        public GameObject Prefab;
    }
}