using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Singularity")]
public class SgtSingularity : MonoBehaviour
{
	public static List<SgtSingularity> AllSingularities = new List<SgtSingularity>();

	public List<Mesh> Meshes = new List<Mesh>();

	public Color Color = Color.white;

	public float Brightness = 1.0f;

	public SgtRenderQueue RenderQueue = SgtRenderQueue.Transparent;

	public int RenderQueueOffset;

	public float Power = 10.0f;

	public float EdgePower = 10.0f;

	public bool Hole;

	[SgtRange(0.0f, 0.25f)]
	public float HoleSize = 0.02f;

	public float HolePower = 2.0f;

	[System.NonSerialized]
	private Material material;

	[SerializeField]
	private List<SgtSingularityModel> models = new List<SgtSingularityModel>();

	private static List<string> keywords = new List<string>();

	public void UpdateState()
	{
		UpdateMaterial();
		UpdateModels();
	}

	public static SgtSingularity CreateSingularity(int layer = 0, Transform parent = null)
	{
		return CreateSingularity(layer, parent, Vector3.zero, Quaternion.identity, Vector3.one);
	}

	public static SgtSingularity CreateSingularity(int layer, Transform parent, Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
	{
		var gameObject  = SgtHelper.CreateGameObject("Singularity", layer, parent, localPosition, localRotation, localScale);
		var singularity = gameObject.AddComponent<SgtSingularity>();

		return singularity;
	}

#if UNITY_EDITOR
	[UnityEditor.MenuItem(SgtHelper.GameObjectMenuPrefix + "Singularity", false, 10)]
	public static void CreateSingularityMenuItem()
	{
		var parent      = SgtHelper.GetSelectedParent();
		var singularity = CreateSingularity(parent != null ? parent.gameObject.layer : 0, parent);

		SgtHelper.SelectAndPing(singularity);
	}
#endif

	protected virtual void OnEnable()
	{
#if UNITY_EDITOR
		if (AllSingularities.Count == 0)
		{
			SgtHelper.RepaintAll();
		}
#endif
		AllSingularities.Add(this);

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
		AllSingularities.Remove(this);

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
			SgtSingularityModel.MarkForDestruction(models[i]);
		}

		models.Clear();
	}

	protected virtual void Update()
	{
		UpdateState();
	}

	private void UpdateMaterial()
	{
		if (material == null) material = SgtHelper.CreateTempMaterial(SgtHelper.ShaderNamePrefix + "Singularity");

		var color       = SgtHelper.Brighten(Color, Brightness);
		var renderQueue = (int)RenderQueue + RenderQueueOffset;

		material.renderQueue = renderQueue;
		material.SetColor("_Color", color);
		material.SetVector("_Center", SgtHelper.NewVector4(transform.position, 1.0f));
		material.SetFloat("_Power", Power);
		material.SetFloat("_EdgePower", EdgePower);

		if (Hole == true)
		{
			keywords.Add("SGT_A");

			material.SetFloat("_HoleSize", HoleSize);
			material.SetFloat("_HolePower", HolePower);
		}

		SgtHelper.SetKeywords(material, keywords); keywords.Clear();
	}

	private void UpdateModels()
	{
		models.RemoveAll(m => m == null);

		if (Meshes.Count != models.Count)
		{
			SgtHelper.ResizeArrayTo(ref models, Meshes.Count, i => SgtSingularityModel.Create(this), m => SgtSingularityModel.Pool(m));
		}

		for (var i = Meshes.Count - 1; i >= 0; i--)
		{
			models[i].ManualUpdate(Meshes[i], material);
		}
	}
}
