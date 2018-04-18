using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Skysphere")]
public class SgtSkysphere : MonoBehaviour
{
	public static List<SgtSkysphere> AllSkyspheres = new List<SgtSkysphere>();

	public List<Mesh> Meshes = new List<Mesh>();

	public Color Color = Color.white;

	public float Brightness = 1.0f;

	public SgtRenderQueue RenderQueue = SgtRenderQueue.Transparent;

	public int RenderQueueOffset;

	public Texture MainTex;

	public bool FollowObservers;

	[System.NonSerialized]
	private Material material;

	[SerializeField]
	private List<SgtSkysphereModel> models = new List<SgtSkysphereModel>();

	[System.NonSerialized]
	private bool tempPositionStored;

	[System.NonSerialized]
	private Vector3 tempPosition;

	public void UpdateState()
	{
		UpdateMaterial();
		UpdateModels();
	}

	public static SgtSkysphere CreateSkysphere(int layer = 0, Transform parent = null)
	{
		return CreateSkysphere(layer, parent, Vector3.zero, Quaternion.identity, Vector3.one);
	}

	public static SgtSkysphere CreateSkysphere(int layer, Transform parent, Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
	{
		var gameObject = SgtHelper.CreateGameObject("Skysphere", layer, parent, localPosition, localRotation, localScale);
		var skysphere  = gameObject.AddComponent<SgtSkysphere>();

		return skysphere;
	}

#if UNITY_EDITOR
	[UnityEditor.MenuItem(SgtHelper.GameObjectMenuPrefix + "Skysphere", false, 10)]
	public static void CreateSkysphereMenuItem()
	{
		var parent    = SgtHelper.GetSelectedParent();
		var skysphere = CreateSkysphere(parent != null ? parent.gameObject.layer : 0, parent);

		SgtHelper.SelectAndPing(skysphere);
	}
#endif

	protected virtual void OnEnable()
	{
#if UNITY_EDITOR
		if (AllSkyspheres.Count == 0)
		{
			SgtHelper.RepaintAll();
		}
#endif
		AllSkyspheres.Add(this);

		Camera.onPreCull    += CameraPreCull;
		Camera.onPostRender += CameraPostRender;

		for (var i = models.Count - 1; i >= 0; i--)
		{
			var model = models[i];

			if (model != null)
			{
				model.gameObject.SetActive(true);
			}
		}
	}

	protected virtual void OnDisable()
	{
		AllSkyspheres.Remove(this);

		Camera.onPreCull    -= CameraPreCull;
		Camera.onPostRender -= CameraPostRender;

		for (var i = models.Count - 1; i >= 0; i--)
		{
			var model = models[i];

			if (model != null)
			{
				model.gameObject.SetActive(false);
			}
		}
	}

	protected virtual void OnDestroy()
	{
		SgtHelper.Destroy(material);

		for (var i = models.Count - 1; i >= 0; i--)
		{
			SgtSkysphereModel.MarkForDestruction(models[i]);
		}

		models.Clear();
	}

	protected virtual void Update()
	{
		UpdateState();
	}

	private void UpdateMaterial()
	{
		if (material == null) material = SgtHelper.CreateTempMaterial(SgtHelper.ShaderNamePrefix + "Skysphere");

		var color       = SgtHelper.Brighten(Color, Brightness);
		var renderQueue = (int)RenderQueue + RenderQueueOffset;

		material.renderQueue = renderQueue;
		material.SetTexture("_MainTex", MainTex);
		material.SetColor("_Color", color);
	}

	private void UpdateModels()
	{
		models.RemoveAll(m => m == null);

		if (Meshes.Count != models.Count)
		{
			SgtHelper.ResizeArrayTo(ref models, Meshes.Count, i => SgtSkysphereModel.Create(this), m => SgtSkysphereModel.Pool(m));
		}

		for (var i = Meshes.Count - 1; i >= 0; i--)
		{
			models[i].ManualUpdate(Meshes[i], material);
		}
	}

	private void CameraPreCull(Camera camera)
	{
		if (FollowObservers == true)
		{
			for (var i = models.Count - 1; i >= 0; i--)
			{
				var model = models[i];

				if (model != null)
				{
					if (model.TempSet == false)
					{
						model.TempSet      = true;
						model.TempPosition = model.transform.position;
					}

					model.transform.position = camera.transform.position;
				}
			}
		}
	}

	private void CameraPostRender(Camera camera)
	{
		if (FollowObservers == true)
		{
			for (var i = models.Count - 1; i >= 0; i--)
			{
				var model = models[i];

				if (model != null && model.TempSet == true)
				{
					model.TempSet = false;

					model.transform.position = model.TempPosition;
				}
			}
		}
	}
}
