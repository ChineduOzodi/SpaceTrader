namespace Sirenix.OdinInspector.Demos
{
    using System;
    using UnityEngine;
    using System.Collections.Generic;
    using System.Linq;
    using Utilities;

    [SelectionBase]
    [ExecuteInEditMode]
    public class ObjectPlacer : SerializedMonoBehaviour
    {
        [SerializeField, HideInInspector]
        private Matrix4x4 prevMatrix;

        [SerializeField]
        [InlineEditor(InlineEditorModes.GUIAndPreview,  PreviewWidth = 55)]
        public List<PlaceableObject> Prefabs;

        [SerializeField]
        [OnValueChanged("RepositionObjects", includeChildren: true)]
        [TabGroup("Placement", false, 2)]
        [HideLabel]
        public ObjectPlacementFunction ObjectPlacementFunction = new CirclePlacementFunction();

        [TabGroup("General")]
        [OnValueChanged("RepositionObjects")]
        public bool KeepPrefabReference;

        [Range(2, 400)]
        [TabGroup("General")]
        [OnValueChanged("RepositionObjects")]
        public int NumberOfObjects = 30;

        [Range(0, 30)]
        [TabGroup("General")]
        [OnValueChanged("RepositionObjects")]
        public float Radius = 4;

        [TabGroup("General")]
        [OnValueChanged("RepositionObjects")]
        public Vector3 Rotation;

        [Range(0, 1)]
        [TabGroup("General")]
        [OnValueChanged("RepositionObjects")]
        public float TerrainRotationFactor;

        [TabGroup("General")]
        [ValueDropdown("GetLayers")]
        [OnValueChanged("RepositionObjects")]
        public string TerrainLayer;

        [LabelText("X")]
        [TabGroup("Randomization")]
        [OnValueChanged("RepositionObjects")]
        [MinMaxSlider(-0.5f, 0.5f)]
        public Vector2 OffsetX;

        [LabelText("Y")]
        [TabGroup("Randomization")]
        [OnValueChanged("RepositionObjects")]
        [MinMaxSlider(-1, 1)]
        public Vector2 OffsetY;

        [LabelText("Z")]
        [TabGroup("Randomization")]
        [OnValueChanged("RepositionObjects")]
        [MinMaxSlider(-0.1f, 0.1f)]
        public Vector2 OffsetZ;

        [Range(0, 360)]
        [TabGroup("Randomization")]
        [OnValueChanged("RepositionObjects")]
        [LabelText("Angle")]
        public float AngleJitter;

        [TabGroup("Randomization")]
        [OnValueChanged("RepositionObjects")]
        [LabelText("Axis")]
        public Axis AngleJitterAxis = Axis.Y;

        private static List<string> GetLayers()
        {
            return Enumerable.Range(0, 32)
                .Select(i => LayerMask.LayerToName(i))
                .Where(s => s.Length > 0)
                .Prepend("All")
                .ToList();
        }

        [Button("Update")]
        [PropertyOrder(3)]
        public void ClearAndRepositionObjects()
        {
            for (int i = this.transform.childCount - 1; i >= 0; i--)
            {
                var go = this.transform.GetChild(i).gameObject;
                DestroyImmediate(go);
            }

            this.RepositionObjects();
        }

        private void Update()
        {
            if (this.prevMatrix != this.transform.localToWorldMatrix)
            {
                this.prevMatrix = this.transform.localToWorldMatrix;
                this.RepositionObjects();
            }
        }

        public void RepositionObjects()
        {
            if (this.ObjectPlacementFunction == null || this.Prefabs == null || !this.Prefabs.Any(x => x && x.Enabled))
            {
                return;
            }

            // Destroy child objects until count matches NumberOfObjects.
            while (this.transform.childCount > this.NumberOfObjects && this.transform.childCount > 0)
            {
                var go = this.transform.GetChild(0).gameObject;
                DestroyImmediate(go);
            }

            var rnd = new System.Random(this.GetInstanceID());
            //Random.InitState(0);

            // Create new objects until count matches NumberOfObjects.
            for (int i = this.transform.childCount; i < this.NumberOfObjects; i++)
            {
                var randomObject = this.Prefabs
                    .Where(x => x && x.Enabled)
                    .OrderBy(x => rnd.NextDouble() - x.SpawnChance)
                    .FirstOrDefault();

#if UNITY_EDITOR
                // Instantiate using PrefabUtility in edit mode to preserve prefab reference.
                GameObject go = !this.KeepPrefabReference || Application.isPlaying ? Instantiate(randomObject.gameObject) : (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(randomObject.gameObject);
#else
                GameObject go = Instantiate(randomObject.gameObject);
#endif
                go.transform.parent = this.transform;
            }

            // Recalculate transforms on all child objects.
            for (int i = 0; i < this.transform.childCount; i++)
            {
                var go = this.transform.GetChild(i).gameObject;
                PlaceableObject obj = go.GetComponent<PlaceableObject>();

                if (obj == null)
                {
                    continue;
                }

                var t = i / (float)this.transform.childCount;
                var binormal = this.ObjectPlacementFunction.GetBinormal(t);
                var tangent = this.ObjectPlacementFunction.GetTangent(t);
                var position = this.ObjectPlacementFunction.GetPosition(t) * this.Radius;
                var scale = Vector3.one;
                var rotation = this.transform.rotation * Quaternion.Euler(new Vector3(0, Mathf.Atan2(binormal.x, binormal.z) * Mathf.Rad2Deg, 0) + this.Rotation);
                var layer = 1 << LayerMask.NameToLayer(this.TerrainLayer);
                var s = new Vector3((int)this.AngleJitterAxis >> 1 & 1, (int)this.AngleJitterAxis >> 2 & 1, (int)this.AngleJitterAxis >> 3 & 1);

                // Randomization:
                var rndVector = new Vector3((float)rnd.NextDouble() - 0.5f, (float)rnd.NextDouble() - 0.5f, (float)rnd.NextDouble() - 0.5f);

                position += binormal * this.Radius * Mathf.Lerp(this.OffsetX.x, this.OffsetX.y, (float)rnd.NextDouble());
                position += tangent * this.Radius * Mathf.Lerp(this.OffsetZ.x, this.OffsetZ.y, (float)rnd.NextDouble());
                scale *= Mathf.Lerp(obj.MinMaxScaleSize.x, obj.MinMaxScaleSize.y, (float)rnd.NextDouble());
                rotation *= Quaternion.Euler(Vector3.Scale(rndVector * this.AngleJitter, s));

                RaycastHit hit;
                if (Physics.Raycast(this.transform.TransformPoint(position) + this.transform.up * 100, -this.transform.up, out hit, float.MaxValue, layer < 0 ? ~0 : layer))
                {
                    var up = Vector3.Lerp(Vector3.up, hit.normal.normalized, this.TerrainRotationFactor);
                    var yJitter = Mathf.Lerp(this.OffsetY.x, this.OffsetY.y, (float)rnd.NextDouble()) * 2;
                    position = this.transform.InverseTransformPoint(hit.point + up * yJitter);
                    rotation = Quaternion.Lerp(rotation, Quaternion.FromToRotation(Vector3.up, hit.normal.normalized) * rotation, this.TerrainRotationFactor);
                }
                else
                {
                    position.y += Mathf.Lerp(this.OffsetY.x, this.OffsetY.y, (float)rnd.NextDouble()) * 2;
                }

                go.transform.localPosition = position;
                go.transform.localScale = scale;
                go.transform.rotation = rotation;
            }
        }
    }

    [Flags]
    public enum Axis
    {
        X = 1 << 1,
        Y = 1 << 2,
        Z = 1 << 3
    }
}