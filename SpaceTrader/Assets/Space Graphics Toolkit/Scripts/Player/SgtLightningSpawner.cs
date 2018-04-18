using UnityEngine;
using System.Collections.Generic;

[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Lightning Spawner")]
public class SgtLightningSpawner : MonoBehaviour
{
	[Tooltip("The minimum delay between lightning spawns")]
	public float DelayMin = 0.25f;

	[Tooltip("The maximum delay between lightning spawns")]
	public float DelayMax = 5.0f;

	[Tooltip("The minimum life of each spawned lightning")]
	public float LifeMin = 0.5f;

	[Tooltip("The maximum life of each spawned lightning")]
	public float LifeMax = 1.0f;

	[Tooltip("The minimum size of each spawned lightning in degrees")]
	public float SizeMin = 5.0f;

	[Tooltip("The maximum size of each spawned lightning in degrees")]
	public float SizeMax = 20.0f;

	[Tooltip("The detail of each spawned lightning based on its size")]
	public float Detail = 50.0f;

	[Tooltip("The radius of the spawned lightning mesh")]
	public float Radius = 1.0f;

	[Tooltip("The random color of the lightning")]
	public Gradient Colors;

	[Tooltip("The brightness of the lightning")]
	public float Brightness = 1.0f;

	[Tooltip("The random sprite used by the lightning")]
	public List<Sprite> Sprites;

	[SerializeField]
	private float delay;

	public Sprite RandomSprite
	{
		get
		{
			if (Sprites != null)
			{
				var count = Sprites.Count;

				if (count > 0)
				{
					var index = Random.Range(0, count);

					return Sprites[index];
				}
			}

			return null;
		}
	}

	public Color RandomColor
	{
		get
		{
			if (Colors != null)
			{
				return Colors.Evaluate(Random.value);
			}

			return Color.white;
		}
	}

	public SgtLightning Spawn()
	{
		var size      = Random.Range(SizeMin, SizeMax);
        var lightning = SgtLightning.Create(gameObject.layer, transform, size, Radius, Detail);

		if (lightning != null)
		{
			var life = Random.Range(LifeMin, LifeMax);

			lightning.Sprite     = RandomSprite;
			lightning.Color      = RandomColor;
			lightning.Brightness = Brightness;
            lightning.Life       = life;
			lightning.MaxLife    = life;

			lightning.transform.localRotation = Random.rotation;
		}

		return lightning;
	}

	protected virtual void Awake()
	{
		ResetDelay();
    }

	protected virtual void Update()
	{
		delay -= Time.deltaTime;

		// Spawn new lightning?
		if (delay <= 0.0f)
		{
			ResetDelay();

			Spawn();
        }
	}

	private void ResetDelay()
	{
		delay = Random.Range(DelayMin, DelayMax);
	}
}
