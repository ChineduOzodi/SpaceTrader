using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(SgtTerrain))]
public abstract class SgtTerrainModifier : MonoBehaviour
{
	[System.NonSerialized]
	protected SgtTerrain terrain;

	protected virtual void OnEnable()
	{
		if (terrain == null) terrain = GetComponent<SgtTerrain>();

		MarkAsDirty();
	}

	protected virtual void OnDisable()
	{
		if (terrain == null) terrain = GetComponent<SgtTerrain>();

		MarkAsDirty();
	}

#if UNITY_EDITOR
	protected virtual void Reset()
	{
		//MarkAsDirty();
	}

	protected virtual void OnValidate()
	{
		MarkAsDirty();
	}
#endif

	protected void MarkAsDirty()
	{
		if (terrain == null) terrain = GetComponent<SgtTerrain>();

		terrain.MarkMeshAsDirty();
	}
}
