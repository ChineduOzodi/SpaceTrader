namespace Sirenix.OdinInspector.Demos
{
    using System.Collections.Generic;
    using UnityEngine;

    public class HideInEditorAndPlayModeExamples : MonoBehaviour
    {
        [Title("Hidden in play mode")]
        [Indent]
        [HideInPlayMode]
        public List<Material> MyMaterials;

        [Indent]
        [HideInPlayMode]
        public Material MyMaterial;

        [Title("Hidden in editor mode")]
        [Indent]
        [HideInEditorMode]
        public List<GameObject> MyPrefabs;

        [Indent]
        [HideInEditorMode]
        public GameObject Prefab;
    }
}