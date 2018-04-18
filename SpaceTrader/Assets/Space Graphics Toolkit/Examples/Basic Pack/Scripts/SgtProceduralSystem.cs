using UnityEngine;
using System.Collections.Generic;

// This component will make a basic star system based on a bunch of preset materials, textures, and random values
[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Procedural System")]
public class SgtProceduralSystem : MonoBehaviour
{
	public Mesh SphereMesh;

	public Light MainLight;

	public List<Material> StarMaterials = new List<Material>();

	public List<Material> PlanetMaterials = new List<Material>();

	public List<Material> MoonMaterials = new List<Material>();

	public List<Cubemap> JovianTextures = new List<Cubemap>();

	public List<Object> GeneratedObjects = new List<Object>();

	public void Clear()
	{
		for (var i = GeneratedObjects.Count - 1; i >= 0; i--)
		{
			SgtHelper.Destroy(GeneratedObjects[i]);
		}

		GeneratedObjects.Clear();
	}

	public void AddStar()
	{
		var gameObject   = AddBasicGameObject("Star", transform, 0.0f, 0.0f, 0.0f, 10.0f, 0.5f, 1.0f);
		var meshFilter   = gameObject.AddComponent<MeshFilter>();
		var meshRenderer = gameObject.AddComponent<MeshRenderer>();
		var corona       = gameObject.AddComponent<SgtCorona>();

		corona.InnerPower = Random.Range(1.0f, 2.0f);

		meshFilter.sharedMesh = SphereMesh;

		meshRenderer.sharedMaterial = GetRandomElement(StarMaterials);

		corona.InnerRenderers.Add(meshRenderer);

		corona.OuterMeshes.Add(SphereMesh);
	}

	public void AddPlanet()
	{
		var gameObject   = AddBasicGameObject("Planet", transform, 5.0f, 30.0f, -5.0f, 10.0f, 0.5f, 1.0f);
		var meshFilter   = gameObject.AddComponent<MeshFilter>();
		var meshRenderer = gameObject.AddComponent<MeshRenderer>();
		var atmosphere   = gameObject.AddComponent<SgtAtmosphere>();

		meshFilter.sharedMesh = SphereMesh;

		meshRenderer.sharedMaterial = GetRandomElement(PlanetMaterials);

		atmosphere.InnerPower = Random.Range(1.0f, 2.0f);

		atmosphere.Lights.Add(MainLight);

		atmosphere.OuterMeshes.Add(SphereMesh);

		// Add moons?
		for (var i = Random.Range(0,2); i >= 0; i--)
		{
			AddMoon(gameObject.transform);
		}
	}

	public void AddMoon(Transform parent)
	{
		var gameObject   = AddBasicGameObject("Moon", parent, 1.0f, 3.0f, -30.0f, 30.0f, 0.05f, 0.2f);
		var meshFilter   = gameObject.AddComponent<MeshFilter>();
		var meshRenderer = gameObject.AddComponent<MeshRenderer>();

		meshFilter.sharedMesh = SphereMesh;

		meshRenderer.sharedMaterial = GetRandomElement(MoonMaterials);
	}

	public void AddJovian()
	{
		var gameObject = AddBasicGameObject("Jovian", transform, 5.0f, 30.0f, -5.0f, 10.0f, 0.5f, 1.0f);
		var jovian     = gameObject.AddComponent<SgtJovian>();

		jovian.MainTex = GetRandomElement(JovianTextures);

		jovian.Lights.Add(MainLight);

		jovian.Meshes.Add(SphereMesh);
	}

	public GameObject AddBasicGameObject(string name, Transform parent, float minOrbitDistance, float maxOrbitDistance, float minRotationSpeed, float maxRotationSpeed, float minScale, float maxScale)
	{
		// Create GO
		var gameObject  = new GameObject(name);
		var simpleOrbit = gameObject.AddComponent<SgtSimpleOrbit>();
		var rotate      = gameObject.AddComponent<SgtRotate>();
		var scale       = Random.Range(maxScale, maxScale);

		gameObject.transform.parent = parent;

		gameObject.transform.localScale = new Vector3(scale, scale, scale);

		// Setup orbit
		simpleOrbit.Angle = Random.Range(0.0f, 360.0f);

		simpleOrbit.Radius = Random.Range(minOrbitDistance, maxOrbitDistance);

		simpleOrbit.DegreesPerSecond = Random.Range(minRotationSpeed, maxRotationSpeed);

		// Setup rotation
		rotate.DegreesPerSecond = new Vector3(0.0f, Random.Range(minRotationSpeed, maxRotationSpeed), 0.0f);

		// Add to list and return
		GeneratedObjects.Add(gameObject);

		return gameObject;
	}

	protected virtual void Awake()
	{
		Clear();

		if (SphereMesh != null)
		{
			AddStar();

			for (var i = Random.Range(1, 6); i >= 0; i--)
			{
				AddPlanet();
			}

			for (var i = Random.Range(1, 4); i >= 0; i--)
			{
				AddJovian();
			}
		}
	}

	private T GetRandomElement<T>(List<T> list)
	{
		if (list != null && list.Count > 0)
		{
			var index = Random.Range(0, list.Count - 1);

			return list[index];
		}

		return default(T);
	}
}
