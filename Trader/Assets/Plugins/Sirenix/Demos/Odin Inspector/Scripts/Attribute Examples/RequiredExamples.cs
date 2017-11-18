namespace Sirenix.OdinInspector.Demos
{
    using UnityEngine;

    public class RequiredExamples : MonoBehaviour
    {
        [InfoBox("Required displays an error when objects are missing.")]
        [Required]
        public GameObject MyGameObject;

        [Required]
        public Rigidbody MyRigidbody;
    }
}