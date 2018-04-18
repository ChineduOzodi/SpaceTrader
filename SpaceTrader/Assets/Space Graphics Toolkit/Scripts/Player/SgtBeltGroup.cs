using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
[AddComponentMenu("")]
public class SgtBeltGroup : MonoBehaviour
{
	public SgtBelt Belt;

	public Texture MainTex;

	public Texture HeightTex;

	[System.NonSerialized]
	public Material Material;

	[System.NonSerialized]
	public Vector3 TempPosition;

	[System.NonSerialized]
	public List<SgtBeltAsteroid> Asteroids = new List<SgtBeltAsteroid>(); // This is only used when generating the group

	public List<SgtBeltModel> Models = new List<SgtBeltModel>();

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

	public static SgtBeltGroup Create(SgtBelt belt)
	{
		var group = SgtComponentPool<SgtBeltGroup>.Pop("Group", belt.gameObject.layer, belt.transform);

		group.Belt = belt;

		return group;
	}

	public static void Pool(SgtBeltGroup group)
	{
		if (group != null)
		{
			group.Belt      = null;
			group.MainTex   = null;
			group.HeightTex = null;
			group.Material  = null;

			for (var i = group.Models.Count - 1; i >= 0; i--)
			{
				SgtBeltModel.Pool(group.Models[i]);
			}

			group.Models.Clear();

			SgtComponentPool<SgtBeltGroup>.Add(group);
		}
	}

	public static void MarkForDestruction(SgtBeltGroup group)
	{
		if (group != null)
		{
			group.Belt = null;

			group.gameObject.SetActive(true);
		}
	}

	protected virtual void OnDestroy()
	{
		for (var i = Models.Count - 1; i >= 0; i--)
		{
			SgtBeltModel.MarkForDestruction(Models[i]);
		}

		Models.Clear();
	}

	protected virtual void Update()
	{
		if (Belt == null)
		{
			Pool(this);
		}
	}
}
