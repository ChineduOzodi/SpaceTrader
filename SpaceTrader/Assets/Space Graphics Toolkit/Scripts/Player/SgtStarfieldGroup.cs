using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
[AddComponentMenu("")]
public class SgtStarfieldGroup : MonoBehaviour
{
	public SgtStarfield Starfield;

	public Texture Texture;

	[System.NonSerialized]
	public Material Material;

	[System.NonSerialized]
	public string[] LastKeywords;

	[System.NonSerialized]
	public bool TempSet;

	[System.NonSerialized]
	public Vector3 TempPosition;

	[System.NonSerialized]
	public List<SgtStarfieldStar> Stars = new List<SgtStarfieldStar>(); // This is only used when generating the group

	public List<SgtStarfieldModel> Models = new List<SgtStarfieldModel>();

	public void ManualUpdate()
	{
		for (var i = Models.Count - 1; i >= 0; i--)
		{
			var model = Models[i];

			if (model != null)
			{
				model.ManualUpdate();
			}
		}
	}

	public static SgtStarfieldGroup Create(SgtStarfield starfield)
	{
		var group = SgtComponentPool<SgtStarfieldGroup>.Pop("Group", starfield.gameObject.layer, starfield.transform);

		group.Starfield = starfield;

		return group;
	}

	public static void Pool(SgtStarfieldGroup group)
	{
		if (group != null)
		{
			group.Starfield    = null;
			group.Texture      = null;
			group.Material     = null;
			group.LastKeywords = null;

			for (var i = group.Models.Count - 1; i >= 0; i--)
			{
				SgtStarfieldModel.Pool(group.Models[i]);
			}

			group.Models.Clear();

			SgtComponentPool<SgtStarfieldGroup>.Add(group);
		}
	}

	public static void MarkForDestruction(SgtStarfieldGroup group)
	{
		if (group != null)
		{
			group.Starfield = null;

			group.gameObject.SetActive(true);
		}
	}

	protected virtual void OnDestroy()
	{
		SgtHelper.Destroy(Material);

		for (var i = Models.Count - 1; i >= 0; i--)
		{
			SgtStarfieldModel.MarkForDestruction(Models[i]);
		}

		Models.Clear();
	}

	protected virtual void Update()
	{
		if (Starfield == null)
		{
			Pool(this);
		}
	}
}
