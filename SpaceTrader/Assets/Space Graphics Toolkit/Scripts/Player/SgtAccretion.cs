using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Accretion")]
public class SgtAccretion : SgtRing
{
	public static List<SgtAccretion> AllAccretions = new List<SgtAccretion>();

	public Texture DustTex;

	public float Age;

	public float TimeScale = 0.125f;

	[Range(0.0f, 10.0f)]
	public float Twist = 2.0f;

	[Range(1.0f, 10.0f)]
	public float TwistBias = 2.0f;

	public bool ReverseTwist;

	public static SgtAccretion CreateAccretion(int layer, Transform parent = null)
	{
		return CreateAccretion(layer, parent, Vector3.zero, Quaternion.identity, Vector3.one);
	}

	public static SgtAccretion CreateAccretion(int layer, Transform parent, Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
	{
		var gameObject = SgtHelper.CreateGameObject("Accretion", layer, parent, localPosition, localRotation, localScale);
		var accretion  = gameObject.AddComponent<SgtAccretion>();

		return accretion;
	}

	protected override void OnEnable()
	{
		base.OnEnable();

		AllAccretions.Add(this);
	}

	protected override void OnDisable()
	{
		base.OnDisable();

		AllAccretions.Remove(this);
	}

	protected override void Update()
	{
		Age += Time.deltaTime * TimeScale;

		base.Update();
	}

	protected override void UpdateMaterial()
	{
		if (material == null) material = SgtHelper.CreateTempMaterial(SgtHelper.ShaderNamePrefix + "Accretion");

		material.SetTexture("_DustTex", DustTex);
		material.SetFloat("_Twist", ReverseTwist == true ? -Twist : Twist);
		material.SetFloat("_TwistBias", TwistBias);
		material.SetFloat("_Age", Age);

		base.UpdateMaterial();
	}

	protected override void RegenerateMesh()
	{
		mesh = SgtObjectPool<Mesh>.Add(mesh, m => m.Clear());

		SgtProceduralMesh.Clear();
		{
			if (SegmentDetail >= 3)
			{
				var angleTotal = SgtHelper.Divide(Mathf.PI * 2.0f, SegmentCount);
				var angleStep  = SgtHelper.Divide(angleTotal, SegmentDetail);
				var coordStep  = SgtHelper.Reciprocal(SegmentDetail);

				for (var i = 0; i <= SegmentDetail; i++)
				{
					var coord = coordStep * i;
					var angle = angleStep * i;
					var sin   = Mathf.Sin(angle);
					var cos   = Mathf.Cos(angle);
					var iPos  = new Vector3(sin * InnerRadius, 0.0f, cos * InnerRadius);
					var oPos  = new Vector3(sin * OuterRadius, 0.0f, cos * OuterRadius);

					SgtProceduralMesh.PushPosition(iPos);
					SgtProceduralMesh.PushPosition(oPos);

					SgtProceduralMesh.PushNormal(Vector3.up);
					SgtProceduralMesh.PushNormal(Vector3.up);

					SgtProceduralMesh.PushCoord1(0.0f, coord * InnerRadius);
					SgtProceduralMesh.PushCoord1(1.0f, coord * OuterRadius);

					SgtProceduralMesh.PushCoord2(InnerRadius, 0.0f);
					SgtProceduralMesh.PushCoord2(OuterRadius, 0.0f);
				}
			}
		}
		SgtProceduralMesh.SplitStrip(HideFlags.DontSave);

		mesh = SgtProceduralMesh.Pop(); SgtProceduralMesh.Discard();
	}

#if UNITY_EDITOR
	[UnityEditor.MenuItem(SgtHelper.GameObjectMenuPrefix + "Accretion", false, 10)]
	public static void CreateAccretionMenuItem()
	{
		var parent    = SgtHelper.GetSelectedParent();
		var accretion = CreateAccretion(parent != null ? parent.gameObject.layer : 0, parent);

		SgtHelper.SelectAndPing(accretion);
	}
#endif
}
