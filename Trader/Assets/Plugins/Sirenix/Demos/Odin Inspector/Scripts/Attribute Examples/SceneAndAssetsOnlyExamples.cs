namespace Sirenix.OdinInspector.Demos
{
    using UnityEngine;
    using System.Collections.Generic;

    public class SceneAndAssetsOnlyExamples : MonoBehaviour
    {
        [Title("Assets only")]
        [Indent]
        [AssetsOnly]
        public List<GameObject> OnlyPrefabs;

        [Indent]
        [AssetsOnly]
        public GameObject SomePrefab;

        [Indent]
        [AssetsOnly]
        public Material MaterialAsset;

        [Indent]
        [AssetsOnly]
        public MeshRenderer SomeMeshRendererOnPrefab;

        [Title("Scene Objects only")]
        [Indent]
        [SceneObjectsOnly]
        public List<GameObject> OnlySceneObjects;

        [Indent]
        [SceneObjectsOnly]
        public GameObject SomeSceneObject;

        [Indent]
        [SceneObjectsOnly]
        public MeshRenderer SomeMeshRenderer;
    }
}