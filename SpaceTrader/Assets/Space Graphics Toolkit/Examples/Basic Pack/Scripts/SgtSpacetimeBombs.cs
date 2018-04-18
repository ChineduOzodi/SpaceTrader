using UnityEngine;
using System.Collections.Generic;

// This component spawns spacetime bombs when you click the button
[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Spacetime Bombs")]
public class SgtSpacetimeBombs : MonoBehaviour
{
	public GameObject BombPrefab;

	public SgtSpacetime Spacetime;

	public void SpawnBomb()
	{
		if (BombPrefab != null && Spacetime != null)
		{
			var bomb     = SgtHelper.CloneGameObject(BombPrefab, transform).GetComponent<SgtSpacetimeBomb>();
			var position = new Vector3(Random.Range(-5.0f, 5.0f), 0.0f, Random.Range(-5.0f, 5.0f));

			bomb.Spacetime = Spacetime;

			bomb.transform.localPosition = position;
		}
	}

	protected virtual void Update()
	{
		if (Input.GetButtonDown("Jump") == true)
		{
			SpawnBomb();
		}
	}
}
