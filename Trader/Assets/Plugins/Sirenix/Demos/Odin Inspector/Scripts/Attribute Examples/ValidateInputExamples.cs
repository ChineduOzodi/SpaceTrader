namespace Sirenix.OdinInspector.Demos
{
    using UnityEngine;

    public class ValidateInputExamples : MonoBehaviour
    {
        [InfoBox("ValidateInput is used to display error boxes in case of invalid values.\nIn this case the GameObject must have a MeshRenderer component.")]
        [ValidateInput("HasMeshRenderer", "Prefab must have a MeshRenderer component")]
        public GameObject GameObjectWithRenderer1;

        [ValidateInput("HasMeshRenderer", "Prefab must have a MeshRenderer component")]
        public GameObject GameObjectWithRenderer2;

        [ValidateInput("HasMeshRenderer", "Prefab must have a MeshRenderer component")]
        public GameObject GameObjectWithRenderer3;

        private bool HasMeshRenderer(GameObject gameObject)
        {
            if (gameObject == null) return true;

            return gameObject.GetComponentInChildren<MeshRenderer>() != null;
        }
    }
}